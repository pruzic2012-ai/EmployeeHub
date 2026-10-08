using EmployeeHub.Blazor.Models;
using EmployeeHub.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace EmployeeHub.Blazor.Components.Pages;

public class AddEmployeeBase : ComponentBase
{
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    protected CreateEmployeeModel Employee { get; set; } = new();

    [Inject]
    protected EmployeeService EmployeeApi { get; set; } = default!;
       
    protected string ErrorMessage { get; set; } = string.Empty;

    protected bool IsSaving { get; set; }

    protected async Task AddEmployeeAsync()
    {
        ErrorMessage = string.Empty;
        IsSaving = true;

        try
        {
            var createdEmployee =
                await EmployeeApi.CreateEmployeeAsync(Employee);

            if (createdEmployee is null)
            {
                return;
            }

            Navigation.NavigateTo("/home");
        }
        catch (HttpRequestException)
        {
            ErrorMessage =
                "Unable to create the employee. Please try again.";
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