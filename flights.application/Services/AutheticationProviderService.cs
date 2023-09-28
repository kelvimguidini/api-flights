using flights.application.Interfaces;
using flights.crosscutting.Utils;
using flights.domain.Interfaces.Providers.Azul;
using flights.domain.Interfaces.Providers.Gol;
using flights.domain.Interfaces.Providers.Latam;
using flights.domain.Interfaces.Repositories;
using flights.domain.Models;

namespace flights.application.Services
{
    public class AutheticationProviderService : IAutheticationProviderService
    {
        private readonly ICredentialRepository _credentialRepository;
        private readonly IAuthenticationAzulService _authenticationAzulService;
        private readonly IAuthenticationGolService _authenticationGolService;
        private readonly IAuthenticationLatamService _authenticationLatamService;

        public AutheticationProviderService(
            ICredentialRepository credentialRepository,
            IAuthenticationAzulService authenticationAzulService,
            IAuthenticationGolService authenticationGolService,
            IAuthenticationLatamService authenticationLatamService
            )
        {
            _credentialRepository = credentialRepository;
            _authenticationAzulService = authenticationAzulService;
            _authenticationGolService = authenticationGolService;
            _authenticationLatamService = authenticationLatamService;
        }

        public SessionProvider AuthenticationProvider(int credencialId, bool token = false)
        {
            var credential = _credentialRepository.GetCredentialById(credencialId);
            var credentialParameters = _credentialRepository.GetCredentialParameters(credential.CredentialId);
            SessionProvider session;
            switch (credential.Provider.Initials)
            {
                case "AD":
                    return _authenticationAzulService.Authentication(credential, credentialParameters);
                case "G3-GWS":
                    return _authenticationGolService.Authentication(credential, credentialParameters); 
                case "G3-BWS":
                    return _authenticationGolService.AuthenticationBws(credential, credentialParameters);
                case "LA":
                    if (token)
                    {
                        session = _authenticationLatamService.GetToken(credential, credentialParameters);
                    }
                    else
                    {
                        session = _authenticationLatamService.Authentication(credential, credentialParameters);
                    }
                    return session;
                default:
                    return new SessionProvider();
            }
        }
    }
}