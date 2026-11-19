using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence;

public sealed class BookingRepository(RoomHubDbContext db) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await db.Bookings.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken) =>
        await db.Bookings
            .AsNoTracking()
            .Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetActiveForParticipantOnDayAsync(
        string bookedBy, DateTime dayStartUtc, CancellationToken cancellationToken)
    {
        var dayEndUtc = dayStartUtc.AddDays(1);
        return await db.Bookings
            .AsNoTracking()
            .Where(b => b.BookedBy == bookedBy
                        && b.Status == BookingStatus.Active
                        && b.StartUtc >= dayStartUtc
                        && b.StartUtc < dayEndUtc)
            .ToListAsync(cancellationToken);
    }

    // No SaveChanges here: the Unit of Work commits.
    public void Add(Booking booking) => db.Bookings.Add(booking);
}
