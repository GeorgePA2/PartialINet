using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public static class GetBookingEndpoint
{
    // GET /api/v1/bookings/{id}
    public static void Map(RouteGroupBuilder bookings) =>
        bookings.MapGet("/{id:guid}", async (
                Guid id,
                IQueryHandler<GetBookingQuery, GetBookingResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetBookingQuery(id), cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToProblem();
            })
            .WithName("V1_GetBooking");
}
