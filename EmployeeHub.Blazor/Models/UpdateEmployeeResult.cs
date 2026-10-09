namespace EmployeeHub.Blazor.Models;

public enum UpdateEmployeeResult
{
    Success,
    Unauthenticated,
    ConcurrencyConflict,
    NotFound,
    Forbidden
}