using MeetingRoomBooking.Api.Models.Enums;
namespace MeetingRoomBooking.Api.DTOs.Rooms;

public record RoomResponse(
    int Id,
    string Name,
    int Capacity,
    int Floor,
    IReadOnlyCollection<RoomEquipment> Equipment,
    bool IsActive
);