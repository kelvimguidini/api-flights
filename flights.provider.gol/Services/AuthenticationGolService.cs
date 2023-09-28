using flights.domain.Interfaces.Providers.Gol;
using System;
using System.Collections.Generic;
using GolWS.Homolog.SessionManager;
using GolWS.gws.SessionCreate;
using flights.domain.Entities;
using System.Linq;
using flights.domain.Models;
using flights.crosscutting.AppConfig;
using GolWS.gws.SessionClose;
using flights.crosscutting.Utils;

namespace flights.provider.gol.Services
{
    public class AuthenticationGolService : IAuthenticationGolService
    {
        private readonly SessionManagerClientClient _webservice =
            new SessionManagerClientClient(SessionManagerClientClient.EndpointConfiguration.BasicHttpBinding_ISessionManagerClient, new AppConfiguration("EndPoints", "GolSession").Configuration);
        private readonly SessionCreateRQServiceClient _webserviceGws =
            new SessionCreateRQServiceClient(SessionCreateRQServiceClient.EndpointConfiguration.BasicHttpBinding_ISessionCreateRQService, new AppConfiguration("EndPoints", "Gol").Configuration + "SessionCreateRQService.svc");
        private readonly SessionCloseRQServiceClient _webServiceCloseGws =
            new SessionCloseRQServiceClient(SessionCloseRQServiceClient.EndpointConfiguration.BasicHttpBinding_ISessionCloseRQService, new AppConfiguration("EndPoints", "Gol").Configuration + "SessionCloseRQService.svc");

        #region GWS
        public SessionProvider Authentication(Credential credential, List<CredentialParameters> credentialParameters)
        {
            try
            {
                var partIdFrom = new GolWS.gws.SessionCreate.MessageHeaderFromPartyId[1];
                partIdFrom[0] = new GolWS.gws.SessionCreate.MessageHeaderFromPartyId
                {
                    Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value
                };
                var partIdTo = new GolWS.gws.SessionCreate.MessageHeaderTOPartyId[1];
                partIdTo[0] = new GolWS.gws.SessionCreate.MessageHeaderTOPartyId
                {
                    Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value
                };

                var request = new SessionCreateRQRequest
                {
                    MessageHeader = new GolWS.gws.SessionCreate.MessageHeader
                    {
                        id = "1",
                        version = "1.0",
                        From = new GolWS.gws.SessionCreate.MessageHeaderFrom
                        {
                            PartyId = partIdFrom
                        },
                        To = new GolWS.gws.SessionCreate.MessageHeaderTO
                        {
                            PartyId = partIdTo
                        },
                        Action = "SessionCreateRQ",
                        CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
                        ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
                        Service = new GolWS.gws.SessionCreate.MessageHeaderService()
                        {
                            Value = "SessionCreateRQ"
                        },
                        MessageData = new GolWS.gws.SessionCreate.MessageHeaderMessageData
                        {
                            MessageId = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value,
                            Timestamp = DateTime.Now.AddHours(7).ToString("s")
                        }
                    },
                    Security = new GolWS.gws.SessionCreate.Security
                    {
                        UsernameToken = new GolWS.gws.SessionCreate.SecurityUsernameToken
                        {
                            Username = credential.Username,
                            Password = credential.Password,
                            Organization = credentialParameters.Where(q => q.Parameter == "Organization").FirstOrDefault()?.Value,
                            Domain = credentialParameters.Where(q => q.Parameter == "DomainCode").FirstOrDefault()?.Value
                        }
                    },
                    SessionCreateRQ = new GolWS.gws.SessionCreate.SessionCreateRQ
                    {
                        POS = new SessionCreateRQPOS
                        {
                            Source = new SessionCreateRQPOSSource
                            {
                                PseudoCityCode = credentialParameters.Where(q => q.Parameter == "PseudoCityCode").FirstOrDefault()?.Value
                            }
                        }
                    }
                };

                var xml = WCFXMLManager.SerializeObjectToXml<SessionCreateRQRequest>(request);
                var sessionResponse = _webserviceGws.SessionCreateRQAsync(request.Security, request.MessageHeader, request.SessionCreateRQ).Result;
                var xmlR = WCFXMLManager.SerializeObjectToXml<SessionCreateRQResponse>(sessionResponse);

                return ToSessionProvider(sessionResponse, credentialParameters, credential.CredentialId);
            }
            catch (Exception ex)
            {
                throw new Exception("Houve algum erro ao tentar se conectar ao serviço: " + ex.Message);
            }
        }

