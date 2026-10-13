using MeetingRoomBooking.Api.Models.Enums;
namespace MeetingRoomBooking.Api.DTOs.Rooms;

public class RoomFilterRequest
{
    public int? MinimumCapacity { get; init; }
    public IReadOnlyCollection<RoomEquipment>? Equipment { get; init; }
    public int? Floor { get; init; }
    public bool? IsActive { get; init; }
}