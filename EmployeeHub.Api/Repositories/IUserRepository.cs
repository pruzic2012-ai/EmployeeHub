using EmployeeHub.Api.Models;

namespace EmployeeHub.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
}
