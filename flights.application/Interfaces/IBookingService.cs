using flights.domain.Models;
using flights.domain.Models.Booking;

namespace flights.application.Interfaces
{
    public interface IBookingService
    {
        Booking Sell(BookingDTO booking, string username);
    }
}
