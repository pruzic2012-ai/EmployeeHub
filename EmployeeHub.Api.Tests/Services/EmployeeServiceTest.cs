using EmployeeHub.Application.DTOs;
using EmployeeHub.Domain.Entities;
using EmployeeHub.Application.Repositories;
using EmployeeHub.Application.Services;
using EmployeeHub.Application.Services.Caching;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using System.Data;

namespace EmployeeHub.Api.Tests.Services;

public class EmployeeServiceTests
{
    //I use unit tests to verify the service behavior independently of the database.
    //Moq allows me to mock the repository and simulate scenarios such as an employee being found or not found.
    //I use xUnit for unit testing and Moq to isolate the service from its dependencies.
    //I test both successful and failure scenarios. For example,
    //I can mock the repository to simulate an employee being found or not found,
    //and I verify that the service calls the repository and invalidates the Redis cache when appropriate.


    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsEmployeeDto()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            Department = "IT",
            Salary = 75000,
            CreatedDate = DateTime.UtcNow
        };

        var repositoryMock = new Mock<IEmployeeRepository>();
        var loggerMock = new Mock<ILogger<EmployeeService>>();
        var cacheMock = new Mock<ICacheService>();

        repositoryMock
            .Setup(r => r.GetEmployeeByIdAsync(1))
            .ReturnsAsync(employee);

        var service = new EmployeeService(
            repositoryMock.Object,
            loggerMock.Object,
            cacheMock.Object);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal("IT", result.Department);
        Assert.Equal(75000, result.Salary);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repositoryMock = new Mock<IEmployeeRepository>();
        var loggerMock = new Mock<ILogger<EmployeeService>>();
        var cacheMock = new Mock<ICacheService>();

        repositoryMock
            .Setup(r => r.GetEmployeeByIdAsync(999))
            .ReturnsAsync((Employee?)null);

        var service = new EmployeeService(
            repositoryMock.Object,
            loggerMock.Object,
            cacheMock.Object);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }


    //I wrote a unit test for a business rule.
    //The test provides an invalid salary of $25,000 and verifies that the service throws an
    //ArgumentException with the expected message.

    [Fact]
    public async Task CreateAsync_WhenSalaryIsBelowMinimum_ThrowsArgumentException()
    {
        // Arrange
        var repositoryMock = new Mock<IEmployeeRepository>();
        var loggerMock = new Mock<ILogger<EmployeeService>>();
        var cacheMock = new Mock<ICacheService>();

        var service = new EmployeeService(
            repositoryMock.Object,
            loggerMock.Object,
            cacheMock.Object);

        var dto = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Email = "test@example.com",
            Department = "IT",
            Salary = 25000
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(dto));

        Assert.Equal(
            "Salary must be at least $30,000.",
            exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesEmployeeAndInvalidatesCache()
    {
        // Arrange
        var repositoryMock = new Mock<IEmployeeRepository>();
        var loggerMock = new Mock<ILogger<EmployeeService>>();
        var cacheMock = new Mock<ICacheService>();

        repositoryMock
            .Setup(r => r.AddEmployeeAsync(It.IsAny<Employee>()))
            .ReturnsAsync((Employee employee) =>
            {
                employee.Id = 10;
                return employee;
            });

        var service = new EmployeeService(
            repositoryMock.Object,
            loggerMock.Object,
            cacheMock.Object);

        var dto = new CreateEmployeeDto
        {
            FirstName = "Mike",
            LastName = "Wilson",
            Email = "mike.wilson@example.com",
            Department = "IT",
            Salary = 75000
        };

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Mike", result.FirstName);
        Assert.Equal("Wilson", result.LastName);
        Assert.Equal("IT", result.Department);
        Assert.Equal(75000, result.Salary);

        repositoryMock.Verify(
            r => r.AddEmployeeAsync(It.IsAny<Employee>()),
            Times.Once);

        cacheMock.Verify(
            c => c.RemoveAsync("employees:all"),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var repositoryMock = new Mock<IEmployeeRepository>();
        var loggerMock = new Mock<ILogger<EmployeeService>>();
        var cacheMock = new Mock<ICacheService>();

        repositoryMock
            .Setup(r => r.GetEmployeeByIdAsync(999))
            .ReturnsAsync((Employee?)null);

        var service = new EmployeeService(
            repositoryMock.Object,
            loggerMock.Object,
            cacheMock.Object);

        var dto = new UpdateEmployeeDto
        {
            FirstName = "Updated",
            LastName = "Employee",
            Email = "updated@example.com",
            Department = "IT",
            Salary = 80000
        };

        // Act
        var result = await service.UpdateAsync(999, dto);

        // Assert
        Assert.False(result);

        repositoryMock.Verify(
            r => r.UpdateEmployeeAsync(It.IsAny<Employee>()),
            Times.Never);

        cacheMock.Verify(
            c => c.RemoveAsync("employees:all"),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeExists_UpdatesEmployeeAndInvalidatesCache()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Smith",
            Email = "john@example.com",
            Department = "IT",
            Salary = 70000,
            CreatedDate = DateTime.UtcNow
        };

        var repositoryMock = new Mock<IEmployeeRepository>();
        var loggerMock = new Mock<ILogger<EmployeeService>>();
        var cacheMock = new Mock<ICacheService>();

        repositoryMock
            .Setup(r => r.GetEmployeeByIdAsync(1))
            .ReturnsAsync(employee);

        var service = new EmployeeService(
            repositoryMock.Object,
            loggerMock.Object,
            cacheMock.Object);

        var dto = new UpdateEmployeeDto
        {
            FirstName = "John Updated",
            LastName = "Smith",
            Email = "john.updated@example.com",
            Department = "Management",
            Salary = 85000
        };

        // Act
        var result = await service.UpdateAsync(1, dto);

        // Assert
        Assert.True(result);

        Assert.Equal("John Updated", employee.FirstName);
        Assert.Equal("john.updated@example.com", employee.Email);
        Assert.Equal("Management", employee.Department);
        Assert.Equal(85000, employee.Salary);

        repositoryMock.Verify(
            r => r.UpdateEmployeeAsync(employee),
            Times.Once);

        cacheMock.Verify(
            c => c.RemoveAsync("employees:all"),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var repositoryMock = new Mock<IEmployeeRepository>();
        var loggerMock = new Mock<ILogger<EmployeeService>>();
        var cacheMock = new Mock<ICacheService>();

        repositoryMock
            .Setup(r => r.GetEmployeeByIdAsync(999))
            .ReturnsAsync((Employee?)null);

        var service = new EmployeeService(
            repositoryMock.Object,
            loggerMock.Object,
            cacheMock.Object);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        Assert.False(result);

        repositoryMock.Verify(
            r => r.DeleteEmployeeAsync(999),
            Times.Never);

        cacheMock.Verify(
            c => c.RemoveAsync("employees:all"),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeExists_DeletesEmployeeAndInvalidatesCache()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Smith",
            Email = "john@example.com",
            Department = "IT",
            Salary = 70000,
            CreatedDate = DateTime.UtcNow
        };

        var repositoryMock = new Mock<IEmployeeRepository>();
        var loggerMock = new Mock<ILogger<EmployeeService>>();
        var cacheMock = new Mock<ICacheService>();

        repositoryMock
            .Setup(r => r.GetEmployeeByIdAsync(1))
            .ReturnsAsync(employee);

        var service = new EmployeeService(
            repositoryMock.Object,
            loggerMock.Object,
            cacheMock.Object);

        // Act
        var result = await service.DeleteAsync(1);

        // Assert
        Assert.True(result);

        repositoryMock.Verify(
            r => r.DeleteEmployeeAsync(1),
            Times.Once);

        cacheMock.Verify(
            c => c.RemoveAsync("employees:all"),
            Times.Once);
    }

}