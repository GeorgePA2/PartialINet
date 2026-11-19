using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    // POST /api/v1/rooms/{roomId}/bookings
    public static void Map(RouteGroupBuilder rooms) =>
        rooms.MapPost("/{roomId:guid}/bookings", async (
                Guid roomId,
                CreateBookingRequest request,
                ICommandHandler<CreateBookingCommand, CreateBookingResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateBookingCommand(
                    roomId,
                    request.BookedBy,
                    request.StartUtc?.UtcDateTime,
                    request.EndUtc?.UtcDateTime,
                    request.Attendees);

                var result = await handler.Handle(command, cancellationToken);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/bookings/{result.Value.Id}", result.Value)
                    : result.Error.ToProblem();
            })
            .WithName("V1_CreateBooking");
}
