using flights.domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace flights.domain.Interfaces.Repositories
{
    public interface IAirportsRepository
    {
        Task<IEnumerable<Airport>> GetByIata(List<string> iataList);
        
        Task<IEnumerable<Airport>> GetAutocomplete(string name);
    }
}
