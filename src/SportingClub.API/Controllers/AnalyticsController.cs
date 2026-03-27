using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportingClub.Application;

namespace SportingClub.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/analytics")]
[Authorize(Policy = "ManagerOrAdmin")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("dashboard-summary")]
    public ActionResult<IDictionary<string, object>> DashboardSummary() =>
        Ok(_analyticsService.GetDashboardSummary());
}
