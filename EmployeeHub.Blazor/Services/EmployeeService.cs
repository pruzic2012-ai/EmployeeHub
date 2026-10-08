using System.Net.Http.Json;
using EmployeeHub.Blazor.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace EmployeeHub.Blazor.Services;

public class EmployeeService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly NavigationManager _navigation;

    public EmployeeService(
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
        var response = await SendAuthorizedAsync(
            HttpMethod.Get,
            "api/v1/employees");

        if (response is null)
        {
            return new List<EmployeeModel>();
        }

        response.EnsureSuccessStatusCode();

        var employees = await response.Content
            .ReadFromJsonAsync<List<EmployeeModel>>();

        return employees ?? new List<EmployeeModel>();
    }

    public async Task<EmployeeModel?> CreateEmployeeAsync(
    CreateEmployeeModel employee)
    {
        var response = await SendAuthorizedAsync(
            HttpMethod.Post,
            "api/v1/employees",
            JsonContent.Create(employee),
            request =>
            {
                request.Headers.Add(
                    "Idempotency-Key",
                    Guid.NewGuid().ToString());
            });

        if (response is null)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<EmployeeModel>();
    }

    public async Task<EmployeeModel?> GetEmployeeByIdAsync(int id)
    {
        var response = await SendAuthorizedAsync(
            HttpMethod.Get,
            $"api/v1/employees/{id}");

        if (response is null)
        {
            return null;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<EmployeeModel>();
    }

    public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeModel employee)
    {
        var response = await SendAuthorizedAsync(
            HttpMethod.Put,
            $"api/v1/employees/{id}",
            JsonContent.Create(employee));

        if (response is null)
        {
            return false;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            $"api/v1/employees/{id}");

        if (response is null)
        {
            return false;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }

    private async Task<HttpResponseMessage?> SendAuthorizedAsync(HttpMethod method,string url,
        HttpContent? content = null,Action<HttpRequestMessage>? configureRequest = null)
    {
        var token = await _tokenService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            _navigation.NavigateTo("/login", replace: true);
            return null;
        }

        var request = new HttpRequestMessage(method, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content = content;

        configureRequest?.Invoke(request);

        return await _httpClient.SendAsync(request);
    }
}