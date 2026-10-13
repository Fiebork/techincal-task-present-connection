using MeetingRoomBooking.Api.DTOs.Rooms;
using MeetingRoomBooking.Api.Repositories;

namespace MeetingRoomBooking.Api.Services;

public sealed class RoomService(IRoomRepository roomRepository) : IRoomService
{
    public async Task<IReadOnlyList<RoomResponse>> GetAllAsync(
        RoomFilterRequest? filter,
        CancellationToken cancellationToken)
    {
        var rooms = await roomRepository.GetAllAsync(filter, cancellationToken);
        return rooms.Select(room => new RoomResponse(
            room.Id,
            room.Name,
            room.Capacity,
            room.Floor,
            room.Equipment,
            room.IsActive)).ToList();
    }

    public async Task<RoomResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var room = await roomRepository.GetByIdAsync(id, cancellationToken);
        return room is null
            ? null
            : new RoomResponse(
                room.Id,
                room.Name,
                room.Capacity,
                room.Floor,
                room.Equipment,
                room.IsActive);
    }
}
