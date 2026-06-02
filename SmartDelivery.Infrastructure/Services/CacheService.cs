using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SmartDelivery.Application.Interfaces.Services;
using System.Collections.Concurrent;
using System.Text.Json;

namespace SmartDelivery.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;

        // Track all keys for pattern-based removal
        private static readonly ConcurrentDictionary<string, bool> _cacheKeys = new();

        private static readonly MemoryCacheEntryOptions DefaultOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2),
            Priority = CacheItemPriority.Normal
        };

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                if(_cache.TryGetValue(key, out string? json) && json is not null)
                {
                    var value = JsonSerializer.Deserialize<T>(json);
                    _logger.LogDebug("Cache HIT for key: {Key}", key);
                    return Task.FromResult(value);
                }
                _logger.LogDebug("Cache MISS for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cache key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
        }
        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var json = JsonSerializer.Serialize(value);
                var options = expiration.HasValue
                    ? new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
                    : DefaultOptions;

                _cache.Set(key, json, options);
                _cacheKeys.TryAdd(key, true);
                _logger.LogDebug("Cache SET for key: {Key} (expires in {Expiry})", key, expiration ?? TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache key: {Key}", key);
            }

            return Task.CompletedTask;
        }
        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _cache.Remove(key);
            _cacheKeys.TryRemove(key, out _);
            _logger.LogDebug("Cache REMOVE for key: {Key}", key);
            return Task.CompletedTask;
        }

        public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            var keysToRemove = _cacheKeys.Keys.Where(k => k.StartsWith(pattern)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _cacheKeys.TryRemove(key, out _);
            }

            _logger.LogDebug("Cache REMOVE by pattern '{Pattern}' — {Count} keys removed", pattern, keysToRemove.Count);
            return Task.CompletedTask;
        }

        
    }
}
