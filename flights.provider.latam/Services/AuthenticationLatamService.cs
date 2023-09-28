using System;
using System.Collections.Generic;
using LatamWS.Homolog.SessionCreate;
using LatamWS.Homolog.SessionClose;
using flights.domain.Entities;
using System.Linq;
using flights.domain.Models;
using flights.domain.Interfaces.Providers.Latam;
using flights.crosscutting.AppConfig;
using flights.crosscutting.Utils;

namespace flights.provider.Latam.Services
{
    public class AuthenticationLatamService : IAuthenticationLatamService
    {
        private readonly SessionCreatePortTypeClient _webServiceCreate =
            new SessionCreatePortTypeClient(SessionCreatePortTypeClient.EndpointConfiguration.SessionCreatePortType, new AppConfiguration("EndPoints", "Latam").Configuration);

        private readonly SessionClosePortTypeClient _webServiceClose =
            new SessionClosePortTypeClient(SessionClosePortTypeClient.EndpointConfiguration.SessionClosePortType, new AppConfiguration("EndPoints", "Latam").Configuration);

        public SessionProvider Authentication(Credential credential, List<CredentialParameters> credentialParameters)
        {
            var logonRequest = ToLogonRequest(credential, credentialParameters);

            var xml = WCFXMLManager.SerializeObjectToXml<SessionCreateRQRequest>(logonRequest);
            try
            {
                var logon = _webServiceCreate.SessionCreateRQAsync(logonRequest.MessageHeader, logonRequest.Security, logonRequest.SessionCreateRQ).Result;
                
                var xmlReq = WCFXMLManager.SerializeObjectToXml<SessionCreateRQRequest>(logonRequest);
                var xmlR = WCFXMLManager.SerializeObjectToXml<SessionCreateRQResponse>(logon);
                return ToSessionProvider(logon, credential.CredentialId);
            }
            catch (Exception ex)
            {
                throw new Exception("Houve algum erro ao tentar se conectar ao serviço: " + ex.Message);
            }
        }

        private SessionCreateRQRequest ToLogonRequest(Credential credential, List<CredentialParameters> credentialParameters)
        {
            var partIdFrom = new LatamWS.Homolog.SessionCreate.PartyId[1];
            partIdFrom[0] = new LatamWS.Homolog.SessionCreate.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value
            };
            var partIdTo = new LatamWS.Homolog.SessionCreate.PartyId[1];
            partIdTo[0] = new LatamWS.Homolog.SessionCreate.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value
            };

