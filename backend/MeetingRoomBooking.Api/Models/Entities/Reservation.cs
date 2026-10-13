using MeetingRoomBooking.Api.Models.Enums;
namespace MeetingRoomBooking.Api.Models.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public int OrganizerId { get; set; }
    public User Organizer { get; set; } = null!;
    public required string Title { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int AttendeeCount { get; set; }
    public ReservationStatus Status { get; set; }
}