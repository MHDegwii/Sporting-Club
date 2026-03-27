using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sports")]
public class SportsController : ControllerBase
{
    private readonly ISportService _sportService;

    public SportsController(ISportService sportService)
    {
        _sportService = sportService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<Sport>>> List([FromQuery] QueryOptions queryOptions)
    {
        var result = await _sportService.ListAsync(queryOptions);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<Sport>> Get([FromRoute] Guid id)
    {
        var sport = await _sportService.GetAsync(id);
        return sport is null ? NotFound() : Ok(sport);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<Sport>> Create([FromBody] CreateSportRequest request)
    {
        var sport = await _sportService.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = sport.Id, version = "1" }, sport);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<Sport>> Update([FromRoute] Guid id, [FromBody] UpdateSportRequest request)
    {
        var sport = await _sportService.UpdateAsync(id, request);
        return sport is null ? NotFound() : Ok(sport);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _sportService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
