using MeetingRoomBooking.Api.Models.Entities;

namespace MeetingRoomBooking.Api.Services;

public interface IReservationValidator
{
    ReservationValidationResult Validate(
        Reservation reservation,
        Room room,
        IReadOnlyCollection<Reservation> existingReservations);
}

public sealed record ReservationValidationResult(string? ErrorCode, string? ErrorMessage)
{
    public bool IsValid => ErrorCode is null;
    public static ReservationValidationResult Success { get; } = new(null, null);
    public static ReservationValidationResult Invalid(string errorCode, string errorMessage)
    {
        return new ReservationValidationResult(errorCode, errorMessage);
    }
}
