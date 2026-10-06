using EmployeeHub.Blazor.Models;
using System.Net.Http.Json;

namespace EmployeeHub.Blazor.Services;

public class AuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("EmployeeHubApi");
    }

    public async Task<LoginResponse?> LoginAsync(
        string username,
        string password)
    {
        var request = new LoginRequest
        {
            Username = username,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/Auth/login",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<LoginResponse>();
    }
}