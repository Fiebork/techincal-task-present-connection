using MeetingRoomBooking.Api.Models.Enums;
namespace MeetingRoomBooking.Api.DTOs.Reservations;

public record ReservationResponse(
    int Id,
    int RoomId,
    string RoomName,
    int OrganizerId,
    string OrganizerName,
    string Title,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    int AttendeeCount,
    ReservationStatus Status
);