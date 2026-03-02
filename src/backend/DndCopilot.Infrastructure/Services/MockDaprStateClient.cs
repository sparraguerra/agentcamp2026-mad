using DndCopilot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DndCopilot.Infrastructure.Services;

/// <summary>
/// Mock implementation of IDaprStateClient for development/testing when Dapr is not available.
/// Stores state in memory using a dictionary.
/// </summary>
public class MockDaprStateClient : IDaprStateClient
{
    private readonly ILogger<MockDaprStateClient> _logger;
    private readonly Dictionary<string, Dictionary<string, object>> _storage = new();

    public MockDaprStateClient(ILogger<MockDaprStateClient> logger)
    {
        _logger = logger;
        _logger.LogInformation("Using MockDaprStateClient (in-memory state store)");
    }

    public Task<T?> GetStateAsync<T>(string storeName, string key) where T : class
    {
        _logger.LogDebug("Mock GetStateAsync: store={StoreName}, key={Key}", storeName, key);

        if (!_storage.ContainsKey(storeName))
        {
            _logger.LogDebug("Store {StoreName} not found", storeName);
            return Task.FromResult<T?>(null);
        }

        if (_storage[storeName].TryGetValue(key, out var value))
        {
            _logger.LogDebug("Found value for key {Key} in store {StoreName}", key, storeName);
            return Task.FromResult(value as T);
        }

        _logger.LogDebug("Key {Key} not found in store {StoreName}", key, storeName);
        return Task.FromResult<T?>(null);
    }

    public Task SaveStateAsync<T>(string storeName, string key, T value)
    {
        _logger.LogDebug("Mock SaveStateAsync: store={StoreName}, key={Key}", storeName, key);

        if (!_storage.ContainsKey(storeName))
        {
            _storage[storeName] = new Dictionary<string, object>();
            _logger.LogDebug("Created new store {StoreName}", storeName);
        }

        if (value != null)
        {
            _storage[storeName][key] = value;
            _logger.LogDebug("Saved value for key {Key} in store {StoreName}", key, storeName);
        }

        return Task.CompletedTask;
    }

    public Task DeleteStateAsync(string storeName, string key)
    {
        _logger.LogDebug("Mock DeleteStateAsync: store={StoreName}, key={Key}", storeName, key);

        if (_storage.ContainsKey(storeName))
        {
            _storage[storeName].Remove(key);
            _logger.LogDebug("Deleted key {Key} from store {StoreName}", key, storeName);
        }

        return Task.CompletedTask;
    }
}
