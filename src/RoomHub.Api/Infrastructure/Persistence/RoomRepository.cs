using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence;

public sealed class RoomRepository(RoomHubDbContext db) : IRoomRepository
{
    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await db.Rooms.FindAsync([id], cancellationToken);

    // No SaveChanges here: the Unit of Work commits.
    public void Add(Room room) => db.Rooms.Add(room);
}
