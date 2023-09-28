using flights.domain.Entities;
using flights.domain.Models;
using System.Collections.Generic;

namespace flights.domain.Interfaces.Providers.Azul
{
    public interface IAuthenticationAzulService
    {
        SessionProvider Authentication(Credential credential, List<CredentialParameters> credentialParameters);
        void LogOff(SessionProvider logon);
    }
}
