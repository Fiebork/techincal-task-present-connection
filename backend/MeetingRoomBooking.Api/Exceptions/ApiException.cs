namespace MeetingRoomBooking.Api.Exceptions;

public sealed class ApiException(
    int statusCode,
    string errorCode,
    string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string ErrorCode { get; } = errorCode;
}
