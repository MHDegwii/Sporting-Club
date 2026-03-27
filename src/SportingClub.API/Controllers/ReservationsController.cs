using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    [Authorize(Policy = "StaffOrManagerOrAdmin")]
    public async Task<ActionResult<PagedResult<Reservation>>> List([FromQuery] QueryOptions queryOptions)
    {
        var result = await _reservationService.ListAsync(queryOptions);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<Reservation>> Get([FromRoute] Guid id)
    {
        var reservation = await _reservationService.GetAsync(id);
        return reservation is null ? NotFound() : Ok(reservation);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Reservation>> Create([FromBody] CreateReservationRequest request)
    {
        try
        {
            var reservation = await _reservationService.CreateAsync(request);
            return CreatedAtAction(nameof(Get), new { id = reservation.Id, version = "1" }, reservation);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<Reservation>> Update([FromRoute] Guid id, [FromBody] UpdateReservationRequest request)
    {
        try
        {
            var reservation = await _reservationService.UpdateAsync(id, request);
            return reservation is null ? NotFound() : Ok(reservation);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Cancel([FromRoute] Guid id)
    {
        var cancelled = await _reservationService.CancelAsync(id);
        return cancelled ? NoContent() : NotFound();
    }

    [HttpGet("sport/{sportId:guid}/available-slots")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<string>>> GetAvailableSlots(
        [FromRoute] Guid sportId,
        [FromQuery] DateTime date)
    {
        var slots = await _reservationService.GetAvailableSlotsAsync(sportId, date);
        return Ok(slots);
    }

    [HttpGet("sport/{sportId:guid}/check-conflict")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> CheckConflict(
        [FromRoute] Guid sportId,
        [FromQuery] DateTime date,
        [FromQuery] string timeSlot)
    {
        var hasConflict = await _reservationService.CheckConflictAsync(sportId, date, timeSlot);
        return Ok(new { hasConflict });
    }
}
