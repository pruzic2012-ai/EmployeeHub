using System.Net.Http.Json;
using EmployeeHub.Blazor.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace EmployeeHub.Blazor.Services;

public class EmployeeApiService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly NavigationManager _navigation;

    public EmployeeApiService(
    IHttpClientFactory httpClientFactory,
    ITokenService tokenService,
    NavigationManager navigationManager)
    {
        _httpClient = httpClientFactory.CreateClient("EmployeeHubApi");
        _tokenService = tokenService;
        _navigation = navigationManager;
    }

    public async Task<List<EmployeeModel>> GetEmployeesAsync()
    {
        var token = _tokenService.Token;

        if (string.IsNullOrWhiteSpace(token))
        {
            _navigation.NavigateTo("/login", replace: true);
            return new List<EmployeeModel>();
        }

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "api/v1/employees");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var employees = await response.Content
            .ReadFromJsonAsync<List<EmployeeModel>>();

        return employees ?? new List<EmployeeModel>();
    }
}