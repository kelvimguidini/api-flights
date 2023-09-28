using flights.domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace flights.domain.Interfaces.Repositories
{
    public interface IAirlineDetailsRepository
    {
        Task<IEnumerable<AirlineDetails>> GetByAirlinesCode(List<string> code);
    }
}
