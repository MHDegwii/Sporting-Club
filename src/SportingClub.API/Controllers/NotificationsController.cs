using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportingClub.Application;

namespace SportingClub.API.Controllers;

public sealed class NotificationRequest
{
    public string? UserEmail { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[Authorize(Policy = "ManagerOrAdmin")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] NotificationRequest request)
    {
        await _notificationService.BroadcastAsync(request.Title, request.Message);
        return Accepted();
    }

    [HttpPost("targeted")]
    public async Task<IActionResult> Targeted([FromBody] NotificationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserEmail))
        {
            return BadRequest("userEmail is required.");
        }

        await _notificationService.SendToUserAsync(request.UserEmail, request.Title, request.Message);
        return Accepted();
    }
}
