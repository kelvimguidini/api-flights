using flights.application.DTO;
using flights.domain.Models.Availability;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace flights.application.Interfaces
{
    public interface IAvailabilityService
    {
        Task<Availability> GetAvailability(AvailabilityRQDTO availability, string username);
        Task<IEnumerable<domain.Entities.Airport>> AirportAutocomplete(string name);
    }
}