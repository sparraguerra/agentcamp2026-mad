using System.Text;
using System.Text.Json;
using DndCopilot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DndCopilot.Infrastructure.Services;

/// <summary>
/// Dapr pub/sub event publisher for game events.
/// </summary>
public class DaprEventPublisher : IDaprEventPublisher
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DaprEventPublisher> _logger;
    private readonly string _daprHttpPort;
    private readonly string _pubsubName;

    public DaprEventPublisher(HttpClient httpClient, ILogger<DaprEventPublisher> logger, string daprHttpPort = "3500", string pubsubName = "pubsub")
    {
        _httpClient = httpClient;
        _logger = logger;
        _daprHttpPort = daprHttpPort;
        _pubsubName = pubsubName;
    }

    /// <inheritdoc />
    public async Task<bool> PublishEventAsync(string topicName, GameEvent eventData)
    {
        try
        {
            var daprUrl = $"http://localhost:{_daprHttpPort}/v1.0/publish/{_pubsubName}/{topicName}";
            
            var content = new StringContent(
                JsonSerializer.Serialize(eventData),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(daprUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Published event {EventType} to topic {TopicName}", eventData.EventType, topicName);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to publish event to Dapr. Status: {StatusCode}", response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event to Dapr pub/sub");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> PublishEventsAsync(string topicName, IEnumerable<GameEvent> events)
    {
        var eventList = events.ToList();
        if (eventList.Count == 0)
        {
            return true;
        }

        var tasks = eventList.Select(eventData => PublishEventAsync(topicName, eventData));
        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }
}

/// <summary>
/// Stub implementation of Dapr event publisher for development/testing.
/// </summary>
public class StubDaprEventPublisher : IDaprEventPublisher
{
    private readonly ILogger<StubDaprEventPublisher> _logger;
    private readonly List<(string Topic, GameEvent Event)> _publishedEvents = new();

    public StubDaprEventPublisher(ILogger<StubDaprEventPublisher> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the list of published events (for testing purposes).
    /// </summary>
    public IReadOnlyList<(string Topic, GameEvent Event)> PublishedEvents => _publishedEvents.AsReadOnly();

    /// <inheritdoc />
    public Task<bool> PublishEventAsync(string topicName, GameEvent eventData)
    {
        _logger.LogInformation("StubDaprEventPublisher: Publishing event {EventType} to topic {TopicName}", eventData.EventType, topicName);
        _publishedEvents.Add((topicName, eventData));
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<bool> PublishEventsAsync(string topicName, IEnumerable<GameEvent> events)
    {
        foreach (var eventData in events)
        {
            _publishedEvents.Add((topicName, eventData));
            _logger.LogInformation("StubDaprEventPublisher: Publishing event {EventType} to topic {TopicName}", eventData.EventType, topicName);
        }
        return Task.FromResult(true);
    }

    /// <summary>
    /// Clears all published events (for testing purposes).
    /// </summary>
    public void ClearEvents()
    {
        _publishedEvents.Clear();
    }
}
