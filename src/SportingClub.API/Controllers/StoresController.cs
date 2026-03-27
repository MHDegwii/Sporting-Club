using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/stores")]
public class StoresController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoresController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<Store>>> List([FromQuery] QueryOptions queryOptions)
    {
        var result = await _storeService.ListAsync(queryOptions);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<Store>> Get([FromRoute] Guid id)
    {
        var store = await _storeService.GetAsync(id);
        return store is null ? NotFound() : Ok(store);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<Store>> Create([FromBody] CreateStoreRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var createdBy))
            return BadRequest("Invalid user ID");

        var store = await _storeService.CreateAsync(request, createdBy);
        return CreatedAtAction(nameof(Get), new { id = store.Id, version = "1" }, store);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<Store>> Update([FromRoute] Guid id, [FromBody] UpdateStoreRequest request)
    {
        var store = await _storeService.UpdateAsync(id, request);
        return store is null ? NotFound() : Ok(store);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _storeService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
