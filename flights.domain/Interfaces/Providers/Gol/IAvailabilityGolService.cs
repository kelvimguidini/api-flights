using flights.domain.Entities;
using flights.domain.Models;
using flights.domain.Models.Availability;
using System.Collections.Generic;

namespace flights.domain.Interfaces.Providers.Gol
{
    public interface IAvailabilityGolService
    {
        AvailabilityRS GetAvailability(SessionProvider session, object availability);

        AvailabilityRS GetAvailabilityBws(SessionProvider session, object availability);
    }
}
