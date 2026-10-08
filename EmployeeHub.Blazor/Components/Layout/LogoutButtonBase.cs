using EmployeeHub.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace EmployeeHub.Blazor.Components.Layout;

public class LogoutButtonBase : ComponentBase
{
    [Inject]
    protected ITokenService TokenService { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    protected async Task Logout()
    {
       await  TokenService.ClearTokenAsync();

        Navigation.NavigateTo("/");
    }
}