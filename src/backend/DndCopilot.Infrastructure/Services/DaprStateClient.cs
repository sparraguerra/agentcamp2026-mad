using System.Text;
using System.Text.Json;
using DndCopilot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DndCopilot.Infrastructure.Services;

/// <summary>
/// Dapr State Store client for agent memory persistence.
/// Implements the Dapr Agents pattern of ConversationDaprStateMemory using Dapr HTTP API.
/// </summary>
public class DaprStateClient : IDaprStateClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DaprStateClient> _logger;
    private readonly string _daprHttpPort;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public DaprStateClient(
        HttpClient httpClient,
        ILogger<DaprStateClient> logger,
        string daprHttpPort = "3500")
    {
        _httpClient = httpClient;
        _logger = logger;
        _daprHttpPort = daprHttpPort;
    }

    /// <inheritdoc />
    public async Task<T?> GetStateAsync<T>(string storeName, string key) where T : class
    {
        try
        {
            var url = $"http://localhost:{_daprHttpPort}/v1.0/state/{Uri.EscapeDataString(storeName)}/{Uri.EscapeDataString(key)}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                    return null;

                return JsonSerializer.Deserialize<T>(content, JsonOptions);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            _logger.LogWarning("Failed to get state for key {Key} from {Store}. Status: {Status}",
                key, storeName, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting state for key {Key} from {Store}", key, storeName);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SaveStateAsync<T>(string storeName, string key, T value)
    {
        try
        {
            var url = $"http://localhost:{_daprHttpPort}/v1.0/state/{Uri.EscapeDataString(storeName)}";
            var stateEntry = new[]
            {
                new { key, value }
            };

            var json = JsonSerializer.Serialize(stateEntry, JsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to save state for key {Key} to {Store}. Status: {Status}",
                    key, storeName, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving state for key {Key} to {Store}", key, storeName);
        }
    }

    /// <inheritdoc />
    public async Task DeleteStateAsync(string storeName, string key)
    {
        try
        {
            var url = $"http://localhost:{_daprHttpPort}/v1.0/state/{Uri.EscapeDataString(storeName)}/{Uri.EscapeDataString(key)}";
            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to delete state for key {Key} from {Store}. Status: {Status}",
                    key, storeName, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting state for key {Key} from {Store}", key, storeName);
        }
    }
}
