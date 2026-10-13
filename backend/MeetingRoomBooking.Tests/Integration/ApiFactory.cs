using MeetingRoomBooking.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MeetingRoomBooking.Tests.Integration;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string databaseName = $"IntegrationTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<MeetingRoomBookingDbContext>();
            services.RemoveAll<DbContextOptions<MeetingRoomBookingDbContext>>();
            services.AddDbContext<MeetingRoomBookingDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MeetingRoomBookingDbContext>();
        dbContext.Database.EnsureDeleted();
        DatabaseSeeder.SeedAsync(dbContext).GetAwaiter().GetResult();
        return host;
    }
}
