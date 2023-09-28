using flights.crosscutting.DomainObjects;
using flights.domain.Models;
using flights.domain.Models.Booking;
using flights.domain.Models.GetPrice;
using flights.domain.Models.Provider.Azul;

namespace flights.domain.Interfaces.Providers.Azul
{
    public interface IBookingAzulService
    {
        string PriceItineraryByKeys(SessionProvider session, PriceItineraryRequestWithKeys request);

        BookingRS SellByKey(SessionProvider session, SellWithKeyRequest sellRequest);

        SellWithKeyRequest GenerateSellRequest(PriceItineraryRequestWithKeys priceRQAzul);

    }
}
