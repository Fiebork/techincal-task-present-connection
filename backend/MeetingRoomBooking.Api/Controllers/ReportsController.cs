using MeetingRoomBooking.Api.DTOs.Reports;
using MeetingRoomBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.Api.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("room-usage")]
    public async Task<ActionResult<IReadOnlyList<RoomUsageResponse>>> GetRoomUsage(
        CancellationToken cancellationToken)
    {
        return Ok(await reportService.GetRoomUsageAsync(cancellationToken));
    }
}
