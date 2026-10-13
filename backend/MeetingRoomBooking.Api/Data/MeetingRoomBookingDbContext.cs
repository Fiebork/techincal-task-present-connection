using MeetingRoomBooking.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Api.Data;

public class MeetingRoomBookingDbContext(DbContextOptions<MeetingRoomBookingDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(user => user.Reservations)
            .WithOne(reservation => reservation.Organizer)
            .HasForeignKey(reservation => reservation.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasMany(room => room.Reservations)
            .WithOne(reservation => reservation.Room)
            .HasForeignKey(reservation => reservation.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
