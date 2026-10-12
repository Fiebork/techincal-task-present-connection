using MeetingRoomBooking.Api.Configuration;
using MeetingRoomBooking.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services
    .AddOptions<BookingRulesOptions>()
    .BindConfiguration(BookingRulesOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddProblemDetails();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Request validation failed",
            Type = "https://httpstatuses.com/400"
        };

        problemDetails.Extensions["code"] = ApiErrorCodes.ValidationFailed;
        return new BadRequestObjectResult(problemDetails);
    };
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program;