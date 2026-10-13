using MeetingRoomBooking.Api.Configuration;
using MeetingRoomBooking.Api.Exceptions;
using MeetingRoomBooking.Api.Models.Entities;
using MeetingRoomBooking.Api.Models.Enums;
using MeetingRoomBooking.Api.Services;
using Microsoft.Extensions.Options;

namespace MeetingRoomBooking.Tests.Services;

public class ReservationValidatorTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 10, 12, 12, 0, 0, TimeSpan.Zero);

    private readonly ReservationValidator validator = new(
        Options.Create(new BookingRulesOptions
        {
            MaximumDurationDays = 7,
            BookingHorizonDays = 90
        }),
        new FixedTimeProvider(Now));

    [Fact]
    public void Allows_reservation_that_last_exactly_seven_days()
    {
        var reservation = CreateReservation(Now.AddDays(1), Now.AddDays(8));
        var result = Validate(reservation);

        AssertValid(result);
    }

    [Fact]
    public void Rejects_reservation_longer_than_seven_days()
    {
        var reservation = CreateReservation(Now.AddDays(1), Now.AddDays(8).AddTicks(1));
        var result = Validate(reservation);

        AssertError(result, ApiErrorCodes.MaximumDurationExceeded);
    }

    [Fact]
    public void Rejects_room_overlap()
    {
        var reservation = CreateReservation(Now.AddDays(1).AddHours(1), Now.AddDays(1).AddHours(2));
        var existing = CreateReservation(
            Now.AddDays(1),
            Now.AddDays(1).AddHours(2),
            id: 2,
            roomId: reservation.RoomId,
            organizerId: 99);
        var result = Validate(reservation, existing);

        AssertError(result, ApiErrorCodes.RoomBookingOverlap);
    }

    [Fact]
    public void Rejects_organizer_overlap_in_another_room()
    {
        var reservation = CreateReservation(Now.AddDays(1).AddHours(1), Now.AddDays(1).AddHours(2));
        var existing = CreateReservation(
            Now.AddDays(1),
            Now.AddDays(1).AddHours(2),
            id: 2,
            roomId: 99,
            organizerId: reservation.OrganizerId);
        var result = Validate(reservation, existing);

        AssertError(result, ApiErrorCodes.OrganizerBookingOverlap);
    }

    [Fact]
    public void Allows_reservations_when_one_ends_exactly_as_the_other_starts()
    {
        var reservation = CreateReservation(Now.AddDays(1).AddHours(2), Now.AddDays(1).AddHours(3));
        var existing = CreateReservation(
            Now.AddDays(1),
            reservation.StartDate,
            id: 2,
            roomId: reservation.RoomId,
            organizerId: reservation.OrganizerId);
        var result = Validate(reservation, existing);

        AssertValid(result);
    }

    [Fact]
    public void Allows_reservations_when_one_starts_exactly_as_the_other_ends()
    {
        var reservation = CreateReservation(Now.AddDays(1), Now.AddDays(1).AddHours(1));
        var existing = CreateReservation(
            reservation.EndDate,
            Now.AddDays(1).AddHours(2),
            id: 2,
            roomId: reservation.RoomId,
            organizerId: reservation.OrganizerId);
        var result = Validate(reservation, existing);

        AssertValid(result);
    }

    [Fact]
    public void Ignores_cancelled_reservation_for_room_overlap()
    {
        var reservation = CreateReservation(Now.AddDays(1).AddHours(1), Now.AddDays(1).AddHours(2));
        var existing = CreateReservation(
            Now.AddDays(1),
            Now.AddDays(1).AddHours(3),
            id: 2,
            roomId: reservation.RoomId,
            organizerId: reservation.OrganizerId);
        existing.Status = ReservationStatus.Cancelled;
        var result = Validate(reservation, existing);

        AssertValid(result);
    }

    [Fact]
    public void Ignores_the_reservation_being_updated()
    {
        var reservation = CreateReservation(Now.AddDays(1), Now.AddDays(1).AddHours(1), id: 10);
        var result = Validate(reservation, reservation);

        AssertValid(result);
    }

    [Fact]
    public void Rejects_start_at_now()
    {
        var reservation = CreateReservation(Now, Now.AddHours(1));
        var result = Validate(reservation);

        AssertError(result, ApiErrorCodes.InvalidTimeRange);
    }

    [Fact]
    public void Rejects_starting_beyond_booking_horizon()
    {
        var reservation = CreateReservation(Now.AddDays(90).AddTicks(1), Now.AddDays(90).AddHours(1));
        var result = Validate(reservation);

        AssertError(result, ApiErrorCodes.BookingHorizonExceeded);
    }

    [Fact]
    public void Allows_starting_exactly_at_booking_horizon()
    {
        var reservation = CreateReservation(Now.AddDays(90), Now.AddDays(90).AddHours(1));
        var result = Validate(reservation);

        AssertValid(result);
    }

    [Fact]
    public void Rejects_attendees_above_room_capacity()
    {
        var reservation = CreateReservation(Now.AddDays(1), Now.AddDays(1).AddHours(1), attendeeCount: 11);
        var result = Validate(reservation, roomCapacity: 10);

        AssertError(result, ApiErrorCodes.AttendeeCapacityExceeded);
    }

    [Fact]
    public void Allows_attendees_equal_to_room_capacity()
    {
        var reservation = CreateReservation(Now.AddDays(1), Now.AddDays(1).AddHours(1), attendeeCount: 10);
        var result = Validate(reservation, roomCapacity: 10);

        AssertValid(result);
    }

    [Fact]
    public void Rejects_inactive_room()
    {
        var reservation = CreateReservation(Now.AddDays(1), Now.AddDays(1).AddHours(1));
        var result = Validate(reservation, isRoomActive: false);

        AssertError(result, ApiErrorCodes.RoomInactive);
    }

    [Fact]
    public void Rejects_end_before_start()
    {
        var reservation = CreateReservation(Now.AddDays(1), Now.AddHours(1));
        var result = Validate(reservation);

        AssertError(result, ApiErrorCodes.InvalidTimeRange);
    }

    private ReservationValidationResult Validate(
        Reservation reservation,
        params Reservation[] existingReservations)
    {
        return validator.Validate(
            reservation,
            new Room
            {
                Id = reservation.RoomId,
                Name = "Test room",
                Capacity = 10,
                Floor = 1,
                IsActive = true
            },
            existingReservations);
    }

    private ReservationValidationResult Validate(
        Reservation reservation,
        int roomCapacity = 10,
        bool isRoomActive = true)
    {
        return validator.Validate(
            reservation,
            new Room
            {
                Id = reservation.RoomId,
                Name = "Test room",
                Capacity = roomCapacity,
                Floor = 1,
                IsActive = isRoomActive
            },
            []);
    }

    private static Reservation CreateReservation(
        DateTimeOffset start,
        DateTimeOffset end,
        int id = 1,
        int roomId = 1,
        int organizerId = 1,
        int attendeeCount = 1)
    {
        return new Reservation
        {
            Id = id,
            RoomId = roomId,
            OrganizerId = organizerId,
            Title = "Test reservation",
            StartDate = start,
            EndDate = end,
            AttendeeCount = attendeeCount,
            Status = ReservationStatus.Booked
        };
    }

    private static void AssertValid(ReservationValidationResult result)
    {
        Assert.True(result.IsValid, result.ErrorMessage);
    }

    private static void AssertError(ReservationValidationResult result, string errorCode)
    {
        Assert.False(result.IsValid);
        Assert.Equal(errorCode, result.ErrorCode);
        Assert.NotNull(result.ErrorMessage);
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return now;
        }
    }
}