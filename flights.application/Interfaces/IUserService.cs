
using flights.application.DTO;

namespace flights.application.Interfaces
{
    public interface IUserService
    {
        string Authenticate(LoginDTO login);
    }
}
