using MeetingRoomBooking.Api.DTOs.Reports;
using MeetingRoomBooking.Api.Repositories;

namespace MeetingRoomBooking.Api.Services;

public sealed class ReportService(
    IRoomRepository roomRepository,
    IReservationRepository reservationRepository) : IReportService
{
    public async Task<IReadOnlyList<RoomUsageResponse>> GetRoomUsageAsync(
        CancellationToken cancellationToken)
    {
        var rooms = await roomRepository.GetAllAsync(cancellationToken: cancellationToken);
        var reservations = await reservationRepository.GetAllAsync(cancellationToken: cancellationToken);

        return rooms
            .Select(room =>
            {
                var roomReservations = reservations
                    .Where(reservation => reservation.RoomId == room.Id)
                    .Where(reservation => reservation.Status != Models.Enums.ReservationStatus.Cancelled)
                    .ToList();
                var usedReservationCount = roomReservations.Count(
                    reservation => reservation.Status == Models.Enums.ReservationStatus.Completed);

                return new RoomUsageResponse(
                    room.Id,
                    room.Name,
                    roomReservations.Count,
                    usedReservationCount,
                    roomReservations.Count == 0
                        ? 0
                        : (double)usedReservationCount / roomReservations.Count);
            })
            .OrderByDescending(report => report.ReservationCount)
            .ToList();
    }
}
