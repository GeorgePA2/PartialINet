using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    // GET /api/v1/rooms/{id}
    public static void Map(RouteGroupBuilder rooms) =>
        rooms.MapGet("/{id:guid}", async (
                Guid id,
                IQueryHandler<GetRoomQuery, GetRoomResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetRoomQuery(id), cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToProblem();
            })
            .WithName("V1_GetRoom");
}
