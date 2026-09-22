using EmployeeHub.Api.DTOs;

namespace EmployeeHub.Api.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
