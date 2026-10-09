using System.Net;
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


    public async Task<CreateEmployeeResult> CreateEmployeeAsync(
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
            return CreateEmployeeResult.Unauthenticated;
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return CreateEmployeeResult.Forbidden;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return CreateEmployeeResult.BadRequest;
        }

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return CreateEmployeeResult.Conflict;
        }

        if (!response.IsSuccessStatusCode)
        {
            return CreateEmployeeResult.Failed;
        }

        return CreateEmployeeResult.Success;
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

    public async Task<UpdateEmployeeResult> UpdateEmployeeAsync(int id, UpdateEmployeeModel employee)
    {
        var response = await SendAuthorizedAsync(
            HttpMethod.Put,
            $"api/v1/employees/{id}",
            JsonContent.Create(employee));

        if (response is null)
        {
            return UpdateEmployeeResult.Unauthenticated;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            return UpdateEmployeeResult.ConcurrencyConflict;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return UpdateEmployeeResult.NotFound;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            return UpdateEmployeeResult.Forbidden;
        }

        response.EnsureSuccessStatusCode();

        return UpdateEmployeeResult.Success;
    }


    public async Task<DeleteEmployeeResult> DeleteEmployeeAsync(int id)
    {
        var response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            $"api/v1/employees/{id}");

        if (response is null)
        {
            return DeleteEmployeeResult.Unauthenticated;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return DeleteEmployeeResult.NotFound;
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return DeleteEmployeeResult.Forbidden;
        }

        response.EnsureSuccessStatusCode();

        return DeleteEmployeeResult.Success;
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