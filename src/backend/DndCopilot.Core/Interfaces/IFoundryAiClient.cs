namespace DndCopilot.Core.Interfaces;

/// <summary>
/// Interface for Foundry AI integration for generating NPC responses.
/// </summary>
public interface IFoundryAiClient
{
    /// <summary>
    /// Generates a completion response from Foundry AI.
    /// </summary>
    /// <param name="prompt">The prompt to send to the AI.</param>
    /// <param name="parameters">Optional parameters for the AI request.</param>
    /// <returns>The AI-generated response.</returns>
    Task<FoundryAiResponse> GenerateCompletionAsync(string prompt, FoundryAiParameters? parameters = null);
}

/// <summary>
/// Response from Foundry AI.
/// </summary>
public class FoundryAiResponse
{
    public bool Success { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Parameters for Foundry AI requests.
/// </summary>
public class FoundryAiParameters
{
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 500;
    public string? SystemPrompt { get; set; }
    public Dictionary<string, object> AdditionalParameters { get; set; } = new();
}
