using EmployeeHub.Blazor.Models;
using EmployeeHub.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace EmployeeHub.Blazor.Components.Pages;

public class HomeBase : ComponentBase
{
    [Inject]
    protected EmployeeService EmployeeApi { get; set; } = default!;
    [Inject]
    protected IJSRuntime? JS { get; set; } = default;

    protected List<EmployeeModel> Employees { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        Employees = await EmployeeApi.GetEmployeesAsync();

        await InvokeAsync(StateHasChanged);
    }
    protected async Task DeleteEmployeeAsync(int id)
    {
        var success = await EmployeeApi.DeleteEmployeeAsync(id);

        if (!success)
        {
            return;
        }

        Employees = await EmployeeApi.GetEmployeesAsync();

        await InvokeAsync(StateHasChanged);
    }

    protected async Task ConfirmDeleteAsync(int id)
    {
        var confirmed = await JS.InvokeAsync<bool>(
            "confirm",
            "Are you sure you want to delete this employee?");

        if (!confirmed)
        {
            return;
        }

        await DeleteEmployeeAsync(id);
    }
}