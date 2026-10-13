using MeetingRoomBooking.Api.Data;
using MeetingRoomBooking.Api.DTOs.Reservations;
using MeetingRoomBooking.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Api.Repositories;

public class ReservationRepository(MeetingRoomBookingDbContext dbContext) : IReservationRepository
{
    public async Task<IReadOnlyList<Reservation>> GetAllAsync(
        ReservationFilterRequest? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Reservations
            .Include(reservation => reservation.Room)
            .Include(reservation => reservation.Organizer)
            .AsNoTracking()
            .AsQueryable();

        if (filter?.RoomId is int roomId)
        {
            query = query.Where(reservation => reservation.RoomId == roomId);
        }

        if (filter?.OrganizerId is int organizerId)
        {
            query = query.Where(reservation => reservation.OrganizerId == organizerId);
        }

        if (filter?.Status is { } status)
        {
            query = query.Where(reservation => reservation.Status == status);
        }

        if (filter?.From is DateTimeOffset from)
        {
            query = query.Where(reservation => reservation.EndDate >= from);
        }

        if (filter?.To is DateTimeOffset to)
        {
            query = query.Where(reservation => reservation.StartDate <= to);
        }

        return await query
            .OrderBy(reservation => reservation.StartDate)
            .ToListAsync(cancellationToken);
    }

    public Task<Reservation?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Reservations
            .Include(reservation => reservation.Room)
            .Include(reservation => reservation.Organizer)
            .AsNoTracking()
            .SingleOrDefaultAsync(reservation => reservation.Id == id, cancellationToken);
    }

    public async Task<Reservation> AddAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default)
    {
        dbContext.Reservations.Add(reservation);
        await dbContext.SaveChangesAsync(cancellationToken);
        return reservation;
    }

    public async Task UpdateAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default)
    {
        dbContext.Reservations.Update(reservation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
