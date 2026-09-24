using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace EmployeeHub.Api.Tests.Integration;

public class EmployeeApiTests
{
    [Fact]
    public async Task Api_CanStartAndRespond()
    {
        // Arrange
        await using var application =
            new CustomWebApplicationFactory();

        using var client = application.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/employees");

        Assert.Equal(
            System.Net.HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetEmployees_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        await using var application =
            new WebApplicationFactory<Program>();

        using var client = application.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/employees");

        // Assert
        Assert.Equal(
            System.Net.HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateEmployee_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        await using var application =
            new CustomWebApplicationFactory();

        using var client = application.CreateClient();

        var employee = new
        {
            firstName = "Test",
            lastName = "User",
            email = "test.user@example.com",
            department = "IT",
            salary = 75000
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/v1/employees",
            employee);

        // Assert
        Assert.Equal(
            System.Net.HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateEmployee_AsAdmin_ReturnsCreated()
    {
        // Arrange
        await using var application =
            new CustomWebApplicationFactory();

        using var client = application.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            "Admin");

        var employee = new
        {
            firstName = "Integration",
            lastName = "Test",
            email = "integration.test@example.com",
            department = "IT",
            salary = 75000
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/v1/employees",
            employee);

        // Assert
        Assert.Equal(
            System.Net.HttpStatusCode.Created,
            response.StatusCode);
    }
}