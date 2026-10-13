namespace MeetingRoomBooking.Api.Exceptions;

public static class ApiErrorCodes
{
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string ResourceNotFound = "RESOURCE_NOT_FOUND";
    public const string RoomBookingOverlap = "ROOM_BOOKING_OVERLAP";
    public const string OrganizerBookingOverlap = "ORGANIZER_BOOKING_OVERLAP";
    public const string InvalidTimeRange = "INVALID_TIME_RANGE";
    public const string MaximumDurationExceeded = "MAXIMUM_DURATION_EXCEEDED";
    public const string BookingHorizonExceeded = "BOOKING_HORIZON_EXCEEDED";
    public const string AttendeeCapacityExceeded = "ATTENDEE_CAPACITY_EXCEEDED";
    public const string RoomInactive = "ROOM_INACTIVE";
    public const string ReservationNotBookable = "RESERVATION_NOT_BOOKABLE";
}