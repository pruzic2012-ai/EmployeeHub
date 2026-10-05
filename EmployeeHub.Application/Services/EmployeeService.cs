

using EmployeeHub.Application.DTOs;
using EmployeeHub.Domain.Entities;
using EmployeeHub.Application.Repositories;
using EmployeeHub.Application.Services.Caching;
using Microsoft.Extensions.Logging;
using EmployeeHub.Application.Exceptions;

//using Microsoft.Extensions.Caching.Distributed;
//using System.Text.Json;

namespace EmployeeHub.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<EmployeeService> _logger;
    private readonly ICacheService _cache;

    private const string EmployeesCacheKey = "employees:all";

    public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService> logger, ICacheService cache)
    {
        _repository = repository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
       // I don't want my business service tightly coupled to Redis.
       // I created an ICacheService abstraction.
       // The employee service depends on that abstraction,
       // while RedisCacheService handles the Redis implementation and serialization details.
        var cachedEmployees =
        await _cache.GetAsync<List<EmployeeDto>>(EmployeesCacheKey);

        if (cachedEmployees is not null)
        {
            _logger.LogInformation(
                "Employee list retrieved from cache");

            return cachedEmployees;
        }

        _logger.LogInformation(
            "Employee list not found in cache. Retrieving from database");


        var employees = await _repository.GetAllEmployeesAsync();
        var employeeDtos = employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Department = e.Department,
            Salary = e.Salary,
            CreatedDate = e.CreatedDate
        }).ToList();

        await _cache.SetAsync(
        EmployeesCacheKey,
        employeeDtos,
        TimeSpan.FromMinutes(5));

        _logger.LogInformation(
            "Employee list stored in cache");

        return employeeDtos;    
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var employee = await _repository.GetEmployeeByIdAsync(id);

        if (employee == null)
        {
            return null;
        }

        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Department = employee.Department,
            Salary = employee.Salary,
            CreatedDate = employee.CreatedDate
        };
        
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        if(dto.Salary < 30000)
        {
            _logger.LogWarning("Employee salary validation failed for {FirstName} {LastName}",
            dto.FirstName,
            dto.LastName);
            throw new ArgumentException("Salary must be at least $30,000.");
        }

        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Department = dto.Department,
            Salary = dto.Salary,
            CreatedDate = DateTime.UtcNow
        };

        var createdEmployee = await _repository.AddEmployeeAsync(employee);

        await _cache.RemoveAsync(EmployeesCacheKey);
        _logger.LogInformation("Employee list cache invalidated after creating a new employee {EmployeeId}", createdEmployee.Id);

        return new EmployeeDto
        {
            Id = createdEmployee.Id,
            FirstName = createdEmployee.FirstName,
            LastName = createdEmployee.LastName,
            Email = createdEmployee.Email,
            Department = createdEmployee.Department,
            Salary = createdEmployee.Salary,
            CreatedDate = createdEmployee.CreatedDate
        };
    }

    public async Task<UpdateEmployeeResult> UpdateAsync(int id, UpdateEmployeeDto employee)
    {
        _logger.LogInformation("Updating employee with ID {EmployeeId}", id);

        var existingEmployee = await _repository.GetEmployeeByIdAsync(id);
        if (existingEmployee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", id);

            return UpdateEmployeeResult.NotFound;
        }

        existingEmployee.RowVersion = employee.RowVersion;

        // Update the employee properties
        existingEmployee.FirstName = employee.FirstName;
        existingEmployee.LastName = employee.LastName;
        existingEmployee.Email = employee.Email;
        existingEmployee.Department = employee.Department;
        existingEmployee.Salary = employee.Salary;

        try
        {
            await _repository.UpdateEmployeeAsync(existingEmployee);
        }
        catch (ConcurrencyException)
        {
            _logger.LogWarning(
                "Concurrency conflict while updating employee with ID {EmployeeId}",
                id);

            return UpdateEmployeeResult.ConcurrencyConflict;
        }

        await _cache.RemoveAsync(EmployeesCacheKey);
        _logger.LogInformation("Employee list cache invalidated after updating employee {EmployeeId}", id);

        return UpdateEmployeeResult.Updated;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting employee with ID {EmployeeId}", id);

        var employee = await _repository.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", id);
            return false;
        }

        await _repository.DeleteEmployeeAsync(id);

        await _cache.RemoveAsync(EmployeesCacheKey);
        _logger.LogInformation("Employee list cache invalidated after deleting employee {EmployeeId}", id);

        return true;
    }
}
