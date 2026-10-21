using System.Net.Http.Json;
using EmployeeHub.Mvc.Models;

namespace EmployeeHub.Mvc.Services;

public class AuthApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<LoginApiResponse> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        using var client =
            _httpClientFactory.CreateClient("EmployeeHubApi");

        using var response = await client.PostAsJsonAsync(
            "api/Auth/login",
            new
            {
                Username = username,
                Password = password
            },
            cancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<LoginApiResponse>(
                cancellationToken: cancellationToken);

        if (result is not null)
        {
            return result;
        }

        return new LoginApiResponse
        {
            Success = false,
            Message = response.IsSuccessStatusCode
                ? "The API returned an empty login response."
                : $"Login failed (HTTP {(int)response.StatusCode})."
        };
    }
}