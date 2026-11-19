using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public sealed class Booking
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    // Used by EF Core.
    private Booking()
    {
        BookedBy = string.Empty;
    }

    private Booking(Guid id, Guid roomId, string bookedBy, TimeRange interval, int attendees)
    {
        Id = id;
        RoomId = roomId;
        BookedBy = bookedBy;
        StartUtc = interval.StartUtc;
        EndUtc = interval.EndUtc;
        Attendees = attendees;
        Status = BookingStatus.Active;
    }

    public Guid Id { get; private set; }

    public Guid RoomId { get; private set; }

    public string BookedBy { get; private set; }

    public DateTime StartUtc { get; private set; }

    public DateTime EndUtc { get; private set; }

    public int Attendees { get; private set; }

    public BookingStatus Status { get; private set; }

    public TimeRange Interval => new(StartUtc, EndUtc);

    public bool IsActive => Status == BookingStatus.Active;

    /// <summary>
    /// Creates a booking for <paramref name="room"/> applying R2–R7.
    /// <paramref name="existingBookings"/> must contain the bookings of this room that can conflict
    /// (the domain itself keeps only the active ones of the same room).
    /// </summary>
    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime? startUtc,
        DateTime? endUtc,
        int? attendees,
        DateTime nowUtc,
        IEnumerable<Booking> existingBookings)
    {
        // R2
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.BookedByRequired;

        // R3
        if (startUtc is null || endUtc is null)
            return BookingErrors.IntervalRequired;

        var intervalResult = TimeRange.Create(startUtc.Value, endUtc.Value);
        if (intervalResult.IsFailure)
            return intervalResult.Error;

        var interval = intervalResult.Value;

        // R4
        if (attendees is null or <= 0)
            return BookingErrors.InvalidAttendees;

        if (attendees.Value > room.Capacity)
            return BookingErrors.AttendeesExceedCapacity;

        // R5: start == now is allowed
        if (interval.StartUtc < nowUtc)
            return BookingErrors.StartInPast;

        // R6: exactly 4 hours is allowed
        if (interval.Duration > MaxDuration)
            return BookingErrors.TooLong;

        // R7: only active bookings of the same room
        var overlaps = existingBookings.Any(b =>
            b.RoomId == room.Id && b.IsActive && b.Interval.Overlaps(interval));

        if (overlaps)
            return BookingErrors.Overlap;

        return new Booking(Guid.NewGuid(), room.Id, bookedBy.Trim(), interval, attendees.Value);
    }

    /// <summary>R8: Active -> Cancelled; cancelling twice is a conflict.</summary>
    public Result<Unit> Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }
}
