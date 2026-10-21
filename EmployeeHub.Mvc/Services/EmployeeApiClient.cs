using System.Net.Http.Headers;
using System.Net.Http.Json;
using EmployeeHub.Mvc.Models;

namespace EmployeeHub.Mvc.Services;

public class EmployeeApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EmployeeApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<EmployeeViewModel>> GetEmployeesAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        using var client =
            _httpClientFactory.CreateClient("EmployeeHubApi");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync(
            "api/v1/employees",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var employees =
            await response.Content.ReadFromJsonAsync<List<EmployeeViewModel>>(
                cancellationToken: cancellationToken);

        return employees ?? new List<EmployeeViewModel>();
    }
}