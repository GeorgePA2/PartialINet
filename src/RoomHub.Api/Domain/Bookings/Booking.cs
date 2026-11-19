using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public sealed class Booking
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    public const int MaxActiveBookingsPerParticipantPerDay = 2;

    // R10: rooms with capacity > LargeRoomCapacityThreshold have a shorter maximum duration.
    public const int LargeRoomCapacityThreshold = 10;

    public static readonly TimeSpan MaxDurationForLargeRoom = TimeSpan.FromHours(2);

    // R11: minimum notice before the start for a cancellation.
    public static readonly TimeSpan MinCancellationNotice = TimeSpan.FromMinutes(60);

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
    /// <paramref name="participantBookings"/> must contain the active bookings of the same bookedBy (R9).
    /// </summary>
    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime? startUtc,
        DateTime? endUtc,
        int? attendees,
        DateTime nowUtc,
        IEnumerable<Booking> existingBookings,
        IEnumerable<Booking> participantBookings)
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

        // R10: capacity > 10 -> at most 2 hours (exactly 2 hours is allowed)
        if (room.Capacity > LargeRoomCapacityThreshold && interval.Duration > MaxDurationForLargeRoom)
            return BookingErrors.TooLongForLargeRoom;

        // R6: capacity <= 10 keeps the 4-hour limit (exactly 4 hours is allowed)
        if (interval.Duration > MaxDuration)
            return BookingErrors.TooLong;

        // R7: only active bookings of the same room
        var overlaps = existingBookings.Any(b =>
            b.RoomId == room.Id && b.IsActive && b.Interval.Overlaps(interval));

        if (overlaps)
            return BookingErrors.Overlap;

        // R9: at most 2 active bookings per participant per UTC day of startUtc, across all rooms.
        var participant = bookedBy.Trim();
        var day = interval.StartUtc.Date;
        var activeSameDay = participantBookings.Count(b =>
            b.IsActive
            && string.Equals(b.BookedBy, participant, StringComparison.Ordinal)
            && b.StartUtc.Date == day);

        if (activeSameDay >= MaxActiveBookingsPerParticipantPerDay)
            return BookingErrors.ParticipantDailyLimit;

        return new Booking(Guid.NewGuid(), room.Id, participant, interval, attendees.Value);
    }

    /// <summary>
    /// R8: Active -> Cancelled; cancelling twice is a conflict.
    /// R11: allowed only if the booking starts at least 60 minutes after <paramref name="nowUtc"/>.
    /// </summary>
    public Result<Unit> Cancel(DateTime nowUtc)
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        if (StartUtc - nowUtc < MinCancellationNotice)
            return BookingErrors.TooLateToCancel;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }
}
