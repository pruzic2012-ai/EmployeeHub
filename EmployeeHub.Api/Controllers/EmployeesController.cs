using EmployeeHub.Application.DTOs;
using EmployeeHub.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using EmployeeHub.Application.Services.Idempotency;

namespace EmployeeHub.Api.Controllers;
/*
 * Authentication validates the user's identity. 
 * Authorization determines what the authenticated user is allowed to do. In EmployeeHub, I use JWT claims and authorization policies to control access to protected API operations
 * 
 * A normal authenticated user can access GET operations, but when they try to perform an administrative operation, authorization evaluates the AdminOnly policy and returns 403 because the user doesn't have the Admin role.
 * 
 */

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IIdempotencyService _idempotencyService;

    public EmployeesController(IEmployeeService empService, IIdempotencyService idempotencyService)
    {
        _employeeService = empService ?? throw new ArgumentNullException(nameof(empService));
        _idempotencyService = idempotencyService ?? throw new ArgumentNullException(nameof(idempotencyService));
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _employeeService.GetAllAsync();

        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEmployee(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateEmployee(
        CreateEmployeeDto dto,
        [FromHeader(Name ="Idempotency-Key")] string? idempotencyKey)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return BadRequest("Idempotency-Key header is required.");
        }

        // Check whether this request was already completed
        var existingResult =
            await _idempotencyService.GetResultAsync<EmployeeDto>(idempotencyKey);

        if (existingResult is not null)
        {
            return CreatedAtAction(
                nameof(GetEmployee),
                new { id = existingResult.Id },
                existingResult);
        }

        // Try to acquire the idempotency key
        var acquired = await _idempotencyService.TryAcquireAsync(
            idempotencyKey,
            TimeSpan.FromMinutes(5));

        if (!acquired)
        {
            return Conflict(
                "A request with this Idempotency-Key is currently being processed.");
        }

        try
        {
            var employee = await _employeeService.CreateAsync(dto);

            // Store the result for future retries
            await _idempotencyService.StoreResultAsync(
                idempotencyKey,
                employee,
                TimeSpan.FromMinutes(5));

            return CreatedAtAction(
                nameof(GetEmployee),
                new { id = employee.Id },
                employee);
        }
        catch
        {
            // Release the key so the client can safely retry
            await _idempotencyService.ReleaseAsync(idempotencyKey);

            throw;
        }

    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        UpdateEmployeeDto dto)
    {
        var result = await _employeeService.UpdateAsync(id, dto);
 
        return result switch
        {
            UpdateEmployeeResult.Updated => NoContent(),
            UpdateEmployeeResult.NotFound => NotFound(),
            UpdateEmployeeResult.ConcurrencyConflict => Conflict("The employee record has been modified by another user. Please refresh and try again."),
            _ => StatusCode(500, "An unexpected error occurred.")
        };

    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var deleted = await _employeeService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }


}
