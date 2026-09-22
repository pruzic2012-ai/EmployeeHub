
using EmployeeHub.Api.Models;

namespace EmployeeHub.Api.Repository;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<Employee> AddEmployeeAsync(Employee employee);
    Task UpdateEmployeeAsync(Employee employee);
    Task<bool> DeleteEmployeeAsync(int id);


}
