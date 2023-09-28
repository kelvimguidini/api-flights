using AutoMapper;
using AzulWS;
using flights.crosscutting.Utils;
using flights.domain.Interfaces.Providers.Azul;
using flights.domain.Models;
using flights.domain.Models.Booking;
using flights.domain.Models.GetPrice;
using flights.domain.Models.Provider.Azul;
using flights.provider.azul.AppConfig;
using flights.provider.azul.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace flights.provider.Azul.Services
{
    public class BookingAzulService : IBookingAzulService
    {
        private readonly IMapper _mapper;
        private AppConfiguration _appConfiguration;

        public BookingAzulService(IMapper mapper)
        {
            _mapper = mapper;
            _appConfiguration = new AppConfiguration();
        }

        private readonly BookingManagerClientClient _webservice = new BookingManagerClientClient(BookingManagerClientClient.EndpointConfiguration.SecureEndpoint_BM, new crosscutting.AppConfig.AppConfiguration("EndPoints", "Azul").Configuration);

        public string PriceItineraryByKeys(SessionProvider session, flights.crosscutting.DomainObjects.PriceItineraryRequestWithKeys request)
        {
            try
            {
                var _sessionContext = ((LogonResponse)session.Session).SessionContext;
                var _objectContext = new SessionContextRequest();
                _objectContext.session = _mapper.Map<SessionAzulRequest>(_sessionContext);

                var url = _appConfiguration.UrlSVCBaseAzul;
                var soapUrl = _appConfiguration.SoapURLPriceItineraryByKeys;

                var stringXmlRequest = BuildStringXmlRequestPriceItinerary(_objectContext, request);
                var xmlStringResult = WCFXMLManager.ResponseWCF(url, soapUrl, stringXmlRequest, "s:");

                xmlStringResult = xmlStringResult.Replace("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><PriceItineraryByKeysResponse >", "");
                xmlStringResult = xmlStringResult.Replace("</PriceItineraryByKeysResponse></s:Body></s:Envelope>", "");

                return xmlStringResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro obter tarifação Azul: " + ex.Message);
            }
        }


        public BookingRS SellByKey(SessionProvider session, SellWithKeyRequest sellRequest)
        {
            try
            {
                var _sessionContext = ((LogonResponse)session.Session).SessionContext;
                var _objectContext = new SessionContextRequest();
                _objectContext.session = _mapper.Map<SessionAzulRequest>(_sessionContext);

                var url = _appConfiguration.UrlSVCBaseAzul;
                var soapUrl = _appConfiguration.SoapURLSellByKey;

                var stringXmlRequest = BuildStringXmlRequestSellByKey(_objectContext, sellRequest);
                var xmlStringResult = WCFXMLManager.ResponseWCF(url, soapUrl, stringXmlRequest, "s:");

                xmlStringResult = xmlStringResult.Replace("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><SellByKeyResponse >", "");
                xmlStringResult = xmlStringResult.Replace("</SellByKeyResponse></s:Body></s:Envelope>", "");

                var stringXmlRequestCommit = BuildStringXmlRequestCommit(_objectContext, sellRequest);
                var xmlStringResultCommit = WCFXMLManager.ResponseWCF(url, _appConfiguration.SoapURLCommitSellByKey, stringXmlRequestCommit, "s:");


                xmlStringResultCommit = xmlStringResultCommit.Replace("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><CommitResponse >", "");
                xmlStringResultCommit = xmlStringResultCommit.Replace("</CommitResponse></s:Body></s:Envelope>", "");

                var objRetorno = new BookingRS();

                objRetorno.Booking = xmlStringResultCommit;

                return objRetorno;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro obter reserva Azul: " + ex.Message);
            }
        }

        public SellWithKeyRequest GenerateSellRequest(crosscutting.DomainObjects.PriceItineraryRequestWithKeys priceRQAzul)
        {
            var sellWithKeyRequest = new SellWithKeyRequest();

            sellWithKeyRequest.PaxCount = priceRQAzul.PaxCount;
            sellWithKeyRequest.CurrencyCode = priceRQAzul.CurrencyCode;
            sellWithKeyRequest.PaxResidentCountry = priceRQAzul.PaxResidentCountry;
            sellWithKeyRequest.SellKeyList = priceRQAzul.PriceKeys.Select(x => new domain.Models.Provider.Azul.SellKeys { FareSellKey = x.FareSellKey, JourneySellKey = x.JourneySellKey }).ToList();

            var _PaxPriceTypes = new List<domain.Models.Provider.Azul.PaxPriceType>();

            priceRQAzul.Passengers.ForEach(x =>
            {
                foreach (var pax in x.PaxPriceType)
                {
                    var paxPriceType = new domain.Models.Provider.Azul.PaxPriceType();
                    paxPriceType.PaxType = pax.PaxType;

                    paxPriceType.FirstName = x.FirstName;
                    paxPriceType.LastName = x.LastName;
                    paxPriceType.PtcFlights = x.PtcFlights;
                    paxPriceType.DateBirth = x.DateBirth;
                    paxPriceType.DocType = x.DocType;
                    paxPriceType.DocNumber = x.DocNumber;
                    paxPriceType.Email = x.Email;
                    paxPriceType.PhoneType = x.PhoneType;
                    paxPriceType.PhoneCountryCode = x.PhoneCountryCode;
                    paxPriceType.PhoneNumber = x.PhoneNumber;
                    paxPriceType.PhoneLocalCode = x.PhoneLocalCode;
                    _PaxPriceTypes.Add(paxPriceType);
                }
            });

            sellWithKeyRequest.PaxPriceTypes = _PaxPriceTypes;

            return sellWithKeyRequest;
        }

        private string BuildStringXmlRequestPriceItinerary(SessionContextRequest contextRequest, flights.crosscutting.DomainObjects.PriceItineraryRequestWithKeys request)
        {
            var messageReturn = "";

            messageReturn += $"<s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/'>";
            messageReturn += $"<s:Body xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>";
            messageReturn += $"<PriceItineraryByKeys xmlns='http://schemas.navitaire.com/ClientServices/BookingManager/BookingManagerClient'>";
            messageReturn += $"<session>";
            #region Parametros Session Extras (desnecessarios)

            #endregion
            messageReturn += $"<SecureToken xmlns='http://schemas.navitaire.com/Common'>{contextRequest.session.SecureToken}</SecureToken>";
            messageReturn += $"</session>";

            messageReturn += $"<priceItineraryRequestWithKeys>";

            messageReturn += $"<CurrencyCode xmlns='http://schemas.navitaire.com/Messages/Booking/Request'>BRL</CurrencyCode>";
            messageReturn += $"<PaxResidentCountry xmlns='http://schemas.navitaire.com/Messages/Booking/Request'>BR</PaxResidentCountry>";

            messageReturn += $"<PriceKeys xmlns='http://schemas.navitaire.com/Messages/Booking/Request'>";

            foreach (var item in request.PriceKeys)
            {
                messageReturn += $"<PriceKeys>";
                messageReturn += $"<JourneySellKey xmlns='http://schemas.navitaire.com/Messages/Booking'>{item.JourneySellKey}</JourneySellKey>";
                messageReturn += $"<FareSellKey xmlns='http://schemas.navitaire.com/Messages/Booking'>{item.FareSellKey}</FareSellKey>";
                messageReturn += $"</PriceKeys>";
            }

            messageReturn += $"</PriceKeys>";
            messageReturn += $"<Passengers xmlns='http://schemas.navitaire.com/Messages/Booking/Request'>";

            foreach (var item in request.Passengers)
            {
                messageReturn += $"<Passenger xmlns='http://schemas.navitaire.com/Messages/Booking'>";
                messageReturn += $"<PassengerNumber>{item.PassengerNumber}</PassengerNumber>";

                foreach (var PaxPriceType in item.PaxPriceType)
                {
                    messageReturn += $"<PaxPriceType xmlns='http://schemas.navitaire.com/Messages/Itinerary'>";
                    messageReturn += $"<PaxType>{PaxPriceType.PaxType}</PaxType>";
                    messageReturn += $"</PaxPriceType>";
                }

                messageReturn += $"</Passenger>";
            }

            messageReturn += $"</Passengers>";

            messageReturn += $"</priceItineraryRequestWithKeys>";
            messageReturn += $"</PriceItineraryByKeys>";
            messageReturn += $"</s:Body>";
            messageReturn += $"</s:Envelope>";

            return messageReturn;
        }

        private string BuildStringXmlRequestSellByKey(SessionContextRequest contextRequest, SellWithKeyRequest request)
        {
            var messageReturn = "";

            messageReturn += $"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:book='http://schemas.navitaire.com/ClientServices/BookingManager/BookingManagerClient' xmlns:com='http://schemas.navitaire.com/Common' xmlns:book1='http://schemas.navitaire.com/Messages/Booking' xmlns:itin='http://schemas.navitaire.com/Messages/Itinerary'>";
            messageReturn += $"<soapenv:Header/>";
            messageReturn += $"<soapenv:Body>";
            messageReturn += $"<book:SellByKey>";
            messageReturn += $"<book:session>";

            messageReturn += $"<com:SecureToken>{contextRequest.session.SecureToken}</com:SecureToken>";

            messageReturn += $"</book:session>";

            messageReturn += $"<book:sellWithKeyRequest>";
            messageReturn += $"<book1:SellKeyList>";

            foreach (var item in request.SellKeyList)
            {
                messageReturn += $"<book1:SellKeys>";
                messageReturn += $"<book1:JourneySellKey>{item.JourneySellKey}</book1:JourneySellKey>";
                messageReturn += $"<book1:FareSellKey>{item.FareSellKey}</book1:FareSellKey>";
                //messageReturn += $"<book1:StandbyPriorityCode>?</book1:StandbyPriorityCode>";
                messageReturn += $"</book1:SellKeys>";
            }

            messageReturn += $"</book1:SellKeyList>";
            messageReturn += $"<book1:PaxPriceTypes>";

            foreach (var item in request.PaxPriceTypes)
            {
                messageReturn += $"<itin:PaxPriceType>";
                messageReturn += $"<itin:PaxType>{item.PaxType}</itin:PaxType>";
                //messageReturn += $"<itin:PaxDiscountCode>?</itin:PaxDiscountCode>";
                messageReturn += $"</itin:PaxPriceType>";
            }

            messageReturn += $"</book1:PaxPriceTypes>";
            messageReturn += $"<book1:ActionStatusCode>{request.ActionStatusCode}</book1:ActionStatusCode>";
            messageReturn += $"<book1:CurrencyCode>{request.CurrencyCode}</book1:CurrencyCode>";
            //messageReturn += $"<book1:SourceOrganization>{}</book1:SourceOrganization>";
            messageReturn += $"<book1:PaxCount>{request.PaxCount}</book1:PaxCount>";
            //messageReturn += $"<book1:PromoCode>?</book1:PromoCode>";
            messageReturn += $"<book1:PaxResidentCountry>{request.PaxResidentCountry}</book1:PaxResidentCountry>";
            messageReturn += $"</book:sellWithKeyRequest>";
            messageReturn += $"</book:SellByKey>";
            messageReturn += $"</soapenv:Body>";
            messageReturn += $"</soapenv:Envelope>";

            return messageReturn;
        }


        private string BuildStringXmlRequestCommit(SessionContextRequest contextRequest, SellWithKeyRequest request)
        {
            var messageReturn = "";

            messageReturn += $"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            messageReturn += $"<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
            messageReturn += $"<Commit xmlns=\"http://schemas.navitaire.com/ClientServices/BookingManager/BookingManagerClient\">";

            messageReturn += $"<session>";
            #region Parametros Session Extras (desnecessarios)

            #endregion
            messageReturn += $"<SecureToken xmlns='http://schemas.navitaire.com/Common'>{contextRequest.session.SecureToken}</SecureToken>";
            messageReturn += $"</session>";

            messageReturn += $"<bookingRequest>";
            messageReturn += $"<CurrencyCode xmlns=\"http://schemas.navitaire.com/Messages/Booking\">{request.CurrencyCode}</CurrencyCode>";
            messageReturn += $"<ReceivedBy xmlns=\"http://schemas.navitaire.com/Messages/Booking\">{request.PaxPriceTypes.First()?.FirstName + request.PaxPriceTypes.First()?.LastName}</ReceivedBy>";
            messageReturn += $"<PaxResidentCountry xmlns=\"http://schemas.navitaire.com/Messages/Booking\">{request.PaxResidentCountry}</PaxResidentCountry>";
            messageReturn += $"<RestrictionOverride xmlns=\"http://schemas.navitaire.com/Messages/Booking\">false</RestrictionOverride>";
            messageReturn += $"<DistributeToContacts xmlns=\"http://schemas.navitaire.com/Messages/Booking\">true</DistributeToContacts>";
            messageReturn += $"<WaiveNameChangeFee xmlns=\"http://schemas.navitaire.com/Messages/Booking\">false</WaiveNameChangeFee>";

            messageReturn += $"<BookingContacts xmlns=\"http://schemas.navitaire.com/Messages/Booking\">";

            string msgPax = string.Empty;


            foreach (var item in request.PaxPriceTypes)
            {
                messageReturn += $"<BookingContact>";
                messageReturn += $"<EmailAddress>{item.Email}</EmailAddress>";
                messageReturn += $"<TypeCode>72</TypeCode>";
                messageReturn += $"<HomePhone>{item.PhoneNumber}</HomePhone>";
                messageReturn += $"<Name xmlns=\"http://schemas.navitaire.com/Messages/Common\">";
                messageReturn += $"<FirstName>{item.FirstName}</FirstName>";
                messageReturn += $"<LastName>{item.LastName}</LastName>";
                messageReturn += $"</Name>";
                messageReturn += $"<AddressLine1>Av. Domingos de Moraes, 3333, apto 3333</AddressLine1>";
                messageReturn += $"<AddressLine2>Liberdade</AddressLine2>";
                messageReturn += $"<City>São Paulo</City>";
                messageReturn += $"<HomePhone>{item.PhoneNumber}</HomePhone>";
                messageReturn += $"</BookingContact>";

                msgPax += $"<BookingPassenger>";
                msgPax += $"<Name xmlns=\"http://schemas.navitaire.com/Messages/Common\">";
                msgPax += $"<FirstName>{item.FirstName}</FirstName>";
                msgPax += $"<LastName>{item.LastName}</LastName>";
                msgPax += $"</Name>";
                msgPax += $"<PaxPriceType xmlns=\"http://schemas.navitaire.com/Messages/Itinerary\">";
                msgPax += $"<PaxType>{item.PaxType}</PaxType>";
                msgPax += $"</PaxPriceType>";
                msgPax += $"<Gender>Male</Gender>";
                msgPax += $"<ResidentCountry>{request.PaxResidentCountry}</ResidentCountry>";
                msgPax += $"</BookingPassenger>";
            }

            messageReturn += $"</BookingContacts>";

            messageReturn += $"<BookingPassengers xmlns=\"http://schemas.navitaire.com/Messages/Booking\">";
            messageReturn += $"{msgPax}";
            messageReturn += $"</BookingPassengers>";

            messageReturn += $"</bookingRequest>";

            messageReturn += $"</Commit>";
            messageReturn += $"</s:Body>";
            messageReturn += $"</s:Envelope>";
            
            return messageReturn;
        }
    }
}



