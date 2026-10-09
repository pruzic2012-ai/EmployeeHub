
using System.ComponentModel.DataAnnotations;

namespace EmployeeHub.Blazor.Models;

public class UpdateEmployeeModel
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    public string Department { get; set; } = string.Empty;

    [Range(30000, double.MaxValue,
        ErrorMessage = "Salary must be at least $30,000.")]
    public decimal Salary { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
