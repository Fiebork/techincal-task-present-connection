using MeetingRoomBooking.Api.DTOs.Reservations;

namespace MeetingRoomBooking.Api.Services;

public interface IReservationService
{
    Task<IReadOnlyList<ReservationResponse>> GetAllAsync(
        ReservationFilterRequest? filter,
        CancellationToken cancellationToken);
    Task<ReservationResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ReservationResponse> CreateAsync(
        CreateReservationRequest request,
        CancellationToken cancellationToken);
    Task<ReservationResponse> UpdateAsync(
        int id,
        UpdateReservationRequest request,
        CancellationToken cancellationToken);
    Task CancelAsync(int id, CancellationToken cancellationToken);
    Task<ReservationResponse> EndEarlyAsync(int id, CancellationToken cancellationToken);
}
