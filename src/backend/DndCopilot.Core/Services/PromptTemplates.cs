namespace DndCopilot.Core.Services;

/// <summary>
/// Collection of prompt templates for NPC agent AI interactions.
/// </summary>
public static class PromptTemplates
{
    /// <summary>
    /// System prompt for the observation phase.
    /// </summary>
    public const string ObservationSystemPrompt = @"You are an NPC perception system in a D&D game. 
Your role is to describe what an NPC observes in their environment.
Be concise and focus on details relevant to the NPC's perspective.
Consider the NPC's position, nearby entities, and environmental factors.
Respond in 2-3 sentences describing the scene from the NPC's viewpoint.";

    /// <summary>
    /// System prompt for the decision phase.
    /// </summary>
    public const string DecisionSystemPrompt = @"You are an NPC decision-making system in a D&D game.
Based on the NPC's personality, goals, and observations, choose the most appropriate action.
Consider the NPC's survival instincts, motivations, and relationships.
Always respond in the specified format with ACTION, TARGET, REASONING, and CONFIDENCE fields.
The action must be one from the available actions list.
Confidence should be between 0.0 and 1.0.";

    /// <summary>
    /// System prompt for the action execution phase.
    /// </summary>
    public const string ActionSystemPrompt = @"You are an NPC action narrator in a D&D game.
Describe the outcome of an NPC's action and provide appropriate dialogue.
The outcome should be descriptive and fit the game's fantasy setting.
The dialogue should match the NPC's personality and the situation.
Always respond in the specified format with OUTCOME and DIALOGUE fields.";

    /// <summary>
    /// Template for generating NPC dialogue based on personality traits.
    /// </summary>
    /// <param name="npcName">The name of the NPC.</param>
    /// <param name="personality">The NPC's personality traits.</param>
    /// <param name="situation">The current situation.</param>
    /// <returns>Formatted prompt for dialogue generation.</returns>
    public static string GetDialoguePrompt(string npcName, string personality, string situation)
    {
        return $@"Generate dialogue for an NPC in a D&D game.

NPC Name: {npcName}
Personality: {personality}
Situation: {situation}

Provide a single line of dialogue that:
1. Matches the NPC's personality
2. Is appropriate for the situation
3. Uses fantasy-appropriate language
4. Is between 10-50 words

DIALOGUE:";
    }

    /// <summary>
    /// Template for generating NPC reaction to player actions.
    /// </summary>
    /// <param name="npcName">The name of the NPC.</param>
    /// <param name="npcMood">The NPC's current mood.</param>
    /// <param name="playerAction">The action the player took.</param>
    /// <returns>Formatted prompt for reaction generation.</returns>
    public static string GetReactionPrompt(string npcName, string npcMood, string playerAction)
    {
        return $@"Generate an NPC reaction to a player's action in a D&D game.

NPC Name: {npcName}
NPC Mood: {npcMood}
Player Action: {playerAction}

Describe how the NPC reacts in 1-2 sentences, including:
1. Physical reaction (body language, expression)
2. Verbal response (if any)

REACTION:";
    }

    /// <summary>
    /// Template for combat decision-making.
    /// </summary>
    /// <param name="npcName">The name of the NPC.</param>
    /// <param name="npcHealth">Current health percentage.</param>
    /// <param name="combatSituation">Description of the combat situation.</param>
    /// <param name="availableAbilities">List of abilities the NPC can use.</param>
    /// <returns>Formatted prompt for combat decision.</returns>
    public static string GetCombatDecisionPrompt(string npcName, double npcHealth, string combatSituation, List<string> availableAbilities)
    {
        return $@"Make a combat decision for an NPC in a D&D game.

NPC Name: {npcName}
Health: {npcHealth:P0}
Situation: {combatSituation}
Available Abilities: {string.Join(", ", availableAbilities)}

Choose the best combat action considering:
1. Current health (flee if very low)
2. Tactical advantage
3. NPC's fighting style

ACTION: [chosen ability or action]
TARGET: [target description]
REASONING: [brief tactical reasoning]";
    }

    /// <summary>
    /// Template for social interaction decisions.
    /// </summary>
    /// <param name="npcName">The name of the NPC.</param>
    /// <param name="relationship">Relationship with the player.</param>
    /// <param name="context">Social context of the interaction.</param>
    /// <returns>Formatted prompt for social decision.</returns>
    public static string GetSocialDecisionPrompt(string npcName, string relationship, string context)
    {
        return $@"Make a social interaction decision for an NPC in a D&D game.

NPC Name: {npcName}
Relationship with Player: {relationship}
Context: {context}

Decide how the NPC should respond socially:
ATTITUDE: [friendly/neutral/hostile/suspicious]
ACTION: [talk/trade/ignore/help/hinder]
DIALOGUE: [what the NPC says]
REASONING: [why this response fits]";
    }
}
