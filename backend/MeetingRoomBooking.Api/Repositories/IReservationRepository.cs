using MeetingRoomBooking.Api.DTOs.Reservations;
using MeetingRoomBooking.Api.Models.Entities;

namespace MeetingRoomBooking.Api.Repositories;

public interface IReservationRepository
{
    Task<IReadOnlyList<Reservation>> GetAllAsync(
        ReservationFilterRequest? filter = null,
        CancellationToken cancellationToken = default);
    Task<Reservation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
}