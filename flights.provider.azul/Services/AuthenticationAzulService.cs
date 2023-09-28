using flights.domain.Entities;
using flights.domain.Interfaces.Providers.Azul;
using System.Collections.Generic;
using System.Linq;
using flights.domain.Models;
using AzulWS;
//using AzulWS.Homolog;

namespace flights.provider.azul.Services
{
    public class AuthenticationAzulService : IAuthenticationAzulService
    {
        private readonly SessionManagerClientClient _webservice = new SessionManagerClientClient();

        public SessionProvider Authentication(Credential credential, List<CredentialParameters> credentialParameters)
        {
            var request = ToLogonRequest(credential, credentialParameters);

            return ToSessionProvider(_webservice.Logon(request), credential.CredentialId);
        }

        private LogonRequest ToLogonRequest(Credential credential, List<CredentialParameters> credentialParameters)
        {
            var logonRequest = new LogonRequest()
            {
                AgentName = credential.Username,
                Password = credential.Password,
                DomainCode = credentialParameters.Where(q => q.Parameter == "DomainCode").FirstOrDefault()?.Value,
                SystemType = SystemType.WebServicesAPI,
                ChannelType = ChannelType.API
            };

            return logonRequest;
        }

        private SessionProvider ToSessionProvider(LogonResponse session, int credentialId)
        {
            return new SessionProvider()
            {
                Session = session,
                CredentialId = credentialId
            };
        }

        public void LogOff(SessionProvider logon)
        {
            //var sessionContext = logon.Session.GetType().GetProperty("SessionContext").GetValue(logon.Session, null);

            //_webservice.Logout((SessionContext)sessionContext);
        }
    }
}