using EmployeeHub.Api.Models;

namespace EmployeeHub.Api.Repository;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
}
