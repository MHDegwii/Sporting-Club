using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<Subscription>>> List([FromQuery] QueryOptions queryOptions)
    {
        var result = await _subscriptionService.ListAsync(queryOptions);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<Subscription>> Get([FromRoute] Guid id)
    {
        var subscription = await _subscriptionService.GetAsync(id);
        return subscription is null ? NotFound() : Ok(subscription);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Subscription>> Create([FromBody] CreateSubscriptionRequest request)
    {
        var subscription = await _subscriptionService.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = subscription.Id, version = "1" }, subscription);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Subscription>> Update([FromRoute] Guid id, [FromBody] UpdateSubscriptionRequest request)
    {
        var subscription = await _subscriptionService.UpdateAsync(id, request);
        return subscription is null ? NotFound() : Ok(subscription);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _subscriptionService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
