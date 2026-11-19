using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

/// <summary>Value object for a half-open interval [StartUtc, EndUtc).</summary>
public sealed record TimeRange
{
    internal TimeRange(DateTime startUtc, DateTime endUtc)
    {
        StartUtc = startUtc;
        EndUtc = endUtc;
    }

    public DateTime StartUtc { get; }

    public DateTime EndUtc { get; }

    public TimeSpan Duration => EndUtc - StartUtc;

    /// <summary>R3: the end must be strictly after the start (equal is invalid).</summary>
    public static Result<TimeRange> Create(DateTime startUtc, DateTime endUtc)
    {
        if (endUtc <= startUtc)
            return BookingErrors.InvalidInterval;

        return new TimeRange(startUtc, endUtc);
    }

    /// <summary>R7: strict comparison, so intervals that only touch do not overlap.</summary>
    public bool Overlaps(TimeRange other) =>
        StartUtc < other.EndUtc && other.StartUtc < EndUtc;
}
