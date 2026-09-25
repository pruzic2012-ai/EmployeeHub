
using EmployeeHub.Application.DTOs;

namespace EmployeeHub.Application.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();

    Task<EmployeeDto?> GetByIdAsync(int id);

    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);

    Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto);

    Task<bool> DeleteAsync(int id);
}
