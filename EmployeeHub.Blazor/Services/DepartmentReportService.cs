
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace EmployeeHub.Blazor.Services;

public class DepartmentReportService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly NavigationManager _navigation;

    public DepartmentReportService(
        IHttpClientFactory httpClientFactory,
        ITokenService tokenService,
        NavigationManager navigationManager)
    {
        _httpClient = httpClientFactory.CreateClient("EmployeeHubApi");
        _tokenService = tokenService;
        _navigation = navigationManager;
    }

    public async Task<string?> GenerateReportAsync(string department)
    {
        var token = await _tokenService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            _navigation.NavigateTo("/login", replace: true);
            return null;
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "api/department-report");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(new
        {
            department
        });

        using var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
