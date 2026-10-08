using EmployeeHub.Blazor.Models;
using EmployeeHub.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace EmployeeHub.Blazor.Components.Pages;

public class EditEmployeeBase : ComponentBase
{
    [Parameter]
    public int Id { get; set; }

    [Inject]
    protected EmployeeService EmployeeApi { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    protected UpdateEmployeeModel? Employee { get; set; }

    protected string ErrorMessage { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        var employee = await EmployeeApi.GetEmployeeByIdAsync(Id);

        if (employee is null)
        {
            ErrorMessage = "Employee not found.";
        }
        else
        {
            Employee = new UpdateEmployeeModel
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Department = employee.Department,
                Salary = employee.Salary,
                RowVersion = employee.RowVersion
            };
        }

        await InvokeAsync(StateHasChanged);
    }

    protected async Task SaveAsync()
    {
        ErrorMessage = string.Empty;

        if (Employee is null)
        {
            return;
        }

        try
        {
            var success = await EmployeeApi.UpdateEmployeeAsync(Id, Employee);

            if (success)
            {
                Navigation.NavigateTo("/home");
                return;
            }

            ErrorMessage =
                "The employee was modified by another user. Please reload the employee and try again.";
        }
        catch (HttpRequestException)
        {
            ErrorMessage =
                "Unable to update the employee. Please try again.";
        }
    }

    protected void Cancel()
    {
        Navigation.NavigateTo("/home");
    }
}