namespace DndCopilot.Core.Interfaces;

/// <summary>
/// Interface for Dapr State Store operations, used for agent memory persistence.
/// Maps to the Dapr Agents pattern of ConversationDaprStateMemory.
/// </summary>
public interface IDaprStateClient
{
    /// <summary>
    /// Gets state from the Dapr state store.
    /// </summary>
    Task<T?> GetStateAsync<T>(string storeName, string key) where T : class;

    /// <summary>
    /// Saves state to the Dapr state store.
    /// </summary>
    Task SaveStateAsync<T>(string storeName, string key, T value);

    /// <summary>
    /// Deletes state from the Dapr state store.
    /// </summary>
    Task DeleteStateAsync(string storeName, string key);
}
