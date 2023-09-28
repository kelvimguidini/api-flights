using flights.domain.Models;


namespace flights.application.Interfaces
{
    public interface IAutheticationProviderService
    {
        SessionProvider AuthenticationProvider(int credencialId, bool token = false);
    }
}
