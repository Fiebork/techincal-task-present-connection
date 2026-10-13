using MeetingRoomBooking.Api.Models.Entities;
using MeetingRoomBooking.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        MeetingRoomBookingDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var users = CreateUsers();
        var rooms = CreateRooms();
        var reservations = CreateReservations(users, rooms);

        await dbContext.Users.AddRangeAsync(users, cancellationToken);
        await dbContext.Rooms.AddRangeAsync(rooms, cancellationToken);
        await dbContext.Reservations.AddRangeAsync(reservations, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static List<User> CreateUsers()
    {
        return
        [
            new User { Id = 1, FirstName = "Radvilas", LastName = "Ulberkis" },
            new User { Id = 2, FirstName = "Monika", LastName = "Kvainauskiene" },
            new User { Id = 3, FirstName = "Danute", LastName = "Minkiene" }
        ];
    }

    private static List<Room> CreateRooms()
    {
        return
        [
            new Room
            {
                Id = 1,
                Name = "ER1",
                Capacity = 12,
                Floor = 1,
                Equipment = [RoomEquipment.Projector, RoomEquipment.Whiteboard],
                IsActive = true
            },
            new Room
            {
                Id = 2,
                Name = "KF5",
                Capacity = 6,
                Floor = 2,
                Equipment = [RoomEquipment.TvScreen, RoomEquipment.VideoConferencing],
                IsActive = true
            },
            new Room
            {
                Id = 3,
                Name = "BN4",
                Capacity = 20,
                Floor = 2,
                Equipment = [RoomEquipment.Projector, RoomEquipment.TvScreen, RoomEquipment.Whiteboard],
                IsActive = false
            }
        ];
    }

    private static List<Reservation> CreateReservations(
        IReadOnlyList<User> users,
        IReadOnlyList<Room> rooms)
    {
        var tomorrow = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(1), TimeSpan.Zero);

        return
        [
            new Reservation
            {
                Id = 1,
                RoomId = rooms[0].Id,
                OrganizerId = users[0].Id,
                Title = "Product planning",
                StartDate = tomorrow.AddHours(9),
                EndDate = tomorrow.AddHours(10),
                AttendeeCount = 8,
                Status = ReservationStatus.Booked
            },
            new Reservation
            {
                Id = 2,
                RoomId = rooms[1].Id,
                OrganizerId = users[1].Id,
                Title = "Design review",
                StartDate = tomorrow.AddDays(1).AddHours(13),
                EndDate = tomorrow.AddDays(1).AddHours(14),
                AttendeeCount = 5,
                Status = ReservationStatus.Booked
            }
        ];
    }
}
