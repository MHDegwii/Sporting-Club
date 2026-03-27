using System.Security.Claims;

namespace SportingClub.API;

public sealed class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var user = context.User.FindFirstValue(ClaimTypes.Email) ?? "anonymous";
        _logger.LogInformation("AUDIT {Method} {Path} by {User}", context.Request.Method, context.Request.Path, user);
        await _next(context);
    }
}
