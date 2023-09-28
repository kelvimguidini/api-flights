using flights.domain.Entities;

namespace flights.domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        User GetByUsername(string username);
    }
}
