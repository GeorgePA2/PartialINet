using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Rooms;

public sealed class Room
{
    // Used by EF Core.
    private Room()
    {
        Name = string.Empty;
    }

    private Room(Guid id, string name, int capacity)
    {
        Id = id;
        Name = name;
        Capacity = capacity;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public int Capacity { get; private set; }

    /// <summary>R1: non-empty name (not only whitespace) and capacity &gt; 0.</summary>
    public static Result<Room> Create(string? name, int? capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoomErrors.NameRequired;

        if (capacity is null or <= 0)
            return RoomErrors.InvalidCapacity;

        return new Room(Guid.NewGuid(), name.Trim(), capacity.Value);
    }
}
