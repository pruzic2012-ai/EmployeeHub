using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeeHub.Application.Services;
using EmployeeHub.Application.DTOs;

namespace EmployeeHub.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    /*
     *The login request goes to the authentication controller, which calls the authentication service. 
     *The service retrieves the user through the repository and uses a password hasher 
     *to verify the supplied password against the stored password hash. 
     *If the credentials are valid, we generate an authentication token.
     * 
     */

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }
}
