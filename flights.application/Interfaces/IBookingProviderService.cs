using flights.domain.Models.Booking;
using flights.domain.Models.Availability;
using flights.domain.Models;

namespace flights.application.Interfaces
{
    public interface IBookingProviderService
    {
        public Booking GetSellProvider(string provider, SessionProvider session, BookingDTO booking, Availability availability);
    }
}
