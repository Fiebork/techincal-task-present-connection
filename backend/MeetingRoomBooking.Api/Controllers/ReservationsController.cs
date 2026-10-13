using MeetingRoomBooking.Api.DTOs.Reservations;
using MeetingRoomBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public sealed class ReservationsController(IReservationService reservationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReservationResponse>>> GetAll(
        [FromQuery] ReservationFilterRequest filter,
        CancellationToken cancellationToken)
    {
        return Ok(await reservationService.GetAllAsync(filter, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservationResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var reservation = await reservationService.GetByIdAsync(id, cancellationToken);
        return reservation is null ? NotFound() : Ok(reservation);
    }

    [HttpPost]
    public async Task<ActionResult<ReservationResponse>> Create(
        CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var reservation = await reservationService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReservationResponse>> Update(
        int id,
        UpdateReservationRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await reservationService.UpdateAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(
        int id,
        CancellationToken cancellationToken)
    {
        await reservationService.CancelAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/end")]
    public async Task<ActionResult<ReservationResponse>> EndEarly(
        int id,
        CancellationToken cancellationToken)
    {
        return Ok(await reservationService.EndEarlyAsync(id, cancellationToken));
    }
}
