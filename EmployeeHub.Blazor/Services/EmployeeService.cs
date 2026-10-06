using System.Net.Http.Json;
using EmployeeHub.Blazor.Models;

namespace EmployeeHub.Blazor.Services;

public class EmployeeApiService
{
    private readonly HttpClient _httpClient;

    public EmployeeApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("EmployeeHubApi");
    }

    public async Task<List<EmployeeModel>> GetEmployeesAsync()
    {
        var employees = await _httpClient.GetFromJsonAsync<List<EmployeeModel>>(
            "api/v1/employees");

        return employees ?? new List<EmployeeModel>();
    }
}