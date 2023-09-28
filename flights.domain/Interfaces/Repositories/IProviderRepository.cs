using flights.domain.Entities;
using System.Collections.Generic;

namespace flights.domain.Interfaces.Repositories
{
    public interface IProviderRepository
    {
        IEnumerable<Provider> GetProvider();
        Provider GetProviderByInitials(string initials);
    }
}
