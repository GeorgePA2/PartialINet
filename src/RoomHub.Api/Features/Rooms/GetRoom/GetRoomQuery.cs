using RoomHub.Api.Common.CQRS;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomQuery(Guid Id) : IQuery<GetRoomResponse>;
