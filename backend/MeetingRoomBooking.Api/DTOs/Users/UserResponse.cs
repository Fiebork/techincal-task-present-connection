namespace MeetingRoomBooking.Api.DTOs.Users;

public record UserResponse(
    int Id,
    string FirstName,
    string LastName,
    int ActiveReservationCount
);