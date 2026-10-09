
namespace EmployeeHub.Blazor.Models;

public enum CreateEmployeeResult
{
    Success,
    Unauthenticated,
    Forbidden,
    BadRequest,
    Conflict,
    Failed
}
