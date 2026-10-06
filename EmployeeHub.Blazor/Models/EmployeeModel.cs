namespace EmployeeHub.Blazor.Models;

public class EmployeeModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime CreatedDate { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
