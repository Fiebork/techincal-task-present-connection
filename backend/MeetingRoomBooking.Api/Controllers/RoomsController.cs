using MeetingRoomBooking.Api.DTOs.Rooms;
using MeetingRoomBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.Api.Controllers;

[ApiController]
[Route("api/rooms")]
public sealed class RoomsController(IRoomService roomService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> GetAll(
        [FromQuery] RoomFilterRequest filter,
        CancellationToken cancellationToken)
    {
        return Ok(await roomService.GetAllAsync(filter, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var room = await roomService.GetByIdAsync(id, cancellationToken);
        return room is null ? NotFound() : Ok(room);
    }
}
