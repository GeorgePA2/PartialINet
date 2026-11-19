using RoomHub.Api.Common.CQRS;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomRequest(string? Name, int? Capacity);

public sealed record CreateRoomCommand(string? Name, int? Capacity) : ICommand<CreateRoomResponse>;

public sealed record CreateRoomResponse(Guid Id, string Name, int Capacity);
