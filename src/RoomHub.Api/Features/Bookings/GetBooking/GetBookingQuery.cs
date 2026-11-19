using RoomHub.Api.Common.CQRS;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed record GetBookingQuery(Guid Id) : IQuery<GetBookingResponse>;
