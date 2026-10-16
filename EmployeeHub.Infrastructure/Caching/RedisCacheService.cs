using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using EmployeeHub.Application.Services.Caching;
using StackExchange.Redis;

namespace EmployeeHub.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }

    public async Task<bool> TryAcquireAsync(string key, TimeSpan expiration)
    {
        var database = _redis.GetDatabase();

        return await database.StringSetAsync(
            key,
            "1",
            expiration,
            When.NotExists);
    }


    public async Task<T?> GetAsync<T>(string key)
    {
        var cachedData = await _cache.GetStringAsync(key);

        if (cachedData is null)
            return default;

        return JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration)
    {
        var serializedData = JsonSerializer.Serialize(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        await _cache.SetStringAsync(
            key,
            serializedData,
            options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}

