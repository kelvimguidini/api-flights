using flights.domain.Entities;
using flights.domain.Models;
using flights.domain.Models.Booking;
using flights.domain.Models.GetPrice;
using System.Collections.Generic;

namespace flights.domain.Interfaces.Providers.Gol
{
    public interface IBookingGolService
    {
        GetPriceRS PriceItinerary(SessionProvider session, GetPriceRQ priceRQ);

        public string Sell(SessionProvider session, GetPriceRQ priceRQ, List<CredentialParameters> credentialParameters);

        public BookingRS SellBws(SessionProvider session, GetPriceRQ priceRQ);
    }
}