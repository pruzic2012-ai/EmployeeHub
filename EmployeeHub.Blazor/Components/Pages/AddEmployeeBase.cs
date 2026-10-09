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
            var result = await EmployeeApi.CreateEmployeeAsync(Employee);

            switch (result)
            {
                case CreateEmployeeResult.Success:
                    Navigation.NavigateTo("/home");
                    return;

                case CreateEmployeeResult.Unauthenticated:
                    Navigation.NavigateTo("/login", replace: true);
                    return;

                case CreateEmployeeResult.Forbidden:
                    ErrorMessage =
                        "You don't have permission to create employees.";
                    break;

                case CreateEmployeeResult.BadRequest:
                    ErrorMessage =
                        "The API rejected the employee data. Please check your entries.";
                    break;

                case CreateEmployeeResult.Conflict:
                    ErrorMessage =
                        "The request conflicts with existing data. Please review your entries and try again.";
                    break;

                default:
                    ErrorMessage =
                        "Unable to create the employee. Please try again.";
                    break;
            }
        }
        catch (HttpRequestException)
        {
            ErrorMessage =
                "Unable to connect to the API. Please check your connection and try again.";
        }
        catch (Exception)
        {
            ErrorMessage =
                "An unexpected error occurred while creating the employee. Please try again.";
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