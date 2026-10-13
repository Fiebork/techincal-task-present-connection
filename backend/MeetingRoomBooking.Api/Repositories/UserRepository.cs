using MeetingRoomBooking.Api.Data;
using MeetingRoomBooking.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Api.Repositories;

public class UserRepository(MeetingRoomBookingDbContext dbContext) : IUserRepository
{
    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Include(user => user.Reservations)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .Include(user => user.Reservations)
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }
}