using MeetingRoomBooking.Api.DTOs.Rooms;

namespace MeetingRoomBooking.Api.Services;

public interface IRoomService
{
    Task<IReadOnlyList<RoomResponse>> GetAllAsync(
        RoomFilterRequest? filter,
        CancellationToken cancellationToken);
    Task<RoomResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
