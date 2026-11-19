using RoomHub.Api.Common.CQRS;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingRequest(
    string? BookedBy,
    DateTimeOffset? StartUtc,
    DateTimeOffset? EndUtc,
    int? Attendees);

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime? StartUtc,
    DateTime? EndUtc,
    int? Attendees) : ICommand<CreateBookingResponse>;

public sealed record CreateBookingResponse(Guid Id, Guid RoomId, string Status);
