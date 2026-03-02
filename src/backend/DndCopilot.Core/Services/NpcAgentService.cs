using System.Text.Json;
using DndCopilot.Core.Interfaces;

namespace DndCopilot.Core.Services;

/// <summary>
/// NPC Agent Service following the Dapr Agents pattern:
/// - Agent Profile: name, role, goal, instructions
/// - LLM: via IFoundryAiClient (Dapr Conversation API)
/// - Memory: conversation history persisted in Dapr State Store
/// - Pub/Sub: game events published via IDaprEventPublisher
/// See: https://docs.dapr.io/developing-ai/dapr-agents/dapr-agents-core-concepts/
/// </summary>
public class NpcAgentService : INpcAgentService
{
    private readonly IFoundryAiClient _foundryAiClient;
    private readonly IDaprEventPublisher _eventPublisher;
    private readonly IDaprStateClient _stateClient;
    private const string StateStoreName = "npc-statestore";

    public NpcAgentService(
        IFoundryAiClient foundryAiClient,
        IDaprEventPublisher eventPublisher,
        IDaprStateClient stateClient)
    {
        _foundryAiClient = foundryAiClient;
        _eventPublisher = eventPublisher;
        _stateClient = stateClient;
    }

    /// <inheritdoc />
    public async Task<NpcActResult> RunAsync(
        NpcAgentProfile profile,
        string playerMessage,
        string sessionId,
        Dictionary<string, object>? gameState = null)
    {
        // 1. Load conversation memory from Dapr State Store
        var memoryKey = $"agent-memory-{profile.Name.ToLower()}-{sessionId}";
        var memory = await _stateClient.GetStateAsync<List<AgentConversationMessage>>(StateStoreName, memoryKey)
                     ?? new List<AgentConversationMessage>();

        // 2. Build system prompt from agent profile (name, role, goal, instructions)
        var systemPrompt = BuildAgentSystemPrompt(profile, gameState);

        // 3. Build conversation context from memory
        var conversationContext = BuildConversationContext(memory, playerMessage);

        // 4. Call LLM via Dapr Conversation API
        var aiResponse = await _foundryAiClient.GenerateCompletionAsync(conversationContext, new FoundryAiParameters
        {
            Temperature = 0.7,
            MaxTokens = 400,
            SystemPrompt = systemPrompt
        });

        var dialogueLine = aiResponse.Success && !string.IsNullOrEmpty(aiResponse.Content)
            ? aiResponse.Content.Trim()
            : GetFallbackDialogue(profile.Name, profile.Role);

        // 5. Store conversation turn in memory (Dapr State Store)
        memory.Add(new AgentConversationMessage { Role = "user", Content = playerMessage });
        memory.Add(new AgentConversationMessage { Role = "assistant", Content = dialogueLine });

        // Keep last 20 messages to avoid unbounded memory growth
        if (memory.Count > 20)
            memory = memory.GetRange(memory.Count - 20, 20);

        await _stateClient.SaveStateAsync(StateStoreName, memoryKey, memory);

        // 6. Publish interaction event via Dapr Pub/Sub
        var interactionEvent = new GameEvent
        {
            EventType = "npc.interaction",
            Source = profile.Name,
            Target = "player",
            Data = new Dictionary<string, object>
            {
                ["action"] = "talk",
                ["location"] = profile.Location,
                ["sessionId"] = sessionId
            }
        };
        await _eventPublisher.PublishEventAsync("npc-events", interactionEvent);

        return new NpcActResult
        {
            Success = true,
            ActionPerformed = "talk",
            DialogueLine = dialogueLine,
            Outcome = $"{profile.Name} responds to the player.",
            GeneratedEvents = new List<GameEvent> { interactionEvent }
        };
    }

