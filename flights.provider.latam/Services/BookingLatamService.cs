using AutoMapper;
using flights.crosscutting.AppConfig;
using flights.crosscutting.Utils;
using flights.domain.Entities;
using flights.domain.Interfaces.Providers.Latam;
using flights.domain.Models;
using flights.domain.Models.GetPrice;
using KissLog;
using LatamWS.Homolog.EnhancedAirBook;
using LatamWS.Homolog.GetReservation;
using LatamWS.Homolog.PassengerDetails;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace flights.provider.latam.Services
{
    public class BookingLatamService : IBookingLatamService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;


        public BookingLatamService(
            IMapper mapper,
            ILogger logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        private readonly EnhancedAirBookPortType _webservice = new EnhancedAirBookPortTypeClient(EnhancedAirBookPortTypeClient.EndpointConfiguration.EnhancedAirBookPortType, new AppConfiguration("EndPoints", "Latam").Configuration);
        private readonly PassengerDetailsPortType _webservicePaxDetails = new PassengerDetailsPortTypeClient(PassengerDetailsPortTypeClient.EndpointConfiguration.PassengerDetailsPortType, new AppConfiguration("EndPoints", "Latam").Configuration);
        private readonly GetReservationPortType _webserviceReservation = new GetReservationPortTypeClient(GetReservationPortTypeClient.EndpointConfiguration.GetReservationPortType, new AppConfiguration("EndPoints", "Latam").Configuration);

        public string AirBook(GetPriceRQ priceRQ, string security, List<CredentialParameters> credentialParameters, SessionProvider session)
        {
            AddPersonName(priceRQ, security, credentialParameters, session);

            EnhancedAirBook(priceRQ, security, credentialParameters, session);

            TicketTimeLimit(priceRQ, security, credentialParameters, session);

            var t = AddMultiElements(priceRQ, security, credentialParameters, session);

            return t.PassengerDetailsRS.ItineraryRef.ID;
        }

        private void EnhancedAirBook(GetPriceRQ priceRQ, string security, List<CredentialParameters> credentialParameters, SessionProvider session)
        {

            string messageReturn;

            messageReturn = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:sec=\"http://schemas.xmlsoap.org/ws/2002/12/secext\" xmlns:mes=\"http://www.ebxml.org/namespaces/messageHeader\" xmlns:v3=\"http://services.sabre.com/sp/eab/v3_7\">";
            messageReturn += $" <soapenv:Header>";
            messageReturn += $"     <sec:Security>";
            messageReturn += $"         <sec:BinarySecurityToken>{security}</sec:BinarySecurityToken>";
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
            messageReturn += $"         <mes:Service mes:type=\"SabreXML\">AirBook</mes:Service>";
            messageReturn += $"         <mes:Action>EnhancedAirBookRQ</mes:Action>";
            messageReturn += $"         <mes:MessageData>";

            var header = session.Session.GetType().GetProperty("MessageHeader").GetValue(session.Session, null);
            var mData = header.GetType().GetProperty("MessageData").GetValue(header, null);

            messageReturn += $"             <mes:MessageId>{mData.GetType().GetProperty("MessageId").GetValue(mData, null).ToString()}</mes:MessageId>";
            messageReturn += $"             <mes:Timestamp>{DateTime.Now.ToString("s")}</mes:Timestamp>";
            messageReturn += $"         </mes:MessageData>";
            messageReturn += $"         <mes:Description xml:lang=\"en-us\"/>";
            messageReturn += $"     </mes:MessageHeader>";
            messageReturn += $" </soapenv:Header>";
            messageReturn += $" <soapenv:Body>";
            messageReturn += $"     <v3:EnhancedAirBookRQ version=\"3.7.0\" HaltOnError=\"true\" IgnoreOnError=\"false\">";
            messageReturn += $"         <v3:OTA_AirBookRQ>";
            messageReturn += $"             <v3:HaltOnStatus Code=\"NO\"/>";
            messageReturn += $"             <v3:HaltOnStatus Code=\"NN\"/>";
            messageReturn += $"             <v3:HaltOnStatus Code=\"UC\"/>";
            messageReturn += $"             <v3:HaltOnStatus Code=\"US\"/>";
            messageReturn += $"             <v3:OriginDestinationInformation>";

            foreach (var jorney in priceRQ.JorneysSell.OrderBy(x => x.ArrivalDateTime))
            {
                messageReturn += $"             <v3:FlightSegment DepartureDateTime=\"{jorney.DepartureDateTime}\" FlightNumber=\"{jorney.FlightNumber}\" NumberInParty=\"{priceRQ.Passagers.Count.ToString()}\" ResBookDesigCode=\"{jorney.ResBookDesigCode}\" Status=\"{jorney.Status}\" BrandID=\"{jorney.BrandID}\">";
                messageReturn += $"                 <v3:DestinationLocation LocationCode=\"{jorney.LocationCodeDestiation}\" />";
                messageReturn += $"                 <v3:MarketingAirline Code=\"{jorney.MarketingCode}\" FlightNumber=\"{jorney.FlightNumber}\"/>";
                messageReturn += $"                 <v3:MarriageGrp>{jorney.MarriageGrp}</v3:MarriageGrp>";
                messageReturn += $"                 <v3:OperatingAirline Code=\"{jorney.MarketingCode}\"/>";
                messageReturn += $"                 <v3:OriginLocation LocationCode=\"{jorney.LocationCodeOrigin}\"/>";
                messageReturn += $"             </v3:FlightSegment>";
            }

            messageReturn += $"             </v3:OriginDestinationInformation>";
            messageReturn += $"             <v3:RedisplayReservation NumAttempts=\"2\" WaitInterval=\"5000\"/>";
            messageReturn += $"         </v3:OTA_AirBookRQ>";
            messageReturn += $"         <v3:OTA_AirPriceRQ>";
            messageReturn += $"             <v3:PriceRequestInformation Retain=\"true\">";
            messageReturn += $"                 <v3:OptionalQualifiers>";
            messageReturn += $"                     <v3:PricingQualifiers>";
            messageReturn += $"                         <v3:BargainFinder Rebook=\"true\"/>";
            messageReturn += $"                         <v3:Brand>{priceRQ.JorneysSell.FirstOrDefault().BrandID}</v3:Brand>";

            foreach (var pax in priceRQ.Passagers.GroupBy(x => x.PaxType))
            {
                string type;
                if (pax.Key == "CHD")
                {
                    type = "CNN";
                }
                else
                {
                    type = pax.Key;
                }
                messageReturn += $"                     <v3:PassengerType Code=\"{type}\" Force=\"false\" Quantity=\"{pax.Count()}\"/>";
            }

            messageReturn += $"                     </v3:PricingQualifiers>";
            messageReturn += $"                 </v3:OptionalQualifiers>";
            messageReturn += $"             </v3:PriceRequestInformation>";
            messageReturn += $"         </v3:OTA_AirPriceRQ>";
            messageReturn += $"         <v3:PostProcessing>";
            messageReturn += $"             <v3:RedisplayReservation />";
            messageReturn += $"         </v3:PostProcessing>";
            messageReturn += $"     </v3:EnhancedAirBookRQ>";
            messageReturn += $" </soapenv:Body>";
            messageReturn += $"</soapenv:Envelope>";

            var xmlStringResult = WCFXMLManager.ResponseWCF(new AppConfiguration("EndPoints", "Latam").Configuration, "", messageReturn, "s:");
            _logger.Log(KissLog.LogLevel.Debug, messageReturn + "\n" + xmlStringResult, "LATAM");
        }

        private void AddPersonName(GetPriceRQ priceRQ, string security, List<CredentialParameters> credentialParameters, SessionProvider session)
        {

            var partIdFrom = new LatamWS.Homolog.PassengerDetails.PartyId[1];
            partIdFrom[0] = new LatamWS.Homolog.PassengerDetails.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value
            };
            var partIdTo = new LatamWS.Homolog.PassengerDetails.PartyId[1];
            partIdTo[0] = new LatamWS.Homolog.PassengerDetails.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value
            };

            var contactNumber = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoContactNumber[priceRQ.Passagers.Count + 1];
            var email = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoEmail[priceRQ.Passagers.Count];
            var person = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoPersonName[priceRQ.Passagers.Count];

            contactNumber[0] = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoContactNumber
            {
                Phone = credentialParameters.Where(q => q.Parameter == "OfficeCode").FirstOrDefault()?.Value,
                PhoneUseType = ""
            };

            int i = 0;
            foreach (var pax in priceRQ.Passagers)
            {
                if (pax.PaxType == "CHD")
                {
                    if ((Convert.ToDateTime(priceRQ.JorneysSell.OrderBy(x => x.ArrivalDateTime).LastOrDefault()?.ArrivalDateTime) - pax.DateBirth).TotalDays < (2 * 365))
                    {
                        pax.PaxType = "INF";
                    }
                    else
                    {
                        pax.PaxType = "CNN";
                    }
                }
                contactNumber[i + 1] = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoContactNumber
                {
                    Phone = pax.PhoneNumber,
                    PhoneUseType = pax.PhoneType == "CELL_PHONE" ? "M" : "A",
                    LocationCode = pax.PhoneLocalCode
                };
                email[i] = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoEmail
                {
                    Address = pax.Email,
                    Type = PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoEmailType.TO
                };
                person[i] = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoPersonName
                {
                    NameNumber = (i + 1).ToString() + ".1",
                    GivenName = pax.FirstName,
                    Surname = pax.LastName,
                    Infant = (Convert.ToDateTime(priceRQ.JorneysSell.OrderBy(x => x.ArrivalDateTime).LastOrDefault()?.ArrivalDateTime) - pax.DateBirth).TotalDays < (2 * 365),
                    NameReference = pax.PaxType + "-" + pax.DateBirth.ToString("ddMMMyy").ToUpper()
                };

                i++;
            }
            var header = session.Session.GetType().GetProperty("MessageHeader").GetValue(session.Session, null);
            var mData = header.GetType().GetProperty("MessageData").GetValue(header, null);


            PassengerDetailsRQRequest passengerDetailsRQRequest = new PassengerDetailsRQRequest
            {
                MessageHeader = new LatamWS.Homolog.PassengerDetails.MessageHeader
                {
                    From = new LatamWS.Homolog.PassengerDetails.From
                    {
                        PartyId = partIdFrom
                    },
                    To = new LatamWS.Homolog.PassengerDetails.To
                    {
                        PartyId = partIdTo
                    },
                    Action = "PassengerDetailsRQ",
                    CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
                    ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
                    Service = new LatamWS.Homolog.PassengerDetails.Service()
                    {
                        type = "SabreXML",
                        Value = "AddPersonName"
                    },
                    MessageData = new LatamWS.Homolog.PassengerDetails.MessageData
                    {
                        MessageId = mData.GetType().GetProperty("MessageId").GetValue(mData, null).ToString(),
                        Timestamp = DateTime.Now.ToString("s")
                    }
                },
                Security = new LatamWS.Homolog.PassengerDetails.Security
                {
                    BinarySecurityToken = security
                },

                PassengerDetailsRQ = new PassengerDetailsRQ
                {
                    version = "3.3.0",
                    HaltOnError = true,
                    IgnoreOnError = true,
                    PostProcessing = new PassengerDetailsRQPostProcessing
                    {
                        IgnoreAfter = false,
                        RedisplayReservation = true
                    },
                    TravelItineraryAddInfoRQ = new PassengerDetailsRQTravelItineraryAddInfoRQ
                    {
                        AgencyInfo = new PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfo
                        {
                            Address = new PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoAddress
                            {
                                AddressLine = "Rua dos Antúrios",
                                CityName = "Sao Paulo",
                                CountryCode = "BR",
                                PostalCode = "72215215", //código de idioma
                                StateCountyProv = new PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoAddressStateCountyProv
                                {
                                    StateCode = "SP"
                                },
                                StreetNmbr = "256 Jr. Real"
                            }
                        },
                        CustomerInfo = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfo
                        {
                            ContactNumbers = contactNumber,
                            Email = email,
                            PersonName = person
                        }

                    }
                }
            };

            var xml = WCFXMLManager.SerializeObjectToXml<PassengerDetailsRQRequest>(passengerDetailsRQRequest);

            var t = _webservicePaxDetails.PassengerDetailsRQAsync(passengerDetailsRQRequest).Result;
            var xmlR = WCFXMLManager.SerializeObjectToXml<PassengerDetailsRQResponse>(t);
            _logger.Log(KissLog.LogLevel.Debug, xml + "\n" + xmlR, "LATAM");
        }

        //private void Ctc(GetPriceRQ priceRQ, string security, List<CredentialParameters> credentialParameters, SessionProvider session)
        //{

        //    var partIdFrom = new LatamWS.Homolog.PassengerDetails.PartyId[1];
        //    partIdFrom[0] = new LatamWS.Homolog.PassengerDetails.PartyId
        //    {
        //        type = "URI",
        //        Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value
        //    };
        //    var partIdTo = new LatamWS.Homolog.PassengerDetails.PartyId[1];
        //    partIdTo[0] = new LatamWS.Homolog.PassengerDetails.PartyId
        //    {
        //        type = "URI",
        //        Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value
        //    };

        //    var header = session.Session.GetType().GetProperty("MessageHeader").GetValue(session.Session, null);
        //    var mData = header.GetType().GetProperty("MessageData").GetValue(header, null);


        //    PassengerDetailsRQRequest passengerDetailsRQRequest = new PassengerDetailsRQRequest
        //    {
        //        MessageHeader = new LatamWS.Homolog.PassengerDetails.MessageHeader
        //        {
        //            From = new LatamWS.Homolog.PassengerDetails.From
        //            {
        //                PartyId = partIdFrom
        //            },
        //            To = new LatamWS.Homolog.PassengerDetails.To
        //            {
        //                PartyId = partIdTo
        //            },
        //            Action = "PassengerDetailsRQ",
        //            CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
        //            ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
        //            Service = new LatamWS.Homolog.PassengerDetails.Service()
        //            {
        //                type = "SabreXML",
        //                Value = "PassengerDetailsRQ"
        //            },
        //            MessageData = new LatamWS.Homolog.PassengerDetails.MessageData
        //            {
        //                MessageId = mData.GetType().GetProperty("MessageId").GetValue(mData, null).ToString(),
        //                Timestamp = DateTime.Now.ToString("s")
        //            }
        //        },
        //        Security = new LatamWS.Homolog.PassengerDetails.Security
        //        {
        //            BinarySecurityToken = security
        //        },

        //        PassengerDetailsRQ = new PassengerDetailsRQ
        //        {
        //            version = "3.3.0",
        //            HaltOnError = true,
        //            IgnoreOnError = true,
        //            PostProcessing = new PassengerDetailsRQPostProcessing
        //            {
        //                IgnoreAfter = false,
        //                RedisplayReservation = true
        //            },
        //            SpecialReqDetails = new PassengerDetailsRQSpecialReqDetails
        //            {
        //                SpecialServiceRQ = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQ
        //                {
        //                    SpecialServiceInfo = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfo
        //                    {
        //                        Service = service
        //                    }
        //                }
        //            },
        //        }
        //    };

        //    var xml = WCFXMLManager.SerializeObjectToXml<PassengerDetailsRQRequest>(passengerDetailsRQRequest);

        //    var t = _webservicePaxDetails.PassengerDetailsRQAsync(passengerDetailsRQRequest).Result;
        //    var xmlR = WCFXMLManager.SerializeObjectToXml<PassengerDetailsRQResponse>(t);
        //    _logger.Log(KissLog.LogLevel.Debug, xml + "\n" + xmlR, "LATAM");
        //}

        private void GetReservation(GetPriceRQ priceRQ, string security, List<CredentialParameters> credentialParameters, SessionProvider session, String locator)
        {

            var partIdFrom = new LatamWS.Homolog.GetReservation.PartyId[1];
            partIdFrom[0] = new LatamWS.Homolog.GetReservation.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value
            };
            var partIdTo = new LatamWS.Homolog.GetReservation.PartyId[1];
            partIdTo[0] = new LatamWS.Homolog.GetReservation.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value
            };

            var areas = new string[1];
            areas[0] = "PRICE_QUOTE";


            var header = session.Session.GetType().GetProperty("MessageHeader").GetValue(session.Session, null);
            var mData = header.GetType().GetProperty("MessageData").GetValue(header, null);


            GetReservationOperationRequest getReservationRQRequest = new GetReservationOperationRequest
            {
                MessageHeader = new LatamWS.Homolog.GetReservation.MessageHeader
                {
                    From = new LatamWS.Homolog.GetReservation.From
                    {
                        PartyId = partIdFrom
                    },
                    To = new LatamWS.Homolog.GetReservation.To
                    {
                        PartyId = partIdTo
                    },
                    Action = "getReservationRQ",
                    CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
                    ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
                    Service = new LatamWS.Homolog.GetReservation.Service()
                    {
                        type = "SabreXML",
                        Value = "pnrRetrieve"
                    },
                    MessageData = new LatamWS.Homolog.GetReservation.MessageData()
                    {
                        MessageId = mData.GetType().GetProperty("MessageId").GetValue(mData, null).ToString(),
                        Timestamp = DateTime.Now.ToString("s")
                    }
                },
                Security = new LatamWS.Homolog.GetReservation.Security
                {
                    BinarySecurityToken = security
                },

                GetReservationRQ = new GetReservationRQ
                {
                    Version = "3.3.0",
                    Locator = locator,
                    RequestType = "Stateful",
                    ReturnOptions = new ReturnOptionsPNRB
                    {
                        SubjectAreas = areas,
                        ShowTicketStatus = true,
                        PriceQuoteServiceVersion = "3.2.0",
                        ViewName = "Full",
                        ResponseFormat = "STL"
                    }
                }
            };

            var xml = WCFXMLManager.SerializeObjectToXml<GetReservationOperationRequest>(getReservationRQRequest);

            var t = _webserviceReservation.GetReservationOperationAsync(getReservationRQRequest).Result;
            var xmlR = WCFXMLManager.SerializeObjectToXml<GetReservationOperationResponse>(t);
            _logger.Log(KissLog.LogLevel.Debug, xml + "\n" + xmlR, "LATAM");
        }

        private void TicketTimeLimit(GetPriceRQ priceRQ, string security, List<CredentialParameters> credentialParameters, SessionProvider session)
        {

            var partIdFrom = new LatamWS.Homolog.PassengerDetails.PartyId[1];
            partIdFrom[0] = new LatamWS.Homolog.PassengerDetails.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value
            };
            var partIdTo = new LatamWS.Homolog.PassengerDetails.PartyId[1];
            partIdTo[0] = new LatamWS.Homolog.PassengerDetails.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value
            };


            var header = session.Session.GetType().GetProperty("MessageHeader").GetValue(session.Session, null);
            var mData = header.GetType().GetProperty("MessageData").GetValue(header, null);


            int qtdCTCE = priceRQ.Passagers.Where(x => x.EmergencyContact != null && !string.IsNullOrEmpty(x.EmergencyContact.Email)).Count();
            int qtdCTCM = priceRQ.Passagers.Where(x => x.EmergencyContact != null && !string.IsNullOrEmpty(x.EmergencyContact.PhoneNumber)).Count();
            int qtdCTCR = priceRQ.Passagers.Where(x => x.EmergencyContact == null || (string.IsNullOrEmpty(x.EmergencyContact.PhoneNumber) && string.IsNullOrEmpty(x.EmergencyContact.Email))).Count();

            var service = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoService[qtdCTCE + qtdCTCM + qtdCTCR];

            int i = 0;
            int x = 1;
            foreach (var pax in priceRQ.Passagers)
            {
                bool ctcr = true;

                if (pax.EmergencyContact != null && !string.IsNullOrEmpty(pax.EmergencyContact.Email))
                {
                    ctcr = false;
                    service[i] = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoService
                    {
                        SegmentNumber = "0001",
                        SSR_Code = "CTCE",
                        Text = pax.EmergencyContact.Email.Replace("@", "//"),
                        VendorPrefs = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefs
                        {
                            Airline = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefsAirline
                            {
                                Hosted = true
                            }
                        },
                        PersonName = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServicePersonName
                        {
                            NameNumber = x.ToString() + ".1"
                        }
                    };
                    i++;
                }

                if (pax.EmergencyContact != null && !string.IsNullOrEmpty(pax.EmergencyContact.PhoneNumber))
                {
                    service[i] = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoService
                    {
                        SegmentNumber = "0001",
                        SSR_Code = "CTCM",
                        Text = pax.EmergencyContact.PhoneCountryCode + " " + pax.EmergencyContact.PhoneLocalCode + " " + pax.EmergencyContact.PhoneNumber,
                        VendorPrefs = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefs
                        {
                            Airline = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefsAirline
                            {
                                Hosted = true
                            }
                        },
                        PersonName = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServicePersonName
                        {
                            NameNumber = x.ToString() + ".1"
                        }
                    };
                    ctcr = false;
                    i++;
                }

                if (ctcr)
                {
                    service[i] = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoService
                    {
                        SegmentNumber = "0001",
                        SSR_Code = "CTCR",
                        Text = "CONTATO RECUSADO",
                        VendorPrefs = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefs
                        {
                            Airline = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefsAirline
                            {
                                Hosted = true
                            }
                        },
                        PersonName = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServicePersonName
                        {
                            NameNumber = x.ToString() + ".1"
                        }
                    };
                    i++;
                }
                x++;
            }


            PassengerDetailsRQRequest passengerDetailsRQRequest = new PassengerDetailsRQRequest
            {
                MessageHeader = new LatamWS.Homolog.PassengerDetails.MessageHeader
                {
                    From = new LatamWS.Homolog.PassengerDetails.From
                    {
                        PartyId = partIdFrom
                    },
                    To = new LatamWS.Homolog.PassengerDetails.To
                    {
                        PartyId = partIdTo
                    },
                    Action = "PassengerDetailsRQ",
                    CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
                    ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
                    Service = new LatamWS.Homolog.PassengerDetails.Service()
                    {
                        type = "SabreXML",
                        Value = "TicketTimeLimit"
                    },
                    MessageData = new LatamWS.Homolog.PassengerDetails.MessageData()
                    {
                        MessageId = mData.GetType().GetProperty("MessageId").GetValue(mData, null).ToString(),
                        Timestamp = DateTime.Now.ToString("s")
                    }
                },
                Security = new LatamWS.Homolog.PassengerDetails.Security
                {
                    BinarySecurityToken = security
                },

                PassengerDetailsRQ = new PassengerDetailsRQ
                {
                    version = "3.3.0",
                    HaltOnError = false,
                    IgnoreOnError = false,
                    SpecialReqDetails = new PassengerDetailsRQSpecialReqDetails
                    {
                        SpecialServiceRQ = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQ
                        {
                            SpecialServiceInfo = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfo
                            {
                                Service = service
                            }
                        }
                    },
                    TravelItineraryAddInfoRQ = new PassengerDetailsRQTravelItineraryAddInfoRQ
                    {
                        AgencyInfo = new PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfo
                        {
                            Ticketing = new PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoTicketing
                            {
                                //TicketTimeLimit =  DateTime.Today.AddDays(2).AddHours(-2).ToString("s"),
                                TicketType = "82359/" + DateTime.Today.AddDays(2).ToString("dMMM", CultureInfo.CreateSpecificCulture("en-US"))
                            }
                        }
                    }
                }
            };

            var xml = WCFXMLManager.SerializeObjectToXml<PassengerDetailsRQRequest>(passengerDetailsRQRequest);

            var t = _webservicePaxDetails.PassengerDetailsRQAsync(passengerDetailsRQRequest).Result;
            var xmlR = WCFXMLManager.SerializeObjectToXml<PassengerDetailsRQResponse>(t);
            _logger.Log(KissLog.LogLevel.Debug, xml + "\n" + xmlR, "LATAM");
        }

        private PassengerDetailsRQResponse AddMultiElements(GetPriceRQ priceRQ, string security, List<CredentialParameters> credentialParameters, SessionProvider session)
        {

            var partIdFrom = new LatamWS.Homolog.PassengerDetails.PartyId[1];
            partIdFrom[0] = new LatamWS.Homolog.PassengerDetails.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value
            };
            var partIdTo = new LatamWS.Homolog.PassengerDetails.PartyId[1];
            partIdTo[0] = new LatamWS.Homolog.PassengerDetails.PartyId
            {
                type = "URI",
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value
            };


            var header = session.Session.GetType().GetProperty("MessageHeader").GetValue(session.Session, null);
            var mData = header.GetType().GetProperty("MessageData").GetValue(header, null);

            PassengerDetailsRQRequest passengerDetailsRQRequest = new PassengerDetailsRQRequest
            {
                MessageHeader = new LatamWS.Homolog.PassengerDetails.MessageHeader
                {
                    From = new LatamWS.Homolog.PassengerDetails.From
                    {
                        PartyId = partIdFrom
                    },
                    To = new LatamWS.Homolog.PassengerDetails.To
                    {
                        PartyId = partIdTo
                    },
                    Action = "PassengerDetailsRQ",
                    CPAId = credentialParameters.Where(q => q.Parameter == "CPAId").FirstOrDefault()?.Value,
                    ConversationId = credentialParameters.Where(q => q.Parameter == "ConversationId").FirstOrDefault()?.Value,
                    Service = new LatamWS.Homolog.PassengerDetails.Service()
                    {
                        type = "SabreXML",
                        Value = "AddMultiElements"
                    },
                    MessageData = new LatamWS.Homolog.PassengerDetails.MessageData
                    {
                        MessageId = mData.GetType().GetProperty("MessageId").GetValue(mData, null).ToString(),
                        Timestamp = DateTime.Now.ToString("s")
                    }
                },
                Security = new LatamWS.Homolog.PassengerDetails.Security
                {
                    BinarySecurityToken = security
                },

                PassengerDetailsRQ = new PassengerDetailsRQ
                {
                    version = "3.3.0",
                    HaltOnError = true,
                    IgnoreOnError = false,
                    PostProcessing = new PassengerDetailsRQPostProcessing
                    {
                        IgnoreAfter = false,
                        RedisplayReservation = false,
                        EndTransactionRQ = new PassengerDetailsRQPostProcessingEndTransactionRQ
                        {
                            EndTransaction = new PassengerDetailsRQPostProcessingEndTransactionRQEndTransaction
                            {
                                Ind = "true"
                            },
                            Source = new PassengerDetailsRQPostProcessingEndTransactionRQSource
                            {
                                ReceivedFrom = "Disolutty"
                            }
                        }
                    }
                }
            };


            var xml = WCFXMLManager.SerializeObjectToXml<PassengerDetailsRQRequest>(passengerDetailsRQRequest);

            var t = _webservicePaxDetails.PassengerDetailsRQAsync(passengerDetailsRQRequest).Result;

            var xmlR = WCFXMLManager.SerializeObjectToXml<PassengerDetailsRQResponse>(t);
            _logger.Log(KissLog.LogLevel.Debug, xml + "\n" + xmlR, "LATAM");
            return t;
        }
    }
}