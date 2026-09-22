

namespace EmployeeHub.Api.DTOs;

public class LoginResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Username { get; set; }

    public string? Role { get; set; }

    public string? Token { get; set; }
}