    private static string BuildAgentSystemPrompt(NpcAgentProfile profile, Dictionary<string, object>? gameState)
    {
        var instructions = string.Join("\n- ", profile.Instructions);
        var stateContext = gameState != null && gameState.Count > 0
            ? $"\n\nPlayer context: Level {gameState.GetValueOrDefault("playerLevel", "unknown")}, " +
              $"Class {gameState.GetValueOrDefault("playerClass", "unknown")}, " +
              $"Gold {gameState.GetValueOrDefault("playerGold", "unknown")}"
            : "";

        return $@"You are {profile.Name}, a {profile.Role} in a D&D fantasy game.
Your goal: {profile.Goal}

Instructions:
- {instructions}

Rules:
- Stay in character at all times.
- Respond with dialogue only — no narration, no action descriptions, no quotation marks.
- Keep responses between 1-3 sentences.
- Reference past conversation if relevant.{stateContext}";
    }

    private static string BuildConversationContext(List<AgentConversationMessage> memory, string currentMessage)
    {
        if (memory.Count == 0)
            return currentMessage;

        var context = new System.Text.StringBuilder();
        // Include recent conversation history as context
        foreach (var msg in memory)
        {
            var role = msg.Role == "user" ? "Player" : "You";
            context.AppendLine($"{role}: {msg.Content}");
        }
        context.AppendLine($"Player: {currentMessage}");
        context.AppendLine("Respond to the player's latest message:");

        return context.ToString();
    }

    private static string GetFallbackDialogue(string npcName, string role)
    {
        return (npcName, role) switch
        {
            ("Grundy", _) => "Escucha, tronco: en las ruinas hay movida con goblins. Si no te rajas, te puedes llevar buena pasta.",
            ("Marcus", _) => "I've been crafting weapons for 30 years. Need something special? I can make it, given the right materials.",
            ("Elara", _) => "Ah, alma aventurera... deja que mi laud convierta tus pasos en cancion antes de que vuelvas al camino.",
            ("Bencomo", _) => "Habla bajito, mi nino, que aqui hay oidos por todos lados. Lo de los goblins es solo la punta, chacho; detras se mueve algo peor en la Torre.",
            ("Explorador", _) => "Zona reconocida. Veo objetos interesantes, pero primero valoro riesgo, peso y utilidad antes de recoger nada.",
            _ => "Greetings, traveler. What brings you here?"
        };
    }

    /// <inheritdoc />
    public async Task<NpcObserveResult> ObserveAsync(NpcObserveRequest request)
    {
        var result = new NpcObserveResult
        {
            Success = true,
            NpcId = request.NpcId.ToString()
        };

        // Categorize nearby characters as threats, opportunities, or neutral
        foreach (var character in request.NearbyCharacters)
        {
            // Use simple heuristics for classification
            var classification = ClassifyEntity(character, request.GameState);
            
            switch (classification)
            {
                case "threat":
                    result.PerceivedThreats.Add(character);
                    break;
                case "opportunity":
                    result.PerceivedOpportunities.Add(character);
                    break;
                default:
                    result.PerceivedNeutral.Add(character);
                    break;
            }
        }

        // Generate environment summary using AI
        var envPrompt = BuildObservationPrompt(request);
        var aiResponse = await _foundryAiClient.GenerateCompletionAsync(envPrompt, new FoundryAiParameters
        {
            Temperature = 0.3,
            MaxTokens = 200,
            SystemPrompt = PromptTemplates.ObservationSystemPrompt
        });

        result.EnvironmentSummary = aiResponse.Success 
            ? aiResponse.Content 
            : $"{request.NpcName} observes the surroundings at {request.CurrentLocation}.";

        result.ContextualData["location"] = request.CurrentLocation;
        result.ContextualData["objectCount"] = request.NearbyObjects.Count;
        result.ContextualData["characterCount"] = request.NearbyCharacters.Count;

        // Publish observation event
        await _eventPublisher.PublishEventAsync("npc-events", new GameEvent
        {
            EventType = "npc.observed",
            Source = request.NpcName,
            Target = request.CurrentLocation,
            Data = new Dictionary<string, object>
            {
                ["threats"] = result.PerceivedThreats,
                ["opportunities"] = result.PerceivedOpportunities
            }
        });

        return result;
    }

