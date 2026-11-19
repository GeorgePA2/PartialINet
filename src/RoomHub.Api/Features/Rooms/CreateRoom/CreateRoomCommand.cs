using RoomHub.Api.Common.CQRS;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomCommand(string? Name, int? Capacity) : ICommand<CreateRoomResponse>;
