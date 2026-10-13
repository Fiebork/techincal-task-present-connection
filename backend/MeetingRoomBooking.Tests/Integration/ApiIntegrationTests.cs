using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MeetingRoomBooking.Api.DTOs.Reports;
using MeetingRoomBooking.Api.DTOs.Reservations;
using MeetingRoomBooking.Api.DTOs.Rooms;
using MeetingRoomBooking.Api.DTOs.Users;
using MeetingRoomBooking.Api.Exceptions;
using MeetingRoomBooking.Api.Models.Enums;

namespace MeetingRoomBooking.Tests.Integration;

public sealed class ApiIntegrationTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
    private ApiFactory factory = null!;
    private HttpClient client = null!;

    public Task InitializeAsync()
    {
        factory = new ApiFactory();
        client = factory.CreateClient();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        client.Dispose();
        await factory.DisposeAsync();
    }

    [Fact]
    public async Task Gets_seeded_rooms()
    {
        var response = await client.GetAsync("/api/rooms");

        var rooms = await ReadJsonAsync<List<RoomResponse>>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, rooms.Count);
        Assert.Contains(rooms, room => room.Id == 1 && room.IsActive);
    }

    [Fact]
    public async Task Filters_rooms_by_capacity_floor_and_equipment()
    {
        var response = await client.GetAsync(
            "/api/rooms?minimumCapacity=10&floor=1&equipment=Projector");

        var rooms = await ReadJsonAsync<List<RoomResponse>>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var room = Assert.Single(rooms);
        Assert.Equal(1, room.Id);
    }

    [Fact]
    public async Task Gets_seeded_users_with_active_reservation_counts()
    {
        var response = await client.GetAsync("/api/users");

        var users = await ReadJsonAsync<List<UserResponse>>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(users, user => user.Id == 1 && user.ActiveReservationCount == 1);
    }

    [Fact]
    public async Task Creates_reservation_and_returns_created_location()
    {
        var request = NewReservationRequest();

        var response = await client.PostAsJsonAsync("/api/reservations", request);
        var reservation = await ReadJsonAsync<ReservationResponse>(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal(ReservationStatus.Booked, reservation.Status);
        Assert.Contains($"/api/reservations/{reservation.Id}", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Rejects_room_overlap_with_conflict_problem_details()
    {
        var request = NewReservationRequest();
        var firstResponse = await client.PostAsJsonAsync("/api/reservations", request);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await client.PostAsJsonAsync("/api/reservations", request);
        var problem = await ReadJsonAsync<ProblemDetailsResponse>(secondResponse);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        Assert.Equal(ApiErrorCodes.RoomBookingOverlap, problem.Code);
        Assert.Equal("The room is already booked during the requested time.", problem.Detail);
    }

    [Fact]
    public async Task Cancelled_reservation_does_not_block_rebooking()
    {
        var request = NewReservationRequest();
        var firstResponse = await client.PostAsJsonAsync("/api/reservations", request);
        var first = await ReadJsonAsync<ReservationResponse>(firstResponse);

        var cancelResponse = await client.PostAsync(
            $"/api/reservations/{first.Id}/cancel",
            content: null);
        var secondResponse = await client.PostAsJsonAsync("/api/reservations", request);

        Assert.Equal(HttpStatusCode.NoContent, cancelResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
    }

    [Fact]
    public async Task Returns_not_found_for_missing_reservation()
    {
        var response = await client.GetAsync("/api/reservations/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Returns_bad_request_for_invalid_request_model()
    {
        var response = await client.PostAsJsonAsync(
            "/api/reservations",
            new
            {
                title = "",
                roomId = 0,
                organizerId = 0,
                startDate = DateTimeOffset.UtcNow.AddDays(1),
                endDate = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
                attendeeCount = 0
            });
        var problem = await ReadJsonAsync<ProblemDetailsResponse>(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(ApiErrorCodes.ValidationFailed, problem.Code);
    }

    [Fact]
    public async Task Gets_room_usage_report()
    {
        var response = await client.GetAsync("/api/reports/room-usage");
        var report = await ReadJsonAsync<List<RoomUsageResponse>>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, report.Count);
        Assert.Equal(1, report.Single(item => item.RoomId == 1).ReservationCount);
        Assert.Equal(0, report.Single(item => item.RoomId == 1).UsedReservationCount);
        Assert.Equal(0, report.Single(item => item.RoomId == 1).UsageRate);
    }

    private static CreateReservationRequest NewReservationRequest()
    {
        return new CreateReservationRequest
        {
            Title = "Integration test meeting",
            RoomId = 1,
            OrganizerId = 3,
            StartDate = DateTimeOffset.UtcNow.AddDays(20),
            EndDate = DateTimeOffset.UtcNow.AddDays(20).AddHours(1),
            AttendeeCount = 5
        };
    }

    private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        return value ?? throw new InvalidOperationException("The response body was empty.");
    }

    private sealed record ProblemDetailsResponse(
        string? Type,
        string? Title,
        int? Status,
        string? Detail,
        string? Instance,
        string? Code);

}
