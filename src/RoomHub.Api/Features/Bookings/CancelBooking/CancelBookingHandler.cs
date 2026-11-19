using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed class CancelBookingHandler(
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<CancelBookingCommand, Unit>
{
    public async Task<Result<Unit>> Handle(CancelBookingCommand command, CancellationToken cancellationToken)
    {
        var booking = await bookings.GetByIdAsync(command.BookingId, cancellationToken);
        if (booking is null)
            return BookingErrors.NotFound(command.BookingId);

        // "Now" is read once, from the injected TimeProvider (R11).
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var cancelResult = booking.Cancel(nowUtc);
        if (cancelResult.IsFailure)
            return cancelResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