    /// <inheritdoc />
    public async Task<NpcDecideResult> DecideAsync(NpcDecideRequest request)
    {
        var result = new NpcDecideResult { Success = true };

        // Build decision prompt
        var decisionPrompt = BuildDecisionPrompt(request);
        
        var aiResponse = await _foundryAiClient.GenerateCompletionAsync(decisionPrompt, new FoundryAiParameters
        {
            Temperature = 0.5,
            MaxTokens = 300,
            SystemPrompt = PromptTemplates.DecisionSystemPrompt
        });

        if (aiResponse.Success && !string.IsNullOrEmpty(aiResponse.Content))
        {
            var parsedDecision = ParseDecisionResponse(aiResponse.Content, request.AvailableActions);
            result.ChosenAction = parsedDecision.Action;
            result.Reasoning = parsedDecision.Reasoning;
            result.TargetEntity = parsedDecision.Target;
            result.Confidence = parsedDecision.Confidence;
        }
        else
        {
            // Fallback to heuristic decision
            result = MakeFallbackDecision(request);
        }

        // Publish decision event
        await _eventPublisher.PublishEventAsync("npc-events", new GameEvent
        {
            EventType = "npc.decided",
            Source = request.NpcName,
            Target = result.TargetEntity,
            Data = new Dictionary<string, object>
            {
                ["action"] = result.ChosenAction,
                ["confidence"] = result.Confidence
            }
        });

        return result;
    }

    /// <inheritdoc />
    public async Task<NpcActResult> ActAsync(NpcActRequest request)
    {
        var result = new NpcActResult
        {
            Success = true,
            ActionPerformed = request.Action
        };

        // Build action execution prompt
        var actionPrompt = BuildActionPrompt(request);
        
        var aiResponse = await _foundryAiClient.GenerateCompletionAsync(actionPrompt, new FoundryAiParameters
        {
            Temperature = 0.7,
            MaxTokens = 400,
            SystemPrompt = PromptTemplates.ActionSystemPrompt
        });

        if (aiResponse.Success && !string.IsNullOrEmpty(aiResponse.Content))
        {
            var parsedAction = ParseActionResponse(aiResponse.Content);
            result.Outcome = parsedAction.Outcome;
            result.DialogueLine = parsedAction.Dialogue;
        }
        else
        {
            // Fallback outcome
            result.Outcome = $"{request.NpcName} attempts to {request.Action}.";
            result.DialogueLine = GetFallbackDialogue(request.Action);
        }

        // Generate events for state changes
        var actionEvent = new GameEvent
        {
            EventType = $"npc.action.{request.Action.ToLower().Replace(" ", "_")}",
            Source = request.NpcName,
            Target = request.TargetEntity,
            Data = new Dictionary<string, object>
            {
                ["outcome"] = result.Outcome,
                ["parameters"] = request.ActionParameters
            }
        };

        result.GeneratedEvents.Add(actionEvent);

        // Publish action event
        await _eventPublisher.PublishEventsAsync("npc-events", result.GeneratedEvents);

        return result;
    }

    private string ClassifyEntity(string entity, Dictionary<string, object> gameState)
    {
        // Simple heuristic classification - can be enhanced with AI
        var lowerEntity = entity.ToLower();
        
        if (lowerEntity.Contains("enemy") || lowerEntity.Contains("hostile") || lowerEntity.Contains("goblin"))
            return "threat";
        
        if (lowerEntity.Contains("merchant") || lowerEntity.Contains("ally") || lowerEntity.Contains("friend"))
            return "opportunity";
        
        return "neutral";
    }

    private string BuildObservationPrompt(NpcObserveRequest request)
    {
        return $@"NPC Name: {request.NpcName}
Location: {request.CurrentLocation}
Nearby Characters: {string.Join(", ", request.NearbyCharacters)}
Nearby Objects: {string.Join(", ", request.NearbyObjects)}

Describe what this NPC observes in their environment in 2-3 sentences.";
    }

    private string BuildDecisionPrompt(NpcDecideRequest request)
    {
        return $@"NPC: {request.NpcName}
Personality: {request.NpcPersonality}
Goal: {request.NpcGoal}

Observations:
- Threats: {string.Join(", ", request.Observation.PerceivedThreats)}
- Opportunities: {string.Join(", ", request.Observation.PerceivedOpportunities)}
- Environment: {request.Observation.EnvironmentSummary}

Available Actions: {string.Join(", ", request.AvailableActions)}

Choose the best action for this NPC and explain the reasoning. Format:
ACTION: [chosen action]
TARGET: [target entity if any]
REASONING: [brief explanation]
CONFIDENCE: [0.0-1.0]";
    }

