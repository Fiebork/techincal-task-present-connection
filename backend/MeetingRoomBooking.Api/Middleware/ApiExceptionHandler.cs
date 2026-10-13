using MeetingRoomBooking.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.Api.Middleware;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ApiException apiException)
        {
            return false;
        }

        logger.LogWarning(exception, "Request failed with error code {ErrorCode}", apiException.ErrorCode);

        httpContext.Response.StatusCode = apiException.StatusCode;
        var problemDetails = new ProblemDetails
        {
            Status = apiException.StatusCode,
            Title = "Request failed",
            Detail = apiException.Message,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.com/{apiException.StatusCode}"
        };
        problemDetails.Extensions["code"] = apiException.ErrorCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