        private SessionProvider ToSessionProvider(SessionCreateRQResponse session, List<CredentialParameters> credentialParameters, int credentialId)
        {
            return new SessionProvider()
            {

                AgencyCode = credentialParameters.Where(q => q.Parameter == "AgencyCode").FirstOrDefault()?.Value,
                CredentialId = credentialId,
                Session = new
                {
                    Session = session,
                    CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
                    DefaultTicketingCarrier = credentialParameters.Where(q => q.Parameter == "DefaultTicketingCarrier").FirstOrDefault()?.Value,
                    ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
                    PseudoCityCode = credentialParameters.Where(q => q.Parameter == "PseudoCityCode").FirstOrDefault()?.Value,
                    PersonalCityCode = credentialParameters.Where(q => q.Parameter == "PersonalCityCode").FirstOrDefault()?.Value,
                    AccountingCode = credentialParameters.Where(q => q.Parameter == "AccountingCode").FirstOrDefault()?.Value,
                    OfficeCode = credentialParameters.Where(q => q.Parameter == "OfficeCode").FirstOrDefault()?.Value,
                    MaxStopsQuantity = credentialParameters.Where(q => q.Parameter == "MaxStopsQuantity").FirstOrDefault()?.Value,
                    To = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value,
                    From = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value
                }
            };
        }


        public void Logoff(List<CredentialParameters> credentialParameters, string sessionProvider)
        {
            var logOffRequest = ToLogOffRequest(credentialParameters, sessionProvider);
            var xml = WCFXMLManager.SerializeObjectToXml<SessionCloseRQRequest>(logOffRequest);
            try
            {
                var xmlR = WCFXMLManager.SerializeObjectToXml<SessionCloseRQResponse>(_webServiceCloseGws.SessionCloseRQAsync(logOffRequest.Security, logOffRequest.MessageHeader, logOffRequest.SessionCloseRQ).Result);
            }
            catch (Exception ex)
            {
                //throw new Exception("Houve algum erro ao tentar se conectar ao serviço: " + ex.Message);
            }
        }

        private SessionCloseRQRequest ToLogOffRequest(List<CredentialParameters> credentialParameters, string sessionProvider)
        {
            var partIdFrom = new GolWS.gws.SessionClose.MessageHeaderFromPartyId[1];
            partIdFrom[0] = new GolWS.gws.SessionClose.MessageHeaderFromPartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value
            };
            var partIdTo = new GolWS.gws.SessionClose.MessageHeaderTOPartyId[1];
            partIdTo[0] = new GolWS.gws.SessionClose.MessageHeaderTOPartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value
            };

            return new SessionCloseRQRequest
            {
                MessageHeader = new GolWS.gws.SessionClose.MessageHeader
                {
                    From = new GolWS.gws.SessionClose.MessageHeaderFrom
                    {
                        PartyId = partIdFrom
                    },
                    To = new GolWS.gws.SessionClose.MessageHeaderTO
                    {
                        PartyId = partIdTo
                    },
                    Action = "SessionCloseRQ",
                    CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
                    ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
                    Service = new GolWS.gws.SessionClose.MessageHeaderService()
                    {
                        Value = "SessionCloseRQ"
                    },
                    MessageData = new GolWS.gws.SessionClose.MessageHeaderMessageData
                    {
                        MessageId = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value,
                        Timestamp = DateTime.Now.AddHours(7).ToString("s")
                    }
                },
                Security = new GolWS.gws.SessionClose.Security
                {
                    BinarySecurityToken = sessionProvider
                },
                SessionCloseRQ = new SessionCloseRQ
                {
                    POS = new SessionCloseRQPOS
                    {
                        Source = new SessionCloseRQPOSSource
                        {
                            PseudoCityCode = credentialParameters.Where(q => q.Parameter == "PseudoCityCode").FirstOrDefault()?.Value
                        }
                    }
                }
            };

        }

        #endregion

        #region BWS
        public SessionProvider AuthenticationBws(Credential credential, List<CredentialParameters> credentialParameters)
        {
            var logonRequest = ToLogonRequestBws(credential, credentialParameters);

            try
            {
                return ToSessionProviderBws(_webservice.Logon(logonRequest.Body.agencyCode, logonRequest.Body.logonRequest), credentialParameters.Where(q => q.Parameter == "AgencyCode").FirstOrDefault()?.Value, credential.CredentialId);
            }
            catch (Exception ex)
            {
                throw new Exception("Houve algum erro ao tentar se conectar ao serviço: " + ex.Message);
            }
        }

        private LogonRequest ToLogonRequestBws(Credential credential, List<CredentialParameters> credentialParameters)
        {
            var logonRequest = new LogonRequest()
            {
                Body = new LogonRequestBody()
                {
                    agencyCode = credentialParameters.Where(q => q.Parameter == "AgencyCode").FirstOrDefault()?.Value,
                    logonRequest = new LogonRequestData()
                    {
                        AgentName = credential.Username,
                        Password = credential.Password,
                        DomainCode = credentialParameters.Where(q => q.Parameter == "DomainCode").FirstOrDefault()?.Value,
                    }
                }
            };

            return logonRequest;
        }

        private SessionProvider ToSessionProviderBws(BWSSession session, string agencyCode, int credentialId)
        {
            return new SessionProvider()
            {
                Session = session,
                AgencyCode = agencyCode,
                CredentialId = credentialId
            };
        }
        #endregion
    }
}