    private string BuildActionPrompt(NpcActRequest request)
    {
        return $@"NPC: {request.NpcName}
Action: {request.Action}
Target: {request.TargetEntity}
Parameters: {JsonSerializer.Serialize(request.ActionParameters)}

Describe the outcome of this action and provide a dialogue line for the NPC. Format:
OUTCOME: [action result description]
DIALOGUE: [what the NPC says]";
    }

    private (string Action, string Target, string Reasoning, double Confidence) ParseDecisionResponse(string response, List<string> availableActions)
    {
        var action = availableActions.FirstOrDefault() ?? "wait";
        var target = "";
        var reasoning = "";
        var confidence = 0.5;

        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.StartsWith("ACTION:", StringComparison.OrdinalIgnoreCase))
                action = line["ACTION:".Length..].Trim();
            else if (line.StartsWith("TARGET:", StringComparison.OrdinalIgnoreCase))
                target = line["TARGET:".Length..].Trim();
            else if (line.StartsWith("REASONING:", StringComparison.OrdinalIgnoreCase))
                reasoning = line["REASONING:".Length..].Trim();
            else if (line.StartsWith("CONFIDENCE:", StringComparison.OrdinalIgnoreCase))
                double.TryParse(line["CONFIDENCE:".Length..].Trim(), out confidence);
        }

        return (action, target, reasoning, confidence);
    }

    private (string Outcome, string Dialogue) ParseActionResponse(string response)
    {
        var outcome = "";
        var dialogue = "";

        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.StartsWith("OUTCOME:", StringComparison.OrdinalIgnoreCase))
                outcome = line["OUTCOME:".Length..].Trim();
            else if (line.StartsWith("DIALOGUE:", StringComparison.OrdinalIgnoreCase))
                dialogue = line["DIALOGUE:".Length..].Trim();
        }

        return (outcome, dialogue);
    }

    private NpcDecideResult MakeFallbackDecision(NpcDecideRequest request)
    {
        // Simple rule-based fallback
        var result = new NpcDecideResult { Success = true };

        if (request.Observation.PerceivedThreats.Any())
        {
            result.ChosenAction = request.AvailableActions.Contains("flee") ? "flee" : 
                                  request.AvailableActions.Contains("defend") ? "defend" : 
                                  request.AvailableActions.FirstOrDefault() ?? "wait";
            result.TargetEntity = request.Observation.PerceivedThreats.First();
            result.Reasoning = "Perceived threat detected, taking defensive action.";
            result.Confidence = 0.7;
        }
        else if (request.Observation.PerceivedOpportunities.Any())
        {
            result.ChosenAction = request.AvailableActions.Contains("interact") ? "interact" : 
                                  request.AvailableActions.Contains("talk") ? "talk" : 
                                  request.AvailableActions.FirstOrDefault() ?? "wait";
            result.TargetEntity = request.Observation.PerceivedOpportunities.First();
            result.Reasoning = "Opportunity detected, engaging positively.";
            result.Confidence = 0.6;
        }
        else
        {
            result.ChosenAction = request.AvailableActions.Contains("patrol") ? "patrol" : 
                                  request.AvailableActions.Contains("idle") ? "idle" : 
                                  request.AvailableActions.FirstOrDefault() ?? "wait";
            result.Reasoning = "No immediate priorities, maintaining default behavior.";
            result.Confidence = 0.5;
        }

        return result;
    }

    private string GetFallbackDialogue(string action)
    {
        return action.ToLower() switch
        {
            "attack" => "Prepare to face my wrath!",
            "flee" => "I must retreat for now!",
            "talk" => "Greetings, traveler.",
            "trade" => "Let me show you my wares.",
            "defend" => "You shall not pass!",
            "search object" => "Voy a inspeccionar cada objeto antes de tocar nada.",
            "pick object" => "Este objeto compensa el riesgo. Lo recojo.",
            "leave object" => "No merece la pena exponerme por esto. Lo dejamos.",
            "report findings" => "Informe listo: objetivo analizado y decision tomada.",
            _ => "*remains silent*"
        };
    }
}
