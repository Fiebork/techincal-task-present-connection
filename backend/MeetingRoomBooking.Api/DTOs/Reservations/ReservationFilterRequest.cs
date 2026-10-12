using MeetingRoomBooking.Api.Models.Enums;
namespace MeetingRoomBooking.Api.DTOs.Reservations;

public class ReservationFilterRequest
{
    public int? RoomId { get; init; }
    public int? OrganizerId { get; init; }
    public ReservationStatus? Status { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}