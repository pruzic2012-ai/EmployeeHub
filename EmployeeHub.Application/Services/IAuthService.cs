using EmployeeHub.Application.DTOs;

namespace EmployeeHub.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
