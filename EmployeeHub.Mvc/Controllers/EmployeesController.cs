using System.Text.Json;
using EmployeeHub.Mvc.Models;
using EmployeeHub.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHub.Mvc.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly EmployeeApiClient _employeeApiClient;

    public EmployeesController(EmployeeApiClient employeeApiClient)
    {
        _employeeApiClient = employeeApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var token = User.FindFirst("EmployeeHubApiToken")?.Value;

        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var employees = await _employeeApiClient.GetEmployeesAsync(token, cancellationToken);

            return View(employees);
        }
        catch (HttpRequestException)
        {
            ViewBag.ApiError =
                "Unable to retrieve employees. Please check that " +
                "EmployeeHub API is running and try again.";

            return View(new List<EmployeeViewModel>());
        }
        catch (JsonException)
        {
            ViewBag.ApiError =
                "The API returned employee data in an unexpected format.";

            return View(new List<EmployeeViewModel>());
        }
    }
}