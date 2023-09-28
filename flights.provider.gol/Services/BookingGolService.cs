using AutoMapper;
using flights.crosscutting.AppConfig;
using flights.crosscutting.Utils;
using flights.domain.Entities;
using flights.domain.Interfaces.Providers.Gol;
using flights.domain.Models;
using flights.domain.Models.Booking;
using flights.domain.Models.GetPrice;
using GolWs.gws.PassengerDetails;
using GolWS.gws.EnhancedAirBook;
using GolWS.Homolog.BookingManager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace flights.provider.gol.Services
{
    public class BookingGolService : IBookingGolService
    {
        private readonly IMapper _mapper;

        public BookingGolService(IMapper mapper)
        {
            _mapper = mapper;
        }

        private readonly BookingManagerClientClient _webservice = new BookingManagerClientClient(BookingManagerClientClient.EndpointConfiguration.BasicHttpBinding_IBookingManagerClient, new AppConfiguration("EndPoints", "GolBooking").Configuration);

        private readonly EnhancedAirBookRQServiceClient _webserviceGwsEnhancedAirBook =
            new EnhancedAirBookRQServiceClient(EnhancedAirBookRQServiceClient.EndpointConfiguration.BasicHttpBinding_IEnhancedAirBookRQService, new AppConfiguration("EndPoints", "Gol").Configuration + "EnhancedAirBookRQService.svc");
        
        private readonly PassengerDetailsRQServiceClient _webserviceGwsPassengerDetails =
            new PassengerDetailsRQServiceClient(PassengerDetailsRQServiceClient.EndpointConfiguration.BasicHttpBinding_IPassengerDetailsRQService, new AppConfiguration("EndPoints", "Gol").Configuration + "PassengerDetailsRQService.svc");

        #region GWS

        public string Sell(SessionProvider session, GetPriceRQ priceRQ, List<CredentialParameters> credentialParameters)
        {
            Enhanced(session, priceRQ, credentialParameters);
            var t = AddPersonName(priceRQ, session, credentialParameters);
            return t.PassengerDetailsRS.ItineraryRef.ID;
        }

        private void Enhanced(SessionProvider session, GetPriceRQ priceRQ, List<CredentialParameters> credentialParameters)
        {
            #region Cabecalho
            var sessionC = (GolWS.gws.SessionCreate.SessionCreateRQResponse)session.Session.GetType().GetProperty("Session").GetValue(session.Session, null);

            var from = new GolWS.gws.EnhancedAirBook.MessageHeaderFromPartyId[1];
            from[0] = new GolWS.gws.EnhancedAirBook.MessageHeaderFromPartyId
            {
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value,
                type = "URI"
            };

            var to = new GolWS.gws.EnhancedAirBook.MessageHeaderTOPartyId[1];
            to[0] = new GolWS.gws.EnhancedAirBook.MessageHeaderTOPartyId
            {
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value,
                type = "URI"
            };

            GolWS.gws.EnhancedAirBook.MessageHeader messageHeader = new GolWS.gws.EnhancedAirBook.MessageHeader
            {
                From = new GolWS.gws.EnhancedAirBook.MessageHeaderFrom
                {
                    PartyId = from
                },
                To = new GolWS.gws.EnhancedAirBook.MessageHeaderTO{
                    PartyId = to
                },
                CPAId = sessionC.MessageHeader.CPAId,
                ConversationId = sessionC.MessageHeader.ConversationId,
                Action = "EnhancedAirBookRQ",
                Service = new GolWS.gws.EnhancedAirBook.MessageHeaderService
                {
                    Value = "EnhancedAirBookRQ"
                },
                MessageData = new GolWS.gws.EnhancedAirBook.MessageHeaderMessageData
                {
                    MessageId = sessionC.MessageHeader.MessageData.MessageId,
                    Timestamp = DateTime.Now.AddHours(7).ToString("s")
                }
            };

            GolWS.gws.EnhancedAirBook.Security security = new GolWS.gws.EnhancedAirBook.Security
            {
                BinarySecurityToken = sessionC.Security.BinarySecurityToken
            };
            #endregion

            var paxType = new EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType[priceRQ.Passagers.GroupBy(x => x.PaxType).Count()];

            int i = 0;
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
                paxType[i] = new EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType
                {
                    Code = type,
                    Force = false,
                    Quantity = pax.Count().ToString()
                };

                i++;
            }

            var brand = new EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersBrand[1];
            brand[0] = new EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersBrand
            {
                Value = priceRQ.JorneysSell.FirstOrDefault().BrandID
            };

            var airPrice = new EnhancedAirBookRQOTA_AirPriceRQ[1];
            airPrice[0] = new EnhancedAirBookRQOTA_AirPriceRQ
            {
                PriceRequestInformation = new EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformation
                {
                    Retain = true,
                    OptionalQualifiers = new EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformationOptionalQualifiers
                    {
                        PricingQualifiers = new EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiers
                        {
                            CurrencyCode = "BRL",
                            Brand = brand,
                            PassengerType = paxType
                        }
                    }
                }
            };

            var origimDestination = new ArrayOfEnhancedAirBookRQOTA_AirBookRQFlightSegmentFlightSegment[priceRQ.JorneysSell.Count];

            int x = 0;
            foreach (var jorney in priceRQ.JorneysSell.OrderBy(x => x.ArrivalDateTime))
            {
                origimDestination[x] = new ArrayOfEnhancedAirBookRQOTA_AirBookRQFlightSegmentFlightSegment
                {
                    DepartureDateTime = jorney.DepartureDateTime,
                    FlightNumber = jorney.FlightNumber,
                    NumberInParty = priceRQ.Passagers.Count.ToString(),
                    ResBookDesigCode = jorney.ResBookDesigCode,
                    Status = jorney.Status,
                    InstantPurchase = false,
                    DestinationLocation = new ArrayOfEnhancedAirBookRQOTA_AirBookRQFlightSegmentFlightSegmentDestinationLocation
                    {
                        LocationCode = jorney.LocationCodeDestiation
                    },
                    MarketingAirline = new ArrayOfEnhancedAirBookRQOTA_AirBookRQFlightSegmentFlightSegmentMarketingAirline
                    {
                        Code = jorney.MarketingCode,
                        FlightNumber = jorney.FlightNumber
                    },
                    OriginLocation = new ArrayOfEnhancedAirBookRQOTA_AirBookRQFlightSegmentFlightSegmentOriginLocation
                    {
                        LocationCode = jorney.LocationCodeOrigin
                    },
                    OperatingAirline = new ArrayOfEnhancedAirBookRQOTA_AirBookRQFlightSegmentFlightSegmentOperatingAirline
                    {
                        Code = jorney.MarketingCode
                    }
                };
                x++;
            }

            var request = new EnhancedAirBookRQRequest
            {
                MessageHeader = messageHeader,
                Security = security,
                EnhancedAirBookRQ = new EnhancedAirBookRQ
                {
                    version = "3.10.0",
                    HaltOnError = true,
                    PostProcessing = new EnhancedAirBookRQPostProcessing
                    {
                        IgnoreAfter = false
                    },
                    PreProcessing = new EnhancedAirBookRQPreProcessing
                    {
                        IgnoreBefore = false
                    },
                    OTA_AirBookRQ = new EnhancedAirBookRQOTA_AirBookRQ
                    {
                        OriginDestinationInformation = origimDestination
                    },
                    OTA_AirPriceRQ = airPrice
                }
            };

            var xml = WCFXMLManager.SerializeObjectToXml<EnhancedAirBookRQRequest>(request);

            var result = _webserviceGwsEnhancedAirBook.EnhancedAirBookRQAsync(request.Security, request.MessageHeader, request.EnhancedAirBookRQ).Result;
            var xmlR = WCFXMLManager.SerializeObjectToXml<EnhancedAirBookRQResponse>(result);
        }

        private PassengerDetailsRQResponse AddPersonName(GetPriceRQ priceRQ, SessionProvider session, List<CredentialParameters> credentialParameters)
        {
            #region Cabecalho

            var sessionC = (GolWS.gws.SessionCreate.SessionCreateRQResponse)session.Session.GetType().GetProperty("Session").GetValue(session.Session, null);

            var from = new GolWs.gws.PassengerDetails.MessageHeaderFromPartyId[1];
            from[0] = new GolWs.gws.PassengerDetails.MessageHeaderFromPartyId
            {
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdFrom").FirstOrDefault()?.Value,
                type = "URI"
            };

            var to = new GolWs.gws.PassengerDetails.MessageHeaderTOPartyId[1];
            to[0] = new GolWs.gws.PassengerDetails.MessageHeaderTOPartyId
            {
                Value = credentialParameters.Where(q => q.Parameter == "PartyIdTo").FirstOrDefault()?.Value,
                type = "URI"
            };

            GolWs.gws.PassengerDetails.MessageHeader messageHeader = new GolWs.gws.PassengerDetails.MessageHeader
            {
                From = new GolWs.gws.PassengerDetails.MessageHeaderFrom
                {
                    PartyId = from
                },
                To = new GolWs.gws.PassengerDetails.MessageHeaderTO
                {
                    PartyId = to
                },
                CPAId = sessionC.MessageHeader.CPAId,
                ConversationId = sessionC.MessageHeader.ConversationId,
                Action = "PassengerDetailsRQ",
                Service = new GolWs.gws.PassengerDetails.MessageHeaderService
                {
                    Value = "PassengerDetailsRQ"
                },
                MessageData = new GolWs.gws.PassengerDetails.MessageHeaderMessageData
                {
                    MessageId = sessionC.MessageHeader.MessageData.MessageId,
                    Timestamp = DateTime.Now.AddHours(7).ToString("s")
                }
            };

            GolWs.gws.PassengerDetails.Security security = new GolWs.gws.PassengerDetails.Security
            {
                BinarySecurityToken = sessionC.Security.BinarySecurityToken
            };

            #endregion

            var contactNumber = new ArrayOfPassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoContactNumberContactNumber[priceRQ.Passagers.Count + 1];
            var email = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoEmail[priceRQ.Passagers.Count];
            var person = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassenger[priceRQ.Passagers.Count];
            var person2 = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoPersonName[priceRQ.Passagers.Count];
            var service = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoService[1 + priceRQ.Passagers.Where(pax => (Convert.ToDateTime(priceRQ.JorneysSell.OrderBy(x => x.ArrivalDateTime).LastOrDefault()?.ArrivalDateTime) - pax.DateBirth).TotalDays < (2 * 365)).Count()];

            service[0] = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoService
            {
                SegmentNumber = "1",
                SSR_Code = "OSI",
                Text = "OIN " + credentialParameters.Where(q => q.Parameter == "VoeBiz").FirstOrDefault()?.Value,
                VendorPrefs = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefs
                {
                    Airline = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefsAirline
                    {
                        Hosted = true
                    }
                }
            };

            int i = 0;
            int infantCount = 0;
            foreach (var pax in priceRQ.Passagers)
            {
                if (pax.PaxType == "CHD")
                {
                    if ((Convert.ToDateTime(priceRQ.JorneysSell.OrderBy(x => x.ArrivalDateTime).LastOrDefault()?.ArrivalDateTime) - pax.DateBirth).TotalDays < (2 * 365))
                    {
                        service[1 + infantCount] = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoService
                        {
                            SegmentNumber = "1",
                            SSR_Code = "INFT",
                            Text = pax.LastName + "/" + pax.FirstName + "/" + pax.DateBirth.ToString("ddMMMyy") + "-" + infantCount + ".1",
                            VendorPrefs = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefs
                            {
                                Airline = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefsAirline
                                {
                                    Hosted = true
                                }
                            }
                        };
                        pax.PaxType = "INF";
                        infantCount++;
                    }
                    else
                    {
                        pax.PaxType = "CNN";
                    }
                }
                contactNumber[i + 1] = new ArrayOfPassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoContactNumberContactNumber
                {
                    Phone = pax.PhoneNumber,
                    PhoneUseType = pax.PhoneType == "CELL_PHONE" ? "H" : "A",
                    LocationCode = pax.PhoneLocalCode,
                    NameNumber = "1."+i
                };
                email[i] = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoEmail
                {
                    Address = pax.Email,
                    Type = PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoEmailType.TO
                };
   
                person2[i] = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoPersonName
                {
                    GivenName = pax.FirstName,
                    Surname = pax.LastName,
                    NameNumber = i + ".1"
                };

                person[i] = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassenger
                {
                    PersonName = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassengerPersonName
                    {
                        GivenName = pax.FirstName,
                        Surname = pax.LastName,
                        DateOfBirth = pax.DateBirth,
                        Gender = PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassengerPersonNameGender.M,
                        NameNumber = i+".1"
                    },
                    VendorPrefs = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassengerVendorPrefs
                    {
                        Airline = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassengerVendorPrefsAirline
                        {
                            Hosted = true
                        }
                    },
                    Document = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassengerDocument
                    {
                        Number = pax.DocNumber,
                        IssueCountry = "BR",
                        NationalityCountry = "BR",
                        Type = pax.DocType
                    },
                    SegmentNumber = "1"
                };

                i++;
            }

            var pQuoteInfo = new ArrayOfPassengerDetailsRQLinkLink[1];
            pQuoteInfo[0] = new ArrayOfPassengerDetailsRQLinkLink
            {
                hostedCarrier = true,
                nameNumber = "1.1",
                record = "1"
            };

            PassengerDetailsRQRequest passengerDetailsRQRequest = new PassengerDetailsRQRequest
            {
                MessageHeader = messageHeader,
                Security = security,
                PassengerDetailsRQ = new PassengerDetailsRQ
                {
                    version = "3.4.0",
                    haltOnError = true,
                    ignoreOnError = true,
                    PostProcessing = new PassengerDetailsRQPostProcessing
                    {
                        ignoreAfter = false,
                        RedisplayReservation = new PassengerDetailsRQPostProcessingRedisplayReservation
                        {
                            waitInterval = "100"
                        },
                        EndTransactionRQ = new PassengerDetailsRQPostProcessingEndTransactionRQ
                        {
                            EndTransaction = new PassengerDetailsRQPostProcessingEndTransactionRQEndTransaction
                            {
                                Ind = true
                            },
                            Source = new PassengerDetailsRQPostProcessingEndTransactionRQSource
                            {
                                ReceivedFrom = "G3 SAMPLE FLOWS"
                            }
                        }
                    },
                    PriceQuoteInfo = pQuoteInfo,
                    SpecialReqDetails = new PassengerDetailsRQSpecialReqDetails
                    {
                        SpecialServiceRQ = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQ
                        {
                            SpecialServiceInfo = new PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfo
                            {
                                AdvancePassenger = person,
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
                                TicketType = "82359/" + DateTime.Today.AddDays(2).AddHours(-2).ToString("dMMM")
                            }
                        },
                        CustomerInfo = new PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfo
                        {
                            ContactNumbers = contactNumber,
                            Email = email,
                            PersonName = person2
                        }
                    }
                }
            };

            var xml = WCFXMLManager.SerializeObjectToXml<PassengerDetailsRQRequest>(passengerDetailsRQRequest);

            var t = _webserviceGwsPassengerDetails.PassengerDetailsRQAsync(passengerDetailsRQRequest.Security, passengerDetailsRQRequest.MessageHeader, passengerDetailsRQRequest.PassengerDetailsRQ).Result;
            var xmlR = WCFXMLManager.SerializeObjectToXml<PassengerDetailsRQResponse>(t);
            return t;
        }

        #endregion

        #region BWS

        public GetPriceRS PriceItinerary(SessionProvider session, GetPriceRQ priceRQ)
        {
            try
            {
                var sessionBM = _mapper.Map<BWSSession>((GolWS.Homolog.SessionManager.BWSSession)session.Session);
                var agencyCode = session.AgencyCode;

                var itineraryPriceRequest = _mapper.Map<ItineraryPriceRequest>(priceRQ);

                return new GetPriceRS
                {
                    Price = _webservice.PriceItinerary(agencyCode, sessionBM, itineraryPriceRequest)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Erro obter tarifação Gol: " + ex.Message);
            }
        }


        public BookingRS SellBws(SessionProvider session, GetPriceRQ priceRQ)
        {
            var sessionBM = _mapper.Map<BWSSession>((GolWS.Homolog.SessionManager.BWSSession)session.Session);
            var agencyCode = session.AgencyCode;

            var SellRequest = _mapper.Map<SellRequestData>(priceRQ);

            _webservice.Sell(agencyCode, sessionBM, SellRequest);


            return new BookingRS
            {
                Booking = _webservice.Commit(agencyCode, sessionBM, CommitRQParse(priceRQ))
            };
        }

        public CommitRequestData CommitRQParse(GetPriceRQ priceRQ)
        {
            var bookingContacts = new BookingContact[1];

            var passengers = new GolWS.Homolog.BookingManager.Passenger[priceRQ.Passagers.Count];
            int i = 0;
            foreach (var pax in priceRQ.Passagers)
            {
                var names = new BookingName[1];
                names[0] = new BookingName
                {
                    FirstName = pax.FirstName,
                    LastName = pax.LastName
                };
                if (i == 0)
                {
                    bookingContacts[0] = new BookingContact
                    {
                        AddressLine1 = "Rua dos Antúrios, 256 Jr. Real",
                        City = "Sao Paulo",
                        CultureCode = "pt-BR", //código de idioma
                        EmailAddress = pax.Email,
                        HomePhone = pax.PhoneNumber,
                        Names = names,
                        NotificationPreference = NotificationPreference.None,
                        TypeCode = "H", //Se estas informações (telefone, endereco, etc...) são residenciais, passamos o valor 'H'. Se são do trabalho, passamos o valor 'W'.
                        CountryCode = "BR",
                    };
                }


                var passengerTypeInfos = new PassengerTypeInfo[1];
                passengerTypeInfos[0] = new PassengerTypeInfo
                {
                    DOB = pax.DateBirth,
                    PaxType = pax.PaxType
                };

                passengers[i] = new GolWS.Homolog.BookingManager.Passenger
                {
                    PassengerInfo = new PassengerInfo
                    {
                        Gender = Gender.Male,
                        ResidentCountry = "BR",
                        Nationality = "BR"
                    },
                    PassengerNumber = (short)i,
                    Names = names,
                    PassengerTypeInfos = passengerTypeInfos,
                    State = MessageState.Confirmed
                };
                i++;
            }


            return new CommitRequestData
            {
                Booking = new GolWS.Homolog.BookingManager.Booking
                {
                    CurrencyCode = priceRQ.CurrencyCode,
                    PaxCount = (short)priceRQ.Passagers.Count,
                    BookingContacts = bookingContacts,
                    Passengers = passengers
                },
                RestrictionOverride = false,
                ChangeHoldDateTime = false,
                WaiveNameChangeFee = false,
                WaivePenaltyFee = false,
                WaiveSpoilageFee = false,
                DistributeToContacts = true
            };
        }
        
        #endregion
    }
}