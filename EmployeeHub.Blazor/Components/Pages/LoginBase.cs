using EmployeeHub.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace EmployeeHub.Blazor.Components.Pages;

public class LoginBase : ComponentBase
{
    [Inject]
    protected AuthApiService AuthApi { get; set; } = default!;

    protected string Username { get; set; } = string.Empty;
    protected string Password { get; set; } = string.Empty;
    protected string ErrorMessage { get; set; } = string.Empty;

    protected async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        var response = await AuthApi.LoginAsync(
            Username,
            Password);

        if (response is null)
        {
            ErrorMessage = "Invalid username or password.";
            return;
        }

        // We'll handle the JWT token in the next step.
    }
}