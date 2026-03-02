using DndCopilot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DndCopilot.Infrastructure.Services;

/// <summary>
/// Uses Dapr Conversation API by default and falls back to mock only on connection failures.
/// </summary>
public class FallbackFoundryAiClient : IFoundryAiClient
{
    private readonly IFoundryAiClient _primary;
    private readonly IFoundryAiClient _fallback;
    private readonly ILogger<FallbackFoundryAiClient> _logger;

    public FallbackFoundryAiClient(
        IFoundryAiClient primary,
        IFoundryAiClient fallback,
        ILogger<FallbackFoundryAiClient> logger)
    {
        _primary = primary;
        _fallback = fallback;
        _logger = logger;
    }

    public async Task<FoundryAiResponse> GenerateCompletionAsync(string prompt, FoundryAiParameters? parameters = null)
    {
        var primaryResponse = await _primary.GenerateCompletionAsync(prompt, parameters);
        if (primaryResponse.Success)
        {
            return primaryResponse;
        }

        if (!IsConnectionFailure(primaryResponse))
        {
            return primaryResponse;
        }

        _logger.LogWarning("Dapr Conversation API unavailable. Falling back to MockFoundryAiClient.");
        var fallbackResponse = await _fallback.GenerateCompletionAsync(prompt, parameters);
        if (fallbackResponse.Metadata != null)
        {
            fallbackResponse.Metadata["fallback_from"] = "dapr-conversation";
        }

        return fallbackResponse;
    }

    private static bool IsConnectionFailure(FoundryAiResponse response)
    {
        if (response.Metadata == null)
        {
            return false;
        }

        return response.Metadata.TryGetValue("error_type", out var value)
            && string.Equals(value?.ToString(), "connection", StringComparison.OrdinalIgnoreCase);
    }
}
