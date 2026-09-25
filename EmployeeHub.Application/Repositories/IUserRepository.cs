
using EmployeeHub.Domain.Entities;

namespace EmployeeHub.Application.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
}
