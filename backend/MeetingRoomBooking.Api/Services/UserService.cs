using MeetingRoomBooking.Api.DTOs.Users;
using MeetingRoomBooking.Api.Models.Enums;
using MeetingRoomBooking.Api.Repositories;

namespace MeetingRoomBooking.Api.Services;

public sealed class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(Map).ToList();
    }

    public async Task<UserResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : Map(user);
    }

    private static UserResponse Map(Models.Entities.User user)
    {
        return new UserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Reservations.Count(reservation => reservation.Status == ReservationStatus.Booked));
    }
}
