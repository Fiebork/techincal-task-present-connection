using System.ComponentModel.DataAnnotations;
namespace MeetingRoomBooking.Api.DTOs.Reservations;

public class UpdateReservationRequest
{
    [Required]
    [StringLength(150, MinimumLength = 1)]
    public required string Title { get; init; }

    [Range(1, int.MaxValue)]
    public int RoomId { get; init; }

    [Range(1, int.MaxValue)]
    public int OrganizerId { get; init; }

    public DateTimeOffset StartDate { get; init; }

    public DateTimeOffset EndDate { get; init; }

    [Range(1, int.MaxValue)]
    public int AttendeeCount { get; init; }
}