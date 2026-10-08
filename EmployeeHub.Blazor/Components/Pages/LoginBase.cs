using EmployeeHub.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace EmployeeHub.Blazor.Components.Pages;

public class LoginBase : ComponentBase
{
    [Inject]
    protected AuthApiService AuthApi { get; set; } = default!;

    [Inject]
    protected ITokenService TokenService { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    protected string Username { get; set; } = string.Empty;
    protected string Password { get; set; } = string.Empty;
    protected string ErrorMessage { get; set; } = string.Empty;
    public bool IsLoggingIn { get; set; }

    protected async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        IsLoggingIn = true;

        try
        {
            var response = await AuthApi.LoginAsync(
                Username,
                Password);

            if (response is null)
            {
                ErrorMessage = "Invalid username or password.";
                return;
            }

            // We'll handle the JWT token in the next step.
            await TokenService.SetTokenAsync(response.Token);

            Navigation.NavigateTo("/home");
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}