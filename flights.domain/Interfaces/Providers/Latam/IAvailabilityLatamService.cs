using flights.domain.Entities;
using flights.domain.Models;
using flights.domain.Models.Availability;
using System.Collections.Generic;
using System.Linq;

namespace flights.domain.Interfaces.Providers.Latam
{
    public interface IAvailabilityLatamService
    {
        AvailabilityRS GetAvailability(SessionProvider session, object availability);
    }
}
