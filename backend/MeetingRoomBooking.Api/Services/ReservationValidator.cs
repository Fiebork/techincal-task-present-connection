using MeetingRoomBooking.Api.Configuration;
using MeetingRoomBooking.Api.Exceptions;
using MeetingRoomBooking.Api.Models.Entities;
using Microsoft.Extensions.Options;

namespace MeetingRoomBooking.Api.Services;

public class ReservationValidator(
    IOptions<BookingRulesOptions> options,
    TimeProvider timeProvider) : IReservationValidator
{
    private readonly BookingRulesOptions rules = options.Value;

    public ReservationValidationResult Validate(
        Reservation reservation,
        Room room,
        IReadOnlyCollection<Reservation> existingReservations)
    {
        var now = timeProvider.GetUtcNow();

        if (reservation.EndDate <= reservation.StartDate)
        {
            return ReservationValidationResult.Invalid(
                ApiErrorCodes.InvalidTimeRange,
                "The reservation end date must be later than the start date.");
        }

        if (reservation.StartDate <= now)
        {
            return ReservationValidationResult.Invalid(
                ApiErrorCodes.InvalidTimeRange,
                "The reservation must start in the future.");
        }

        if (reservation.EndDate - reservation.StartDate > TimeSpan.FromDays(rules.MaximumDurationDays))
        {
            return ReservationValidationResult.Invalid(
                ApiErrorCodes.MaximumDurationExceeded,
                $"A reservation cannot last longer than {rules.MaximumDurationDays} days.");
        }

        if (reservation.StartDate > now.AddDays(rules.BookingHorizonDays))
        {
            return ReservationValidationResult.Invalid(
                ApiErrorCodes.BookingHorizonExceeded,
                $"A reservation cannot start more than {rules.BookingHorizonDays} days in the future.");
        }

        if (reservation.AttendeeCount > room.Capacity)
        {
            return ReservationValidationResult.Invalid(
                ApiErrorCodes.AttendeeCapacityExceeded,
                "The number of attendees cannot exceed the room capacity.");
        }

        if (!room.IsActive)
        {
            return ReservationValidationResult.Invalid(
                ApiErrorCodes.RoomInactive,
                "Inactive rooms cannot be booked.");
        }

        var overlappingReservations = existingReservations
            .Where(existing => existing.Id != reservation.Id)
            .Where(existing => existing.Status == Models.Enums.ReservationStatus.Booked)
            .Where(existing => Overlaps(existing.StartDate, existing.EndDate, reservation.StartDate, reservation.EndDate));

        if (overlappingReservations.Any(existing => existing.RoomId == reservation.RoomId))
        {
            return ReservationValidationResult.Invalid(
                ApiErrorCodes.RoomBookingOverlap,
                "The room is already booked during the requested time.");
        }

        if (overlappingReservations.Any(existing => existing.OrganizerId == reservation.OrganizerId))
        {
            return ReservationValidationResult.Invalid(
                ApiErrorCodes.OrganizerBookingOverlap,
                "The organizer already has a meeting during the requested time.");
        }

        return ReservationValidationResult.Success;
    }

    private static bool Overlaps(
        DateTimeOffset firstStart,
        DateTimeOffset firstEnd,
        DateTimeOffset secondStart,
        DateTimeOffset secondEnd)
    {
        return firstStart < secondEnd && secondStart < firstEnd;
    }
}
