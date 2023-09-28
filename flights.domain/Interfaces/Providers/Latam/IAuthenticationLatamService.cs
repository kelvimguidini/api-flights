using flights.domain.Entities;
using flights.domain.Models;
using System.Collections.Generic;

namespace flights.domain.Interfaces.Providers.Latam
{
    public interface IAuthenticationLatamService
    {
        SessionProvider Authentication(Credential credential, List<CredentialParameters> credentialParameters);
        void Logoff(List<CredentialParameters> credentialParameters, string sessionProvider);
        SessionProvider GetToken(Credential credential, List<CredentialParameters> credentialParameters);
    }
}
