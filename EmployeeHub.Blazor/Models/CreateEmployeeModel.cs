
using System.ComponentModel.DataAnnotations;

namespace EmployeeHub.Blazor.Models;

public class CreateEmployeeModel
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    [Required]
    [Range(30000, double.MaxValue,
        ErrorMessage = "Salary must be at least $30,000.")]
    public decimal Salary { get; set; }
}
