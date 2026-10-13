using MeetingRoomBooking.Api.Models.Enums;
namespace MeetingRoomBooking.Api.Models.Entities;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public int Floor { get; set; }
    public List<RoomEquipment> Equipment { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public List<Reservation> Reservations { get; set; } = [];
}