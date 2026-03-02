namespace DndCopilot.Core.Interfaces;

/// <summary>
/// Interface for Dapr pub/sub event publishing.
/// </summary>
public interface IDaprEventPublisher
{
    /// <summary>
    /// Publishes an event to a Dapr pub/sub topic.
    /// </summary>
    /// <param name="topicName">The name of the topic to publish to.</param>
    /// <param name="eventData">The event data to publish.</param>
    /// <returns>True if the event was published successfully.</returns>
    Task<bool> PublishEventAsync(string topicName, GameEvent eventData);

    /// <summary>
    /// Publishes multiple events to a Dapr pub/sub topic.
    /// </summary>
    /// <param name="topicName">The name of the topic to publish to.</param>
    /// <param name="events">The events to publish.</param>
    /// <returns>True if all events were published successfully.</returns>
    Task<bool> PublishEventsAsync(string topicName, IEnumerable<GameEvent> events);
}
