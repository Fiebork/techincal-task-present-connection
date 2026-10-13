using MeetingRoomBooking.Api.DTOs.Rooms;
using MeetingRoomBooking.Api.Models.Entities;

namespace MeetingRoomBooking.Api.Repositories;

public interface IRoomRepository
{
    Task<IReadOnlyList<Room>> GetAllAsync(
        RoomFilterRequest? filter = null,
        CancellationToken cancellationToken = default);
    Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Room> AddAsync(Room room, CancellationToken cancellationToken = default);
}