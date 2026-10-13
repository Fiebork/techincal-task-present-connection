namespace MeetingRoomBooking.Api.DTOs.Reports;

public record RoomUsageResponse(
    int RoomId,
    string RoomName,
    int ReservationCount,
    int UsedReservationCount,
    double UsageRate
);