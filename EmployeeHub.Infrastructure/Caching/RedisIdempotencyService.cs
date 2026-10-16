using EmployeeHub.Application.Services.Caching;
using EmployeeHub.Application.Services.Idempotency;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeHub.Infrastructure.Caching;

public class RedisIdempotencyService : IIdempotencyService
{
    private readonly ICacheService _cache;

    private const string LockPrefix = "idempotency:lock:";
    private const string ResultPrefix = "idempotency:result:";

    public RedisIdempotencyService(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<bool> TryAcquireAsync(string key, TimeSpan expiration)
    {
        return await _cache.TryAcquireAsync(LockPrefix + key, expiration);
    }

    public async Task<T?> GetResultAsync<T>(string key)
    {
        return await _cache.GetAsync<T>(ResultPrefix + key);
    }

    public async Task StoreResultAsync<T>(string key,T result, TimeSpan expiration)
    {
        await _cache.SetAsync(ResultPrefix + key,result,expiration);
    }

    public async Task ReleaseAsync(string key)
    {
        await _cache.RemoveAsync(LockPrefix + key);
    }
}