            return new SessionCreateRQRequest
            {
                MessageHeader = new LatamWS.Homolog.SessionCreate.MessageHeader
                {
                    From = new LatamWS.Homolog.SessionCreate.From
                    {
                        PartyId = partIdFrom
                    },
                    To = new LatamWS.Homolog.SessionCreate.To
                    {
                        PartyId = partIdTo
                    },
                    Action = "SessionCreateRQ",
                    //CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
                    ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
                    Service = new LatamWS.Homolog.SessionCreate.Service()
                    {
                        type = "SabreXML",
                        Value = "SessionCreateRQ"
                    },
                    MessageData = new LatamWS.Homolog.SessionCreate.MessageData
                    {
                        MessageId = "4213662796178120152",
                        Timestamp = DateTime.Now.AddHours(7).ToString("s")
                    }
                },
                Security = new LatamWS.Homolog.SessionCreate.Security
                {
                    UsernameToken = new LatamWS.Homolog.SessionCreate.SecurityUsernameToken
                    {
                        Username = credential.Username,
                        Domain = credentialParameters.Where(q => q.Parameter == "DomainCode").FirstOrDefault()?.Value,
                        Password = credential.Password,
                        Organization = credentialParameters.Where(q => q.Parameter == "Organization").FirstOrDefault()?.Value
                    }
                },
                SessionCreateRQ = new SessionCreateRQ
                {
                    POS = new SessionCreateRQPOS
                    {
                        Source = new SessionCreateRQPOSSource
                        {
                            PseudoCityCode = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value
                        }
                    },
                    returnContextID = false
                }
            };

        }

        private SessionCloseRQRequest ToLogOffRequest(List<CredentialParameters> credentialParameters, string sessionProvider)
        {
            var partIdFrom = new LatamWS.Homolog.SessionClose.PartyId[1];
            partIdFrom[0] = new LatamWS.Homolog.SessionClose.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value
            };
            var partIdTo = new LatamWS.Homolog.SessionClose.PartyId[1];
            partIdTo[0] = new LatamWS.Homolog.SessionClose.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value
            };

            return new SessionCloseRQRequest
            {
                MessageHeader = new LatamWS.Homolog.SessionClose.MessageHeader
                {
                    id = "1",
                    version = "1.0",
                    From = new LatamWS.Homolog.SessionClose.From
                    {
                        PartyId = partIdFrom
                    },
                    To = new LatamWS.Homolog.SessionClose.To
                    {
                        PartyId = partIdTo
                    },
                    Action = "SessionCloseRQ",
                    CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
                    ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
                    Service = new LatamWS.Homolog.SessionClose.Service()
                    {
                        type = "SabreXML",
                        Value = "SessionCloseRQ"
                    }
                },
                Security = new LatamWS.Homolog.SessionClose.Security
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

        private SessionProvider ToSessionProvider(SessionCreateRQResponse session, int credentialId)
        {
            return new SessionProvider()
            {
                Session = session,
                CredentialId = credentialId
            };
        }

        public void Logoff(List<CredentialParameters> credentialParameters, string sessionProvider)
        {
            var logOffRequest = ToLogOffRequest(credentialParameters, sessionProvider);
            var xml = WCFXMLManager.SerializeObjectToXml<SessionCloseRQRequest>(logOffRequest);
            try
            {
                var xmlR = WCFXMLManager.SerializeObjectToXml<SessionCloseRQResponse>(_webServiceClose.SessionCloseRQAsync(logOffRequest).Result);
            }
            catch (Exception ex)
            {
                throw new Exception("Houve algum erro ao tentar se conectar ao serviço: " + ex.Message);
            }
        }

        public SessionProvider GetToken(Credential credential, List<CredentialParameters> credentialParameters)
        {
            try
            {
                string messageReturn;

                messageReturn = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:sec=\"http://schemas.xmlsoap.org/ws/2002/12/secext\" xmlns:mes=\"http://www.ebxml.org/namespaces/messageHeader\" xmlns:web=\"http://webservices.sabre.com\">";
                messageReturn += $" <soapenv:Header>";
                messageReturn += $"     <sec:Security>";
                messageReturn += $"         <sec:UsernameToken>";
                messageReturn += $"             <sec:Username>{credential.Username}</sec:Username >";
                messageReturn += $"             <sec:Password>{credential.Password}</sec:Password>";
                messageReturn += $"             <Organization>{credentialParameters.Where(q => q.Parameter == "Organization").FirstOrDefault()?.Value}</Organization>";
                messageReturn += $"             <Domain>{credentialParameters.Where(q => q.Parameter == "DomainCode").FirstOrDefault()?.Value}</Domain>";
                messageReturn += $"         </sec:UsernameToken>";
                messageReturn += $"     </sec:Security>";
                messageReturn += $"     <mes:MessageHeader mes:id=\"1\" mes:version=\"1.0.0\">";
                messageReturn += $"         <mes:From>";
                messageReturn += $"             <mes:PartyId mes:type=\"URI\">{credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value}</mes:PartyId>";
                messageReturn += $"         </mes:From>";
                messageReturn += $"         <mes:To>";
                messageReturn += $"             <mes:PartyId mes:type=\"URI\">{credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value}</mes:PartyId>";
                messageReturn += $"         </mes:To>";
                messageReturn += $"         <mes:CPAId>{credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value}</mes:CPAId>";
                messageReturn += $"         <mes:ConversationId>{credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value}</mes:ConversationId>";
                messageReturn += $"         <mes:Service mes:type=\"OTA\">TokenCreateRQService</mes:Service>";
                messageReturn += $"         <mes:Action>TokenCreateRQ</mes:Action>";
                messageReturn += $"         <mes:MessageData>";
                messageReturn += $"             <mes:MessageId></mes:MessageId>";
                messageReturn += $"             <mes:Timestamp></mes:Timestamp>";
                messageReturn += $"         </mes:MessageData>";
                messageReturn += $"         <mes:Description xml:lang=\"en-us\"/>";
                messageReturn += $"     </mes:MessageHeader>";
                messageReturn += $" </soapenv:Header>";
                messageReturn += $" <soapenv:Body>";
                messageReturn += $"     <web:TokenCreateRQ Version=\"1.0.0\"/>";

                messageReturn += $" </soapenv:Body>";
                messageReturn += $"</soapenv:Envelope>";

                var xmlStringResult = WCFXMLManager.ResponseWCF(new AppConfiguration("EndPoints", "Latam").Configuration, "", messageReturn, "s:");


                xmlStringResult = xmlStringResult.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<soap-env:Envelope xmlns:soap-env=\"http://schemas.xmlsoap.org/soap/envelope/\">", "");

                xmlStringResult = xmlStringResult.Replace("<soap-env:Body><sws:TokenCreateRS xmlns:sws=\"http://webservices.sabre.com\" Version=\"1.0.0\"><sws:Success/></sws:TokenCreateRS></soap-env:Body></soap-env:Envelope>", "");
                xmlStringResult = xmlStringResult.Replace("eb:", "");
                xmlStringResult = xmlStringResult.Replace("soap-env:", "");
                xmlStringResult = xmlStringResult.Replace("wsse:", "");

                xmlStringResult = xmlStringResult.Replace("<Security xmlns:wsse=\"http://schemas.xmlsoap.org/ws/2002/12/secext\">", "");
                xmlStringResult = xmlStringResult.Replace("</Security>", "");

                var obj = new { tatus = false };

                return new SessionProvider
                {
                    Session = new
                    {
                        Token = WCFXMLManager.ElementValue(xmlStringResult, "Header", "BinarySecurityToken"),
                        From = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value,
                        To = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value,
                        CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
                        ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
                        MessageId = WCFXMLManager.ElementValue(xmlStringResult, "Header", "MessageId"),
                        PseudoCityCode = credentialParameters.Where(q => q.Parameter == "PseudoCityCode").FirstOrDefault()?.Value,
                        PersonalCityCode = credentialParameters.Where(q => q.Parameter == "PersonalCityCode").FirstOrDefault()?.Value,
                        AccountingCode = credentialParameters.Where(q => q.Parameter == "AccountingCode").FirstOrDefault()?.Value,
                        OfficeCode = credentialParameters.Where(q => q.Parameter == "OfficeCode").FirstOrDefault()?.Value,
                        MaxStopsQuantity = credentialParameters.Where(q => q.Parameter == "MaxStopsQuantity").FirstOrDefault()?.Value
                    }
                };

            }
            catch (Exception ex)
            {
                throw new Exception("Houve algum erro ao tentar se conectar ao serviço: " + ex.Message);
            }
        }
    }
}