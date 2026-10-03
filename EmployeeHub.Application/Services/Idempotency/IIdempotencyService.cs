using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeHub.Application.Services.Idempotency;

public interface IIdempotencyService
{
    Task<bool> TryAcquireAsync(string key, TimeSpan expiration);

    Task<T?> GetResultAsync<T>(string key);

    Task StoreResultAsync<T>(string key, T result, TimeSpan expiration);

    Task ReleaseAsync(string key);
}
