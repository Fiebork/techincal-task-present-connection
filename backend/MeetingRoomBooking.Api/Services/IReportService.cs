using MeetingRoomBooking.Api.DTOs.Reports;

namespace MeetingRoomBooking.Api.Services;

public interface IReportService
{
    Task<IReadOnlyList<RoomUsageResponse>> GetRoomUsageAsync(
        CancellationToken cancellationToken);
}
