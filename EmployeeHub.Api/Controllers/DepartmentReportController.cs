
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EmployeeHub.Api.Controllers;

[ApiController]
[Route("api/department-report")]
[Authorize]
public class DepartmentReportController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DepartmentReportController> _logger;

    public DepartmentReportController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<DepartmentReportController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Generate(
        [FromBody] DepartmentReportRequest request,
        CancellationToken cancellationToken)
    {
        var allowedDepartments = new[] { "IT", "Finance", "Accounting" };

        if (string.IsNullOrWhiteSpace(request.Department) ||
            !allowedDepartments.Contains(
                request.Department,
                StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Select IT, Finance, or Accounting."
            });
        }

        var webhookUrl = _configuration[
            "N8n:DepartmentReportWebhookUrl"];
        var headerName = _configuration["N8n:HeaderName"];
        var headerValue = _configuration["N8n:HeaderValue"];

        if (string.IsNullOrWhiteSpace(webhookUrl) ||
            string.IsNullOrWhiteSpace(headerName) ||
            string.IsNullOrWhiteSpace(headerValue))
        {
            _logger.LogError("Department report configuration is incomplete.");
            return StatusCode(500, new
            {
                message = "The department report service is not configured."
            });
        }

        try
        {
            using var client = _httpClientFactory.CreateClient();
            using var content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    department = request.Department
                }),
                Encoding.UTF8,
                "application/json");

            using var message = new HttpRequestMessage(
                HttpMethod.Post, webhookUrl)
            {
                Content = content
            };

            message.Headers.TryAddWithoutValidation(
                headerName, headerValue);

            using var response = await client.SendAsync(
                message, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "n8n department report returned HTTP {StatusCode}.",
                    (int)response.StatusCode);

                return StatusCode(502, new
                {
                    message = "The department report could not be generated."
                });
            }

            var html = await response.Content.ReadAsStringAsync(
                cancellationToken);

            return Content(html, "text/html; charset=utf-8");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex, "Unable to reach the department report service.");

            return StatusCode(502, new
            {
                message = "The department report service is unavailable."
            });
        }
        catch (TaskCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(504, new
            {
                message = "The department report took too long to generate."
            });
        }
    }
}

public sealed class DepartmentReportRequest
{
    public string Department { get; set; } = string.Empty;
}
