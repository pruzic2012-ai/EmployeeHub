using EmployeeHub.Api.DTOs;
using EmployeeHub.Api.Models;
using EmployeeHub.Api.Repository;

namespace EmployeeHub.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var employees = await _repository.GetAllEmployeesAsync();
        return employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Department = e.Department,
            Salary = e.Salary,
            CreatedDate = e.CreatedDate
        });
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

    public async Task<bool> UpdateAsync(int id, UpdateEmployeeDto employee)
    {
        var existingEmployee = await _repository.GetEmployeeByIdAsync(id);
        if (existingEmployee == null)
        {
            return false;
        }

        // Update the employee properties
        existingEmployee.FirstName = employee.FirstName;
        existingEmployee.LastName = employee.LastName;
        existingEmployee.Email = employee.Email;
        existingEmployee.Department = employee.Department;
        existingEmployee.Salary = employee.Salary;

        await _repository.UpdateEmployeeAsync(existingEmployee);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _repository.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return false;
        }

        await _repository.DeleteEmployeeAsync(id);
        return true;
    }
}
