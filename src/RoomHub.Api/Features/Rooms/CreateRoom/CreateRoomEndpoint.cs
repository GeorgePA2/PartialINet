using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    // POST /api/v1/rooms
    public static void Map(RouteGroupBuilder rooms) =>
        rooms.MapPost("/", async (
                CreateRoomRequest request,
                ICommandHandler<CreateRoomCommand, CreateRoomResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(
                    new CreateRoomCommand(request.Name, request.Capacity), cancellationToken);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/rooms/{result.Value.Id}", result.Value)
                    : result.Error.ToProblem();
            })
            .WithName("V1_CreateRoom");
}
