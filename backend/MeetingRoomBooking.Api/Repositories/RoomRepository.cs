using MeetingRoomBooking.Api.Data;
using MeetingRoomBooking.Api.DTOs.Rooms;
using MeetingRoomBooking.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Api.Repositories;

public class RoomRepository(MeetingRoomBookingDbContext dbContext) : IRoomRepository
{
    public async Task<IReadOnlyList<Room>> GetAllAsync(
        RoomFilterRequest? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Rooms.AsNoTracking().AsQueryable();

        if (filter?.MinimumCapacity is int minimumCapacity)
        {
            query = query.Where(room => room.Capacity >= minimumCapacity);
        }

        if (filter?.Floor is int floor)
        {
            query = query.Where(room => room.Floor == floor);
        }

        if (filter?.IsActive is bool isActive)
        {
            query = query.Where(room => room.IsActive == isActive);
        }

        var rooms = await query.ToListAsync(cancellationToken);

        if (filter?.Equipment is { Count: > 0 } equipment)
        {
            rooms = rooms
                .Where(room => equipment.All(requiredEquipment =>
                    room.Equipment.Contains(requiredEquipment)))
                .ToList();
        }

        return rooms;
    }

    public Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Rooms
            .Include(room => room.Reservations)
            .AsNoTracking()
            .SingleOrDefaultAsync(room => room.Id == id, cancellationToken);
    }

    public async Task<Room> AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        dbContext.Rooms.Add(room);
        await dbContext.SaveChangesAsync(cancellationToken);
        return room;
    }
}