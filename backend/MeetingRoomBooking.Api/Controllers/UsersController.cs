using MeetingRoomBooking.Api.DTOs.Users;
using MeetingRoomBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await userService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }
}
