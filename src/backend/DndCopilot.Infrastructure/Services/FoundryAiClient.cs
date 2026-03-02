using System.Text;
using System.Text.Json;
using DndCopilot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DndCopilot.Infrastructure.Services;

/// <summary>
/// Client for Foundry AI integration.
/// </summary>
public class FoundryAiClient : IFoundryAiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FoundryAiClient> _logger;
    private readonly string _apiEndpoint;
    private readonly string _apiKey;

    public FoundryAiClient(HttpClient httpClient, ILogger<FoundryAiClient> logger, string apiEndpoint, string apiKey)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiEndpoint = apiEndpoint;
        _apiKey = apiKey;
    }

    /// <inheritdoc />
    public async Task<FoundryAiResponse> GenerateCompletionAsync(string prompt, FoundryAiParameters? parameters = null)
    {
        try
        {
            parameters ??= new FoundryAiParameters();

            var requestBody = new
            {
                prompt = prompt,
                system_prompt = parameters.SystemPrompt,
                temperature = parameters.Temperature,
                max_tokens = parameters.MaxTokens,
                additional_parameters = parameters.AdditionalParameters
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _apiEndpoint);
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            if (!string.IsNullOrEmpty(_apiKey))
            {
                request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            }

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return new FoundryAiResponse
                {
                    Success = true,
                    Content = jsonResponse.TryGetProperty("content", out var contentProp) 
                        ? contentProp.GetString() ?? string.Empty 
                        : responseContent,
                    Metadata = new Dictionary<string, object>
                    {
                        ["status_code"] = (int)response.StatusCode
                    }
                };
            }
            else
            {
                _logger.LogWarning("Foundry AI request failed with status {StatusCode}", response.StatusCode);
                return new FoundryAiResponse
                {
                    Success = false,
                    ErrorMessage = $"API request failed with status {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Foundry AI");
            return new FoundryAiResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}

/// <summary>
/// Stub implementation of Foundry AI client for development/testing.
/// </summary>
public class StubFoundryAiClient : IFoundryAiClient
{
    private readonly ILogger<StubFoundryAiClient> _logger;

    public StubFoundryAiClient(ILogger<StubFoundryAiClient> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<FoundryAiResponse> GenerateCompletionAsync(string prompt, FoundryAiParameters? parameters = null)
    {
        _logger.LogInformation("StubFoundryAiClient: Generating stub response for prompt");

        // Generate stub response based on prompt content
        var response = GenerateStubResponse(prompt);

        return Task.FromResult(new FoundryAiResponse
        {
            Success = true,
            Content = response,
            Metadata = new Dictionary<string, object>
            {
                ["stub"] = true
            }
        });
    }

    private string GenerateStubResponse(string prompt)
    {
        if (prompt.Contains("observe", StringComparison.OrdinalIgnoreCase))
        {
            return "The NPC surveys their surroundings carefully, noting the positions of nearby adventurers and potential threats in the dimly lit area.";
        }
        else if (prompt.Contains("ACTION:", StringComparison.OrdinalIgnoreCase))
        {
            return @"ACTION: wait
TARGET: none
REASONING: No immediate threats or opportunities detected, maintaining vigilant stance.
CONFIDENCE: 0.75";
        }
        else if (prompt.Contains("OUTCOME:", StringComparison.OrdinalIgnoreCase))
        {
            return @"OUTCOME: The NPC successfully completes their action, maintaining their position in the area.
DIALOGUE: Hmm, all seems quiet for now...";
        }
        else
        {
            return "The NPC considers their options carefully.";
        }
    }
}
