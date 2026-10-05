using System.ComponentModel.DataAnnotations;

namespace EmployeeHub.Application.DTOs;

public class UpdateEmployeeDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Range(30000, 1000000)]
    public decimal Salary { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
