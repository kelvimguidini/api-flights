using flights.domain.Entities;
using flights.domain.Models;
using System.Collections.Generic;

namespace flights.domain.Interfaces.Providers.Gol
{
    public interface IAuthenticationGolService
    {
        SessionProvider Authentication(Credential credential, List<CredentialParameters> credentialParameters);
        SessionProvider AuthenticationBws(Credential credential, List<CredentialParameters> credentialParameters);
        void Logoff(List<CredentialParameters> credentialParameters, string sessionProvider);
    }
}