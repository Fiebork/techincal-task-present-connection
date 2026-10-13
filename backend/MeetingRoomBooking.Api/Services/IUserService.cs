using MeetingRoomBooking.Api.DTOs.Users;

namespace MeetingRoomBooking.Api.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
