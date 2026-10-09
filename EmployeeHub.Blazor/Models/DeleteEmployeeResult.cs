namespace EmployeeHub.Blazor.Models;

public enum DeleteEmployeeResult
{
    Success,
    Unauthenticated,
    NotFound,
    Forbidden
}