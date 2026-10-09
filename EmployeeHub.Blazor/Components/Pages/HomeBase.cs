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
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    protected List<EmployeeModel> Employees { get; set; } = new();

    protected string ErrorMessage { get; set; } = string.Empty;

    protected int? DeletingEmployeeId { get; set; }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        try
        {
            Employees = await EmployeeApi.GetEmployeesAsync();
        }
        catch (HttpRequestException)
        {
            ErrorMessage =
                "Unable to load employees. Please check your connection and try again.";
        }

        await InvokeAsync(StateHasChanged);
    }



    protected async Task DeleteEmployeeAsync(int id)
    {
        if (DeletingEmployeeId.HasValue)
        {
            return;
        }

        ErrorMessage = string.Empty;
        DeletingEmployeeId = id;

        try
        {
            var result = await EmployeeApi.DeleteEmployeeAsync(id);

            switch (result)
            {
                case DeleteEmployeeResult.Success:
                    Employees = await EmployeeApi.GetEmployeesAsync();
                    break;

                case DeleteEmployeeResult.Unauthenticated:
                    Navigation.NavigateTo("/login", replace: true);
                    return;

                case DeleteEmployeeResult.NotFound:
                    ErrorMessage =
                        "The employee was not found. The list may be out of date.";
                    break;

                case DeleteEmployeeResult.Forbidden:
                    ErrorMessage =
                        "You don't have permission to delete employees.";
                    break;
            }
        }
        catch (HttpRequestException)
        {
            ErrorMessage =
                "Unable to delete the employee. Please try again.";
        }
        finally
        {
            DeletingEmployeeId = null;
        }

        await InvokeAsync(StateHasChanged);
    }


    protected async Task ConfirmDeleteAsync(int id)
    {
        if (JS is null)
        {
            // IJSRuntime not available (e.g. during prerender). Do not proceed.
            return;
        }

        var confirmed = await JS.InvokeAsync<bool>(
            "window.confirm",
            "Are you sure you want to delete this employee?");

        if (!confirmed)
        {
            return;
        }

        await DeleteEmployeeAsync(id);
    }
}