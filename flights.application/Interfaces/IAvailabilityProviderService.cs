using flights.domain.Models;
using flights.domain.Models.Availability;


namespace flights.application.Interfaces
{
    public interface IAvailabilityProviderService
    {
        Availability GetAvailabilityProvider(string provider, SessionProvider session, object availability);
    }
}
