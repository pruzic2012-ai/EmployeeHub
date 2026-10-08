using Microsoft.JSInterop;

namespace EmployeeHub.Blazor.Services;

public class TokenService : ITokenService
{
    private const string TokenKey = "employeehub_token";

    private readonly IJSRuntime _jsRuntime;

    public TokenService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string?>(
            "sessionStorage.getItem",
            TokenKey);
    }

    public async Task SetTokenAsync(string token)
    {
        await _jsRuntime.InvokeVoidAsync(
            "sessionStorage.setItem",
            TokenKey,
            token);
    }

    public async Task ClearTokenAsync()
    {
        await _jsRuntime.InvokeVoidAsync(
            "sessionStorage.removeItem",
            TokenKey);
    }
}
