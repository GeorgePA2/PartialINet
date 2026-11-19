using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBooking;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;
using RoomHub.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// JSON: camelCase (web default) + enums as text.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();

// Persistence: SQLite file on disk, next to the project (content root).
var connectionString = builder.Configuration.GetConnectionString("RoomHub")
    ?? $"Data Source={Path.Combine(builder.Environment.ContentRootPath, "roomhub.db")}";

builder.Services.AddDbContext<RoomHubDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RoomHubDbContext>());
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddSingleton(TimeProvider.System);

// Handlers
builder.Services.AddScoped<ICommandHandler<CreateRoomCommand, CreateRoomResponse>, CreateRoomHandler>();
builder.Services.AddScoped<IQueryHandler<GetRoomQuery, GetRoomResponse>, GetRoomHandler>();
builder.Services.AddScoped<ICommandHandler<CreateBookingCommand, CreateBookingResponse>, CreateBookingHandler>();
builder.Services.AddScoped<IQueryHandler<GetBookingQuery, GetBookingResponse>, GetBookingHandler>();
builder.Services.AddScoped<ICommandHandler<CancelBookingCommand, Unit>, CancelBookingHandler>();

var app = builder.Build();

// Create the schema if the database does not exist yet (non-destructive; keeps existing data).
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<RoomHubDbContext>().Database.EnsureCreated();
}

// Unhandled exceptions and empty 4xx responses (e.g. malformed JSON, unknown route) as ProblemDetails.
app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/", () => "RoomHub API is running.");

// All business endpoints live exclusively under /api/v1.
var api = app.MapGroup("/api/v1");
var rooms = api.MapGroup("/rooms").WithTags("Rooms");
var bookings = api.MapGroup("/bookings").WithTags("Bookings");

CreateRoomEndpoint.Map(rooms);       // POST /api/v1/rooms
GetRoomEndpoint.Map(rooms);          // GET  /api/v1/rooms/{id}
CreateBookingEndpoint.Map(rooms);    // POST /api/v1/rooms/{roomId}/bookings
GetBookingEndpoint.Map(bookings);    // GET  /api/v1/bookings/{id}
CancelBookingEndpoint.Map(bookings); // POST /api/v1/bookings/{id}/cancel

app.Run();

public partial class Program
{
}
