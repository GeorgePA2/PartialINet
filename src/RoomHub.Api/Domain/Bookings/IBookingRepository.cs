namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>All active bookings of a room (past, current and future).</summary>
    Task<IReadOnlyList<Booking>> GetActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken);

    /// <summary>R9: active bookings of a participant (all rooms) whose start is in [dayStartUtc, dayStartUtc + 1 day).</summary>
    Task<IReadOnlyList<Booking>> GetActiveForParticipantOnDayAsync(
        string bookedBy, DateTime dayStartUtc, CancellationToken cancellationToken);

    void Add(Booking booking);
}
