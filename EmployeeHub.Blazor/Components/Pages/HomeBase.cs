using EmployeeHub.Blazor.Models;
using EmployeeHub.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace EmployeeHub.Blazor.Components.Pages;

public class HomeBase : ComponentBase
{
    [Inject]
    protected EmployeeApiService EmployeeApi { get; set; } = default!;

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
}