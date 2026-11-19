using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    // POST /api/v1/bookings/{id}/cancel
    public static void Map(RouteGroupBuilder bookings) =>
        bookings.MapPost("/{id:guid}/cancel", async (
                Guid id,
                ICommandHandler<CancelBookingCommand, Unit> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new CancelBookingCommand(id), cancellationToken);
                return result.IsSuccess ? Results.NoContent() : result.Error.ToProblem();
            })
            .WithName("V1_CancelBooking");
}
