using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<CreateBookingCommand, CreateBookingResponse>
{
    public async Task<Result<CreateBookingResponse>> Handle(
        CreateBookingCommand command, CancellationToken cancellationToken)
    {
        // 1. The room must exist: 404 before any business validation.
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound(command.RoomId);

        // 2. "Now" is read once, from the injected TimeProvider.
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        // 3. Data the domain needs for R7.
        var activeBookings = await bookings.GetActiveForRoomAsync(room.Id, cancellationToken);

        // 4. The domain applies R2–R7.
        var bookingResult = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            nowUtc,
            activeBookings);

        if (bookingResult.IsFailure)
            return bookingResult.Error;

        var booking = bookingResult.Value;
        bookings.Add(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status.ToString());
    }
}
