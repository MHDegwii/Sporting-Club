using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SportingClub.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/operations")]
[Authorize(Policy = "AdminOnly")]
public class OperationsController : ControllerBase
{
    [HttpGet("ready")]
    [AllowAnonymous]
    public IActionResult Readiness() => Ok(new { status = "ready", checkedAtUtc = DateTime.UtcNow });

    [HttpGet("live")]
    [AllowAnonymous]
    public IActionResult Liveness() => Ok(new { status = "alive", checkedAtUtc = DateTime.UtcNow });
}
