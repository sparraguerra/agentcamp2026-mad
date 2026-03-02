using System.Net.Http;
using System.Net.Sockets;
using Dapr.AI.Conversation;
using Dapr.AI.Conversation.ConversationRoles;
using DndCopilot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DndCopilot.Infrastructure.Services;

/// <summary>
/// Implementation of IFoundryAiClient that uses Dapr Conversation API to generate NPC responses.
/// </summary>
public class DaprConversationAiClient : IFoundryAiClient
{
    private readonly DaprConversationClient _conversationClient;
    private readonly ILogger<DaprConversationAiClient> _logger;
    private readonly string _componentName;

    public DaprConversationAiClient(
        DaprConversationClient conversationClient,
        ILogger<DaprConversationAiClient> logger,
        string componentName = "openai")
    {
        _conversationClient = conversationClient;
        _logger = logger;
        _componentName = componentName;
    }

    /// <inheritdoc />
    public async Task<FoundryAiResponse> GenerateCompletionAsync(string prompt, FoundryAiParameters? parameters = null)
    {
        try
        {
            parameters ??= new FoundryAiParameters();

            var messages = new List<IConversationMessage>();

            if (!string.IsNullOrEmpty(parameters.SystemPrompt))
            {
                messages.Add(new SystemMessage { Content = [new MessageContent(parameters.SystemPrompt)] });
            }

            messages.Add(new UserMessage { Content = [new MessageContent(prompt)] });

            var inputs = new List<ConversationInput>
            {
                new(messages, null)
            };

            var options = new ConversationOptions(_componentName)
            {
                Temperature = parameters.Temperature
            };

            _logger.LogDebug("Calling Dapr Conversation API with component {Component}", _componentName);

            var response = await _conversationClient.ConverseAsync(inputs, options);

            var resultContent = response.Outputs?.FirstOrDefault()?.Choices?.FirstOrDefault()?.Message?.Content
                                ?? string.Empty;

            _logger.LogInformation("Dapr Conversation API returned successfully");

            return new FoundryAiResponse
            {
                Success = true,
                Content = resultContent,
                Metadata = new Dictionary<string, object>
                {
                    ["source"] = "dapr-conversation",
                    ["component"] = _componentName
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Dapr Conversation API");
            var isConnectionFailure = IsConnectionFailure(ex);
            return new FoundryAiResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                Metadata = new Dictionary<string, object>
                {
                    ["source"] = "dapr-conversation",
                    ["component"] = _componentName,
                    ["error_type"] = isConnectionFailure ? "connection" : "unknown"
                }
            };
        }
    }

    private static bool IsConnectionFailure(Exception ex)
    {
        for (var current = ex; current != null; current = current.InnerException)
        {
            if (current is HttpRequestException || current is TaskCanceledException || current is SocketException)
            {
                return true;
            }
        }

        return false;
    }
}
