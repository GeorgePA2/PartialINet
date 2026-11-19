using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public static class BookingErrors
{
    public static readonly Error BookedByRequired =
        Error.Validation("Booking.BookedByRequired", "BookedBy is required.");

    public static readonly Error IntervalRequired =
        Error.Validation("Booking.IntervalRequired", "StartUtc and EndUtc are required.");

    public static readonly Error InvalidInterval =
        Error.Validation("Booking.InvalidInterval", "EndUtc must be strictly after StartUtc.");

    public static readonly Error InvalidAttendees =
        Error.Validation("Booking.InvalidAttendees", "Attendees must be strictly positive.");

    public static readonly Error AttendeesExceedCapacity =
        Error.Validation("Booking.AttendeesExceedCapacity", "Attendees cannot exceed the room capacity.");

    public static readonly Error StartInPast =
        Error.Validation("Booking.StartInPast", "A booking cannot start in the past.");

    public static readonly Error TooLong =
        Error.Validation("Booking.TooLong", "A booking can last at most 4 hours.");

    public static readonly Error Overlap =
        Error.Conflict("Booking.Overlap", "The room already has an active booking in this interval.");

    public static readonly Error ParticipantDailyLimit =
        Error.Conflict("Booking.ParticipantDailyLimit", "A participant cannot have more than 2 active bookings on the same UTC day.");

    public static readonly Error AlreadyCancelled =
        Error.Conflict("Booking.AlreadyCancelled", "The booking is already cancelled.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Booking.NotFound", $"The booking '{id}' does not exist.");
}
