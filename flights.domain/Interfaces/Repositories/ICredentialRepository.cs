using flights.domain.Entities;
using System.Collections.Generic;

namespace flights.domain.Interfaces.Repositories
{
    public interface ICredentialRepository
    {
        Credential GetCredentialById(int credencialId);
        Credential GetCredentialByProvider(string provider);
        List<CredentialParameters> GetCredentialParameters(int credentialId);

        CredentialContext GetCredentialContext(int applicationId, int providerId);

    }
}
