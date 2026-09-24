using EmployeeHub.Api.DTOs;
using EmployeeHub.Api.Models;
using EmployeeHub.Api.Repositories;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeHub.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    /*
     * I use structured logging for authentication events, 
     * but I make sure not to log sensitive information such as passwords, password hashes, or JWT tokens.
     * 
     */
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository
            .GetByUsernameAsync(request.Username);

        if (user is null)
        {
            _logger.LogWarning("Login attempt failed for username: {Username}", request.Username);
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password."
            };
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Login attempt failed for username: {Username}", request.Username);
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password."
            };
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is missing.");

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "30")),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        _logger.LogInformation("User {Username} logged in successfully with role: {Role}", user.Username, user.Role?.Name);

        return new LoginResponse
        {
            Success = true,
            Message = "Login successful.",
            Username = user.Username,
            Role = user.Role?.Name,
            Token = tokenString
        };
    }
}
