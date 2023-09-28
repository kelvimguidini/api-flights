using AutoMapper;
using AzulWS;
using flights.crosscutting.Utils;
using flights.domain.Interfaces.Providers.Azul;
using flights.domain.Models;
using flights.domain.Models.Availability;
using flights.provider.azul.AppConfig;
using flights.provider.azul.Models;
using System;

//using AzulWS.Homolog;

namespace flights.provider.Azul.Services
{
    public class AvailabilityAzulService : IAvailabilityAzulService
    {
        private readonly IMapper _mapper;
        private readonly BookingManagerClientClient _websevice = new BookingManagerClientClient(BookingManagerClientClient.EndpointConfiguration.SecureEndpoint_BM, new crosscutting.AppConfig.AppConfiguration("EndPoints", "Azul").Configuration);
        private AppConfiguration _appConfiguration;

        public AvailabilityAzulService(IMapper mapper)
        {
            _mapper = mapper;
            _appConfiguration = new AppConfiguration();
        }

        private readonly string[] _fareTypes = { "W", "P", "T", "R", "C" };

        public AvailabilityRS GetAvailability(SessionProvider session, object availability)
        {
            try
            {
                var obj = availability.GetType().GetProperty("availabilityRQDTO");

                short countTotalPassangers = Convert.ToInt16(obj.GetValue(availability, null).GetType().GetProperty("CountTotalPassangers").GetValue(obj.GetValue(availability, null), null));
                var countADT = obj.GetValue(availability, null).GetType().GetProperty("CountADT").GetValue(obj.GetValue(availability, null), null);
                var countCHD = obj.GetValue(availability, null).GetType().GetProperty("CountCHD").GetValue(obj.GetValue(availability, null), null);
                AvailabilityRequest availabilityRequest = _mapper.Map<AvailabilityRequest>(obj.GetValue(availability, null));

                var ptc = new AzulWS.PaxPriceType[(int)countTotalPassangers];

                for (int i = 0; i < (int)countTotalPassangers; i++)
                {
                    ptc[i] = new AzulWS.PaxPriceType() { PaxType = "ADT" };
                }

                for (int i = (int)countADT; i < (int)countADT + (int)countCHD; i++)
                {
                    ptc[i] = new AzulWS.PaxPriceType() { PaxType = "CHD" };
                }

                availabilityRequest.PaxPriceTypes = ptc;
                availabilityRequest.FareTypes = _fareTypes;
                availabilityRequest.SourceOrganization = "01401552";
                availabilityRequest.PaxCount = (short)countTotalPassangers;
                availabilityRequest.CarrierCode = "AD";

                availabilityRequest.BeginTime = new AzulWS.Time()
                {
                    TotalMinutes = 0
                };

                availabilityRequest.EndTime = new AzulWS.Time()
                {
                    TotalMinutes = 1439
                };

                var _sessionContext = ((LogonResponse)session.Session).SessionContext;

                var _objectContext = new SessionContextRequest();

                _objectContext.session = _mapper.Map<SessionAzulRequest>(_sessionContext);
                _objectContext.availabilityRequest = _mapper.Map<AvailabilityAzulRequest>(availabilityRequest);

                var url = _appConfiguration.UrlSVCBaseAzul;
                var soapUrl = _appConfiguration.SoapURLGetAvailabilityByTripAzul;

                var stringXmlRequest = BuildStringXmlRequest(_objectContext);

                var xmlStringResult = WCFXMLManager.ResponseWCF(url, soapUrl, stringXmlRequest, "s:");


                xmlStringResult = xmlStringResult.Replace("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><GetAvailabilityByTripResponse >", "");
                xmlStringResult = xmlStringResult.Replace("</GetAvailabilityByTripResponse></s:Body></s:Envelope>", "");
                
                return new AvailabilityRS
                {
                    Availability = WCFXMLManager.SerializeXmlToObject<GetAvailabilityByTripResult>(xmlStringResult)
                };

            }
            catch (Exception ex)
            {
                throw new Exception("Erro obter disponibilidade Azul: " + ex.Message);
            }
        }

