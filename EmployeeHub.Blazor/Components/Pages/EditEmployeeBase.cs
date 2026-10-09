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

    protected bool IsSaving { get; set; }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        try
        {
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
        }
        catch (HttpRequestException)
        {
            ErrorMessage =
                "Unable to load the employee. Please check your connection and try again.";
        }
        catch (Exception)
        {
            ErrorMessage =
                "An unexpected error occurred while loading the employee.";
        }

        StateHasChanged();
    }


    protected async Task SaveAsync()
    {
        if (IsSaving || Employee is null)
        {
            return;
        }

        ErrorMessage = string.Empty;
        IsSaving = true;

        try
        {
            var result = await EmployeeApi.UpdateEmployeeAsync(
                Id, Employee);

            switch (result)
            {
                case UpdateEmployeeResult.Success:
                    Navigation.NavigateTo("/home");
                    return;

                case UpdateEmployeeResult.Unauthenticated:
                    Navigation.NavigateTo("/login", replace: true);
                    return;

                case UpdateEmployeeResult.ConcurrencyConflict:
                    ErrorMessage =
                        "The employee was modified by another user. Please reload the employee and try again.";
                    break;

                case UpdateEmployeeResult.NotFound:
                    ErrorMessage =
                        "This employee no longer exists. Return to the employee list.";
                    break;

                case UpdateEmployeeResult.Forbidden:
                    ErrorMessage =
                        "You don't have permission to update this employee.";
                    break;
            }
        }
        catch (HttpRequestException)
        {
            ErrorMessage =
                "Unable to update the employee. Please try again.";
        }
        finally
        {
            IsSaving = false;
        }
    }


    protected void Cancel()
    {
        Navigation.NavigateTo("/home");
    }
}