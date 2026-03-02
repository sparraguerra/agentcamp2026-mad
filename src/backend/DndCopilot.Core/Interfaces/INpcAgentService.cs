namespace DndCopilot.Core.Interfaces;

/// <summary>
/// Dapr Agent profile following the Dapr Agents pattern.
/// Defines an NPC agent's identity: name, role, goal, and instructions.
/// See: https://docs.dapr.io/developing-ai/dapr-agents/dapr-agents-core-concepts/
/// </summary>
public class NpcAgentProfile
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public List<string> Instructions { get; set; } = new();
    public string Location { get; set; } = string.Empty;
}

/// <summary>
/// Conversation message stored in agent memory (Dapr State Store).
/// </summary>
public class AgentConversationMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Interface for the NPC Agent Service that handles NPC AI behavior
/// following the Dapr Agents pattern: agent profile + LLM + memory + tools + pub/sub.
/// </summary>
public interface INpcAgentService
{
    /// <summary>
    /// Runs the agent's reasoning cycle for a player interaction.
    /// This is the main entry point, equivalent to Dapr Agents' agent.run().
    /// Uses conversation memory from Dapr State Store for context.
    /// </summary>
    /// <param name="profile">The NPC agent's profile definition.</param>
    /// <param name="playerMessage">The player's message or action.</param>
    /// <param name="sessionId">Session identifier for memory keying.</param>
    /// <param name="gameState">Current game state context.</param>
    /// <returns>The agent's response with dialogue and actions.</returns>
    Task<NpcActResult> RunAsync(NpcAgentProfile profile, string playerMessage, string sessionId, Dictionary<string, object>? gameState = null);

    /// <summary>
    /// Observes the game context and returns relevant information for NPC decision-making.
    /// </summary>
    Task<NpcObserveResult> ObserveAsync(NpcObserveRequest request);

    /// <summary>
    /// Decides the next action for an NPC based on observed context.
    /// </summary>
    Task<NpcDecideResult> DecideAsync(NpcDecideRequest request);

    /// <summary>
    /// Executes the decided action and returns the result.
    /// </summary>
    Task<NpcActResult> ActAsync(NpcActRequest request);
}

/// <summary>
/// Request for NPC observation phase.
/// </summary>
public class NpcObserveRequest
{
    public int NpcId { get; set; }
    public string NpcName { get; set; } = string.Empty;
    public string CurrentLocation { get; set; } = string.Empty;
    public List<string> NearbyCharacters { get; set; } = new();
    public List<string> NearbyObjects { get; set; } = new();
    public Dictionary<string, object> GameState { get; set; } = new();
}

/// <summary>
/// Result of NPC observation phase.
/// </summary>
public class NpcObserveResult
{
    public bool Success { get; set; }
    public string NpcId { get; set; } = string.Empty;
    public List<string> PerceivedThreats { get; set; } = new();
    public List<string> PerceivedOpportunities { get; set; } = new();
    public List<string> PerceivedNeutral { get; set; } = new();
    public string EnvironmentSummary { get; set; } = string.Empty;
    public Dictionary<string, object> ContextualData { get; set; } = new();
}

/// <summary>
/// Request for NPC decision phase.
/// </summary>
public class NpcDecideRequest
{
    public int NpcId { get; set; }
    public string NpcName { get; set; } = string.Empty;
    public string NpcPersonality { get; set; } = string.Empty;
    public string NpcGoal { get; set; } = string.Empty;
    public NpcObserveResult Observation { get; set; } = new();
    public List<string> AvailableActions { get; set; } = new();
}

/// <summary>
/// Result of NPC decision phase.
/// </summary>
public class NpcDecideResult
{
    public bool Success { get; set; }
    public string ChosenAction { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public Dictionary<string, object> ActionParameters { get; set; } = new();
}

/// <summary>
/// Request for NPC action execution phase.
/// </summary>
public class NpcActRequest
{
    public int NpcId { get; set; }
    public string NpcName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public Dictionary<string, object> ActionParameters { get; set; } = new();
    public Dictionary<string, object> GameState { get; set; } = new();
}

/// <summary>
/// Result of NPC action execution phase.
/// </summary>
public class NpcActResult
{
    public bool Success { get; set; }
    public string ActionPerformed { get; set; } = string.Empty;
    public string Outcome { get; set; } = string.Empty;
    public string DialogueLine { get; set; } = string.Empty;
    public List<GameEvent> GeneratedEvents { get; set; } = new();
    public Dictionary<string, object> StateChanges { get; set; } = new();
}

/// <summary>
/// Represents a game event generated by NPC actions for pub/sub.
/// </summary>
public class GameEvent
{
    public string EventType { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Data { get; set; } = new();
}
