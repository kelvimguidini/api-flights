using flights.domain.Models;
using flights.domain.Models.Availability;

namespace flights.domain.Interfaces.Providers.Azul
{
    public interface IAvailabilityAzulService
    {
        AvailabilityRS GetAvailability(SessionProvider session, object availability);
    }
}
