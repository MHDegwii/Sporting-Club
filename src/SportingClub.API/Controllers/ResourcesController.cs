using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/{resource}")]
[Authorize(Policy = "StaffOrManagerOrAdmin")]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ResourceItem>>> List(
        [FromRoute] string resource,
        [FromQuery] QueryOptions queryOptions) =>
        Ok(await _resourceService.ListAsync(resource, queryOptions));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResourceItem>> Get([FromRoute] string resource, [FromRoute] Guid id)
    {
        var item = await _resourceService.GetAsync(resource, id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<ResourceItem>> Create([FromRoute] string resource, [FromBody] ResourceItem request)
    {
        var created = await _resourceService.CreateAsync(resource, request);
        return CreatedAtAction(nameof(Get), new { resource, id = created.Id, version = "1" }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<ResourceItem>> Update([FromRoute] string resource, [FromRoute] Guid id, [FromBody] ResourceItem request)
    {
        var updated = await _resourceService.UpdateAsync(resource, id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete([FromRoute] string resource, [FromRoute] Guid id) =>
        await _resourceService.DeleteAsync(resource, id) ? NoContent() : NotFound();
}
