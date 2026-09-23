using Serilog.Context;

namespace EmployeeHub.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.TraceIdentifier;
        var username = context.User.Identity?.Name ?? "Anonymous";

        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("Username", username))
        {
            _logger.LogInformation(
                "Request started: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await _next(context);

            _logger.LogInformation(
                "Request completed: {StatusCode}",
                context.Response.StatusCode);
        }
    }
}