        private string BuildStringXmlRequest(SessionContextRequest contextRequest)
        {
            var messageReturn = "";
            messageReturn += "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:book='http://schemas.navitaire.com/ClientServices/BookingManager/BookingManagerClient' xmlns:com='http://schemas.navitaire.com/Common' xmlns:book1='http://schemas.navitaire.com/Messages/Booking' xmlns:itin='http://schemas.navitaire.com/Messages/Itinerary'>";
            messageReturn += "<soapenv:Header/>";
            messageReturn += "<soapenv:Body>";
            messageReturn += "<book:GetAvailabilityByTrip>";
            messageReturn += "<book:session>";
            #region Parametros Session Extras (desnecessarios)
            //messageReturn += $"<com:SessionControl>{contextRequest.session.SessionControl}</com:SessionControl > ";
            //messageReturn += $"<com:SystemType>{contextRequest.session.SystemType}</com:SystemType > ";
            //messageReturn += $"<com:SessionID>{contextRequest.session.SessionID}</com:SessionID > ";
            //messageReturn += $"<com:SequenceNumber>{contextRequest.session.SequenceNumber}</com:SequenceNumber > ";
            //messageReturn += $"<com:MessageVersion>{contextRequest.session.MessageVersion}</com:MessageVersion > ";
            //messageReturn += $"<com:Signature>{contextRequest.session.Signature}</com:Signature > ";
            //messageReturn += "<com:SessionPrincipal>";
            //messageReturn += $"<com:RoleCode>{contextRequest.session.SessionPrincipal.RoleCode}</com:RoleCode > ";
            //messageReturn += "</com:SessionPrincipal>";
            //messageReturn += $"<com:ChannelType>{contextRequest.session.ChannelType}</com:ChannelType > ";
            //messageReturn += $"<com:InTransaction>{contextRequest.session.InTransaction.ToString().ToLower()}</com:InTransaction > ";
            //messageReturn += $"<com:TransactionDepth>{contextRequest.session.TransactionDepth}</com:TransactionDepth > ";
            //messageReturn += $"<com:TransactionCount>{contextRequest.session.TransactionCount}</com:TransactionCount > ";
            #endregion
            messageReturn += $"<com:SecureToken>{contextRequest.session.SecureToken}</com:SecureToken > ";
            messageReturn += "</book:session>";

            messageReturn += "<book:tripAvailabilityRequest>";
            messageReturn += "<book1:AvailabilityRequests>";

            // AvailabilityRequest IDA
            messageReturn += "<AvailabilityRequest xmlns='http://schemas.navitaire.com/Messages/Booking'>";
            messageReturn += $"<DepartureStation>{contextRequest.availabilityRequest.DepartureStation}</DepartureStation > ";
            messageReturn += $"<ArrivalStation>{contextRequest.availabilityRequest.ArrivalStation}</ArrivalStation > ";
            messageReturn += $"<BeginDate>{contextRequest.availabilityRequest.BeginDate.ToString("yyyy-MM-dd")}</BeginDate > ";
            messageReturn += $"<EndDate>{contextRequest.availabilityRequest.BeginDate.ToString("yyyy-MM-dd")}</EndDate > ";
            //messageReturn += $"<CarrierCode>{contextRequest.availabilityRequest.CarrierCode}</CarrierCode > ";
            messageReturn += $"<FlightType>{contextRequest.availabilityRequest.FlightType}</FlightType > ";
            messageReturn += $"<PaxCount>{contextRequest.availabilityRequest.PaxCount}</PaxCount > ";
            messageReturn += $"<Dow>{contextRequest.availabilityRequest.Dow}</Dow > ";
            messageReturn += $"<CurrencyCode>{contextRequest.availabilityRequest.CurrencyCode}</CurrencyCode > ";
            messageReturn += $"<DisplayCurrencyCode>{contextRequest.availabilityRequest.DisplayCurrencyCode}</DisplayCurrencyCode > ";
            messageReturn += $"<AvailabilityType>{contextRequest.availabilityRequest.AvailabilityType}</AvailabilityType > ";
            messageReturn += $"<SourceOrganization>{contextRequest.availabilityRequest.SourceOrganization}</SourceOrganization > ";
            messageReturn += $"<MaximumConnectingFlights>{contextRequest.availabilityRequest.MaximumConnectingFlights}</MaximumConnectingFlights > ";
            messageReturn += $"<AvailabilityFilter>{contextRequest.availabilityRequest.AvailabilityFilter}</AvailabilityFilter > ";
            messageReturn += $"<FareClassControl>{contextRequest.availabilityRequest.FareClassControl}</FareClassControl > ";
            messageReturn += $"<InboundOutbound>{contextRequest.availabilityRequest.InboundOutbound}</InboundOutbound > ";
            messageReturn += $"<SSRCollectionsMode>None</SSRCollectionsMode>";
            messageReturn += $"<NightsStay>{contextRequest.availabilityRequest.NightsStay}</NightsStay > ";
            messageReturn += $"<IncludeAllotments>{contextRequest.availabilityRequest.IncludeAllotments.ToString().ToLower()}</IncludeAllotments > ";
            messageReturn += "<BeginTime>";
            messageReturn += $"<com:TotalMinutes>{contextRequest.availabilityRequest.BeginTime.TotalMinutes}</com:TotalMinutes > ";
            messageReturn += "</BeginTime>";
            messageReturn += "<EndTime>";
            messageReturn += $"<com:TotalMinutes>{contextRequest.availabilityRequest.EndTime.TotalMinutes}</com:TotalMinutes > ";
            messageReturn += "</EndTime>";
            messageReturn += "<PaxPriceTypes>";

            foreach (var paxType in contextRequest.availabilityRequest.PaxPriceTypes)
            {
                messageReturn += $"<itin:PaxPriceType>";
                messageReturn += $"<itin:PaxType>{paxType.PaxType}</itin:PaxType>";
                messageReturn += $"</itin:PaxPriceType>";
            }

            messageReturn += "</PaxPriceTypes>";
            messageReturn += "</AvailabilityRequest>";

            // AvailabilityRequest VOLTA

            if (contextRequest.availabilityRequest.EndDate != DateTime.MinValue)
            {
                messageReturn += "<AvailabilityRequest xmlns='http://schemas.navitaire.com/Messages/Booking'>";
                messageReturn += $"<DepartureStation>{contextRequest.availabilityRequest.ArrivalStation}</DepartureStation > ";
                messageReturn += $"<ArrivalStation>{contextRequest.availabilityRequest.DepartureStation}</ArrivalStation > ";
                messageReturn += $"<BeginDate>{contextRequest.availabilityRequest.EndDate.ToString("yyyy-MM-dd")}</BeginDate > ";
                messageReturn += $"<EndDate>{contextRequest.availabilityRequest.EndDate.ToString("yyyy-MM-dd")}</EndDate > ";
                //messageReturn += $"<CarrierCode>{contextRequest.availabilityRequest.CarrierCode}</CarrierCode > ";
                messageReturn += $"<FlightType>{contextRequest.availabilityRequest.FlightType}</FlightType > ";
                messageReturn += $"<PaxCount>{contextRequest.availabilityRequest.PaxCount}</PaxCount > ";
                messageReturn += $"<Dow>{contextRequest.availabilityRequest.Dow}</Dow > ";
                messageReturn += $"<CurrencyCode>{contextRequest.availabilityRequest.CurrencyCode}</CurrencyCode > ";
                messageReturn += $"<DisplayCurrencyCode>{contextRequest.availabilityRequest.DisplayCurrencyCode}</DisplayCurrencyCode > ";
                messageReturn += $"<AvailabilityType>{contextRequest.availabilityRequest.AvailabilityType}</AvailabilityType > ";
                messageReturn += $"<SourceOrganization>{contextRequest.availabilityRequest.SourceOrganization}</SourceOrganization > ";
                messageReturn += $"<MaximumConnectingFlights>{contextRequest.availabilityRequest.MaximumConnectingFlights}</MaximumConnectingFlights > ";
                messageReturn += $"<AvailabilityFilter>{contextRequest.availabilityRequest.AvailabilityFilter}</AvailabilityFilter > ";
                messageReturn += $"<FareClassControl>{contextRequest.availabilityRequest.FareClassControl}</FareClassControl > ";
                messageReturn += $"<InboundOutbound>{contextRequest.availabilityRequest.InboundOutbound}</InboundOutbound > ";
                messageReturn += $"<SSRCollectionsMode>None</SSRCollectionsMode>";
                messageReturn += $"<NightsStay>{contextRequest.availabilityRequest.NightsStay}</NightsStay > ";
                messageReturn += $"<IncludeAllotments>{contextRequest.availabilityRequest.IncludeAllotments.ToString().ToLower()}</IncludeAllotments > ";
                messageReturn += "<BeginTime>";
                messageReturn += $"<com:TotalMinutes>{contextRequest.availabilityRequest.BeginTime.TotalMinutes}</com:TotalMinutes > ";
                messageReturn += "</BeginTime>";
                messageReturn += "<EndTime>";
                messageReturn += $"<com:TotalMinutes>{contextRequest.availabilityRequest.EndTime.TotalMinutes}</com:TotalMinutes > ";
                messageReturn += "</EndTime>";
                messageReturn += "<PaxPriceTypes>";

                foreach (var paxType in contextRequest.availabilityRequest.PaxPriceTypes)
                {
                    messageReturn += $"<itin:PaxPriceType>";
                    messageReturn += $"<itin:PaxType>{paxType.PaxType}</itin:PaxType>";
                    messageReturn += $"</itin:PaxPriceType>";
                }

                messageReturn += "</PaxPriceTypes>";
                messageReturn += "</AvailabilityRequest>";
            }


            messageReturn += "</book1:AvailabilityRequests>";
            messageReturn += "</book:tripAvailabilityRequest>";

            messageReturn += "</book:GetAvailabilityByTrip>";
            messageReturn += "</soapenv:Body>";
            messageReturn += "</soapenv:Envelope>";

            return messageReturn;
        }

    }
}
