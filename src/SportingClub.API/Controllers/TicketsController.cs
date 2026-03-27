using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tickets")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<Ticket>>> List([FromQuery] QueryOptions queryOptions)
    {
        var result = await _ticketService.ListAsync(queryOptions);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<Ticket>> Get([FromRoute] Guid id)
    {
        var ticket = await _ticketService.GetAsync(id);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<Ticket>> Create([FromBody] CreateTicketRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var createdBy))
            return BadRequest("Invalid user ID");

        var ticket = await _ticketService.CreateAsync(request, createdBy);
        return CreatedAtAction(nameof(Get), new { id = ticket.Id, version = "1" }, ticket);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<Ticket>> Update([FromRoute] Guid id, [FromBody] UpdateTicketRequest request)
    {
        var ticket = await _ticketService.UpdateAsync(id, request);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _ticketService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/book")]
    [Authorize]
    public async Task<ActionResult<TicketBooking>> BookTicket([FromRoute] Guid id, [FromBody] int quantity)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var userGuid))
            return BadRequest("Invalid user ID");

        try
        {
            var booking = await _ticketService.BookTicketAsync(new BookTicketRequest(id, quantity), userGuid);
            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my-bookings")]
    [Authorize]
    public async Task<ActionResult<PagedResult<TicketBooking>>> MyBookings([FromQuery] QueryOptions queryOptions)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var userGuid))
            return BadRequest("Invalid user ID");

        var result = await _ticketService.ListUserBookingsAsync(userGuid, queryOptions);
        return Ok(result);
    }
}
