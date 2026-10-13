using System.ComponentModel.DataAnnotations;
namespace MeetingRoomBooking.Api.Configuration;

public class BookingRulesOptions
{
    public const string SectionName = "BookingRules";

    [Range(1, 365)]
    public int MaximumDurationDays { get; init; } = 7;

    [Range(1, 365)]
    public int BookingHorizonDays { get; init; } = 90;
}