using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/offers")]
public class OffersController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OffersController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<Offer>>> List([FromQuery] QueryOptions queryOptions)
    {
        var result = await _offerService.ListAsync(queryOptions);
        return Ok(result);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<Offer>>> ListActive([FromQuery] QueryOptions queryOptions)
    {
        var result = await _offerService.ListActiveOffersAsync(queryOptions);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<Offer>> Get([FromRoute] Guid id)
    {
        var offer = await _offerService.GetAsync(id);
        return offer is null ? NotFound() : Ok(offer);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<Offer>> Create([FromBody] CreateOfferRequest request)
    {
        var offer = await _offerService.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = offer.Id, version = "1" }, offer);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<Offer>> Update([FromRoute] Guid id, [FromBody] UpdateOfferRequest request)
    {
        var offer = await _offerService.UpdateAsync(id, request);
        return offer is null ? NotFound() : Ok(offer);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _offerService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
