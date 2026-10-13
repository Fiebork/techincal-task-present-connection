using MeetingRoomBooking.Api.DTOs.Reservations;
using MeetingRoomBooking.Api.Exceptions;
using MeetingRoomBooking.Api.Models.Entities;
using MeetingRoomBooking.Api.Models.Enums;
using MeetingRoomBooking.Api.Repositories;

namespace MeetingRoomBooking.Api.Services;

public sealed class ReservationService(
    IReservationRepository reservationRepository,
    IRoomRepository roomRepository,
    IUserRepository userRepository,
    IReservationValidator reservationValidator,
    TimeProvider timeProvider) : IReservationService
{
    public async Task<IReadOnlyList<ReservationResponse>> GetAllAsync(
        ReservationFilterRequest? filter,
        CancellationToken cancellationToken)
    {
        var reservations = await reservationRepository.GetAllAsync(filter, cancellationToken);
        return reservations.Select(Map).ToList();
    }

    public async Task<ReservationResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.GetByIdAsync(id, cancellationToken);
        return reservation is null ? null : Map(reservation);
    }

    public async Task<ReservationResponse> CreateAsync(
        CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var room = await GetRoomAsync(request.RoomId, cancellationToken);
        await GetUserAsync(request.OrganizerId, cancellationToken);

        var reservation = new Reservation
        {
            RoomId = room.Id,
            OrganizerId = request.OrganizerId,
            Title = request.Title,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AttendeeCount = request.AttendeeCount,
            Status = ReservationStatus.Booked
        };

        await ValidateAsync(reservation, room, cancellationToken);
        var savedReservation = await reservationRepository.AddAsync(reservation, cancellationToken);
        return (await reservationRepository.GetByIdAsync(savedReservation.Id, cancellationToken)) is { } created
            ? Map(created)
            : throw new InvalidOperationException("The created reservation could not be loaded.");
    }

    public async Task<ReservationResponse> UpdateAsync(
        int id,
        UpdateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var reservation = await GetReservationAsync(id, cancellationToken);
        EnsureBookable(reservation);

        var room = await GetRoomAsync(request.RoomId, cancellationToken);
        await GetUserAsync(request.OrganizerId, cancellationToken);

        reservation.RoomId = room.Id;
        reservation.OrganizerId = request.OrganizerId;
        reservation.Title = request.Title;
        reservation.StartDate = request.StartDate;
        reservation.EndDate = request.EndDate;
        reservation.AttendeeCount = request.AttendeeCount;

        await ValidateAsync(reservation, room, cancellationToken);
        await reservationRepository.UpdateAsync(reservation, cancellationToken);
        var updatedReservation = await reservationRepository.GetByIdAsync(id, cancellationToken);
        return updatedReservation is null
            ? throw new InvalidOperationException("The updated reservation could not be loaded.")
            : Map(updatedReservation);
    }

    public async Task CancelAsync(int id, CancellationToken cancellationToken)
    {
        var reservation = await GetReservationAsync(id, cancellationToken);
        EnsureBookable(reservation);
        reservation.Status = ReservationStatus.Cancelled;
        await reservationRepository.UpdateAsync(reservation, cancellationToken);
    }

    public async Task<ReservationResponse> EndEarlyAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var reservation = await GetReservationAsync(id, cancellationToken);
        EnsureBookable(reservation);

        var now = timeProvider.GetUtcNow();
        if (now <= reservation.StartDate)
        {
            throw new ApiException(
                StatusCodes.Status400BadRequest,
                ApiErrorCodes.InvalidTimeRange,
                "A meeting can only be ended after it has started.");
        }

        reservation.EndDate = now < reservation.EndDate ? now : reservation.EndDate;
        reservation.Status = ReservationStatus.Completed;
        await reservationRepository.UpdateAsync(reservation, cancellationToken);
        return Map(reservation);
    }

    private async Task ValidateAsync(
        Reservation reservation,
        Room room,
        CancellationToken cancellationToken)
    {
        var existingReservations = await reservationRepository.GetAllAsync(
            cancellationToken: cancellationToken);
        var result = reservationValidator.Validate(reservation, room, existingReservations);

        if (!result.IsValid)
        {
            throw new ApiException(
                StatusCodes.Status409Conflict,
                result.ErrorCode!,
                result.ErrorMessage!);
        }
    }

    private async Task<Room> GetRoomAsync(int id, CancellationToken cancellationToken)
    {
        return await roomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw NotFound("Room", id);
    }

    private async Task GetUserAsync(int id, CancellationToken cancellationToken)
    {
        if (await userRepository.GetByIdAsync(id, cancellationToken) is null)
        {
            throw NotFound("User", id);
        }
    }

    private async Task<Reservation> GetReservationAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await reservationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw NotFound("Reservation", id);
    }

    private static void EnsureBookable(Reservation reservation)
    {
        if (reservation.Status != ReservationStatus.Booked)
        {
            throw new ApiException(
                StatusCodes.Status409Conflict,
                ApiErrorCodes.ReservationNotBookable,
                "Only booked reservations can be changed.");
        }
    }

    private static ApiException NotFound(string resource, int id)
    {
        return new ApiException(
            StatusCodes.Status404NotFound,
            ApiErrorCodes.ResourceNotFound,
            $"{resource} with ID {id} was not found.");
    }

    private static ReservationResponse Map(Reservation reservation)
    {
        return new ReservationResponse(
            reservation.Id,
            reservation.RoomId,
            reservation.Room.Name,
            reservation.OrganizerId,
            $"{reservation.Organizer.FirstName} {reservation.Organizer.LastName}",
            reservation.Title,
            reservation.StartDate,
            reservation.EndDate,
            reservation.AttendeeCount,
            reservation.Status);
    }
}
