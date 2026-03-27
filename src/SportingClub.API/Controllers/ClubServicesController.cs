using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/services")]
public class ClubServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ClubServicesController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<Service>>> List([FromQuery] QueryOptions queryOptions)
    {
        var result = await _serviceService.ListAsync(queryOptions);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<Service>> Get([FromRoute] Guid id)
    {
        var service = await _serviceService.GetAsync(id);
        return service is null ? NotFound() : Ok(service);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<Service>> Create([FromBody] CreateServiceRequest request)
    {
        var service = await _serviceService.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = service.Id, version = "1" }, service);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<Service>> Update([FromRoute] Guid id, [FromBody] UpdateServiceRequest request)
    {
        var service = await _serviceService.UpdateAsync(id, request);
        return service is null ? NotFound() : Ok(service);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _serviceService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
