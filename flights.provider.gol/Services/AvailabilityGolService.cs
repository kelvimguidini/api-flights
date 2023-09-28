using AutoMapper;
using flights.crosscutting.AppConfig;
using flights.crosscutting.Utils;
using flights.domain.Interfaces.Providers.Gol;
using flights.domain.Models;
using flights.domain.Models.Availability;
using GolWS.gws.AdvancedAirShopping;
using GolWS.gws.ContextChange;
using GolWS.gws.DesignatePrinter;
using GolWS.Homolog.BookingManager;
using System;

namespace flights.provider.gol.Services
{
    public class AvailabilityGolService : IAvailabilityGolService
    {
        private readonly IMapper _mapper;

        public AvailabilityGolService(IMapper mapper)
        {
            _mapper = mapper;
        }

        private readonly BookingManagerClientClient _webservice = new BookingManagerClientClient(BookingManagerClientClient.EndpointConfiguration.BasicHttpBinding_IBookingManagerClient, new AppConfiguration("EndPoints", "GolBooking").Configuration);

        private readonly ContextChangeLLSRQServiceClient _webserviceGwsCC =
            new ContextChangeLLSRQServiceClient(ContextChangeLLSRQServiceClient.EndpointConfiguration.BasicHttpBinding_IContextChangeLLSRQService, new AppConfiguration("EndPoints", "Gol").Configuration + "ContextChangeLLSRQService.svc");

        private readonly DesignatePrinterLLSRQServiceClient _webserviceGwsDP =
            new DesignatePrinterLLSRQServiceClient(DesignatePrinterLLSRQServiceClient.EndpointConfiguration.BasicHttpBinding_IDesignatePrinterLLSRQService, new AppConfiguration("EndPoints", "Gol").Configuration + "DesignatePrinterLLSRQService.svc");

        private readonly AdvancedAirShoppingRQServiceClient _webserviceGwsAAS =
            new AdvancedAirShoppingRQServiceClient(AdvancedAirShoppingRQServiceClient.EndpointConfiguration.BasicHttpBinding_IAdvancedAirShoppingRQService, new AppConfiguration("EndPoints", "Gol").Configuration + "AdvancedAirShoppingRQService.svc");

        private readonly string[] _fareTypes = { "T", "C", "P", "R", "W", "CE", "CD", "CO", "VF", "CS", "AS", "CK", "CX", "AP" };

        #region GWS
        public AvailabilityRS GetAvailability(SessionProvider session, object availability)
        {
            try
            {
                ContextChange(session.Session);
                DesignatePrinter(session.Session);

                return AirLowFareSearch(session, availability);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro obter disponibilidade Gol: " + ex.Message);
            }
        }

        private void ContextChange(object sessionObj)
        {
            try
            {
                var session = (GolWS.gws.SessionCreate.SessionCreateRQResponse)sessionObj.GetType().GetProperty("Session").GetValue(sessionObj, null);

                #region Cabecalho

                var from = new GolWS.gws.ContextChange.MessageHeaderFromPartyId[1];
                from[0] = new GolWS.gws.ContextChange.MessageHeaderFromPartyId
                {
                    Value = sessionObj.GetType().GetProperty("From").GetValue(sessionObj, null).ToString(),
                    type = "URI"
                };

                var to = new GolWS.gws.ContextChange.MessageHeaderTOPartyId[1];
                to[0] = new GolWS.gws.ContextChange.MessageHeaderTOPartyId
                {
                    Value = sessionObj.GetType().GetProperty("To").GetValue(sessionObj, null).ToString(),
                    type = "URI"
                };

                #endregion

                var request = new ContextChangeRQRequest
                {
                    MessageHeader = new GolWS.gws.ContextChange.MessageHeader
                    {
                        From = new GolWS.gws.ContextChange.MessageHeaderFrom
                        {
                            PartyId = from
                        },
                        To = new GolWS.gws.ContextChange.MessageHeaderTO
                        {
                            PartyId = to
                        },
                        CPAId = session.MessageHeader.CPAId,
                        ConversationId = session.MessageHeader.ConversationId,
                        Service = new GolWS.gws.ContextChange.MessageHeaderService()
                        {
                            Value = "ContextChangeLLSRQ"
                        },
                        Action = "ContextChangeLLSRQ",
                        MessageData = new GolWS.gws.ContextChange.MessageHeaderMessageData
                        {
                            MessageId = session.MessageHeader.MessageData.MessageId,
                            Timestamp = DateTime.Now.AddHours(7).ToString("s")
                        }
                    },
                    Security = new GolWS.gws.ContextChange.Security
                    {
                        BinarySecurityToken = session.Security.BinarySecurityToken
                    },
                    ContextChangeRQ = new GolWS.gws.ContextChange.ContextChangeRQ
                    {
                        ReturnHostCommand = true,
                        TimeStamp = DateTime.Now,
                        Version = "2.0.3",
                        ChangeDuty = new ContextChangeRQChangeDuty
                        {
                            Code = "4"
                        }
                    }
                };

                var xml = WCFXMLManager.SerializeObjectToXml<ContextChangeRQRequest>(request);
                var sessionResponse = _webserviceGwsCC.ContextChangeRQAsync(request.Security, request.MessageHeader, request.ContextChangeRQ);

                var xmlR = WCFXMLManager.SerializeObjectToXml<ContextChangeRQResponse>(sessionResponse.Result);
            }
            catch (Exception ex)
            {
                throw new Exception("Houve algum erro ao tentar se conectar ao serviço: " + ex.Message);
            }
        }

        private void DesignatePrinter(object sessionObj)
        {
            try
            {

                var session = (GolWS.gws.SessionCreate.SessionCreateRQResponse)sessionObj.GetType().GetProperty("Session").GetValue(sessionObj, null);

                #region Cabecalho

                var from = new GolWS.gws.DesignatePrinter.MessageHeaderFromPartyId[1];
                from[0] = new GolWS.gws.DesignatePrinter.MessageHeaderFromPartyId
                {
                    Value = sessionObj.GetType().GetProperty("From").GetValue(sessionObj, null).ToString(),
                    type = "URI"
                };

                var to = new GolWS.gws.DesignatePrinter.MessageHeaderTOPartyId[1];
                to[0] = new GolWS.gws.DesignatePrinter.MessageHeaderTOPartyId
                {
                    Value = sessionObj.GetType().GetProperty("To").GetValue(sessionObj, null).ToString(),
                    type = "URI"
                };

                #endregion

                var request = new DesignatePrinterRQRequest
                {
                    MessageHeader = new GolWS.gws.DesignatePrinter.MessageHeader
                    {
                        From = new GolWS.gws.DesignatePrinter.MessageHeaderFrom
                        {
                            PartyId = from
                        },
                        To = new GolWS.gws.DesignatePrinter.MessageHeaderTO
                        {
                            PartyId = to
                        },
                        CPAId = session.MessageHeader.CPAId,
                        ConversationId = session.MessageHeader.ConversationId,
                        Service = new GolWS.gws.DesignatePrinter.MessageHeaderService()
                        {
                            Value = "DesignatePrinterLLSRQ"
                        },
                        Action = "DesignatePrinterLLSRQ",
                        MessageData = new GolWS.gws.DesignatePrinter.MessageHeaderMessageData
                        {
                            MessageId = session.MessageHeader.MessageData.MessageId,
                            Timestamp = DateTime.Now.AddHours(7).ToString("s")
                        }
                    },
                    Security = new GolWS.gws.DesignatePrinter.Security
                    {
                        BinarySecurityToken = session.Security.BinarySecurityToken
                    },
                    DesignatePrinterRQ = new GolWS.gws.DesignatePrinter.DesignatePrinterRQ
                    {
                        ReturnHostCommand = true,
                        Version = "2.0.1",
                        Printers = new DesignatePrinterRQPrinters
                        {
                            Ticket = new DesignatePrinterRQPrintersTicket
                            {
                                CountryCode = "2A"
                            }
                        }
                    }
                };

                var xml = WCFXMLManager.SerializeObjectToXml<DesignatePrinterRQRequest>(request);
                var sessionResponse = _webserviceGwsDP.DesignatePrinterRQAsync(request.Security, request.MessageHeader, request.DesignatePrinterRQ);
                var xmlR = WCFXMLManager.SerializeObjectToXml<DesignatePrinterRQResponse>(sessionResponse.Result);
            }
            catch (Exception ex)
            {
                throw new Exception("Houve algum erro ao tentar se conectar ao serviço: " + ex.Message);
            }
        }

        private AvailabilityRS AirLowFareSearch(SessionProvider session, object availability)
        {
            var sessionResponse = (GolWS.gws.SessionCreate.SessionCreateRQResponse)session.Session.GetType().GetProperty("Session").GetValue(session.Session, null);

            var obj = availability.GetType().GetProperty("availabilityRQDTO");

            var countADT = obj.GetValue(availability, null).GetType().GetProperty("CountADT").GetValue(obj.GetValue(availability, null), null);
            var countCHD = obj.GetValue(availability, null).GetType().GetProperty("CountCHD").GetValue(obj.GetValue(availability, null), null);
            var returnDate = (DateTime?)obj.GetValue(availability, null).GetType().GetProperty("ReturnDate").GetValue(obj.GetValue(availability, null), null);
            var departDate = (DateTime)obj.GetValue(availability, null).GetType().GetProperty("DepartureDate").GetValue(obj.GetValue(availability, null), null);

            var ptc = new TravelerInformationType[1];


            var adt = new PassengerTypeQuantityType
            {
                Code = "ADT",
                Quantity = countADT.ToString()
            };

            var chd = new PassengerTypeQuantityType
            {
                Code = "CNN",
                Quantity = countCHD.ToString()
            };

            int qtdPax = (int)countADT > 0 && (int)countCHD > 0 ? 2 : 1;

            var paxType = new PassengerTypeQuantityType[qtdPax];
            if ((int)countADT > 0)
            {
                paxType[0] = adt;
                if ((int)countCHD > 0)
                {
                    paxType[1] = chd;
                }
            }
            else if ((int)countCHD > 0)
            {
                paxType[0] = chd;
            }
            ptc[0] = new TravelerInformationType
            {
                PassengerTypeQuantity = paxType
            };

            int qtd = (returnDate == null) ? 1 : 2;
            var pos = new SourceType[1];

            pos[0] = new SourceType
            {

                DefaultTicketingCarrier = session.Session.GetType().GetProperty("DefaultTicketingCarrier").GetValue(session.Session, null).ToString(),
                PseudoCityCode = session.Session.GetType().GetProperty("PseudoCityCode").GetValue(session.Session, null).ToString(),
                PersonalCityCode = session.Session.GetType().GetProperty("PersonalCityCode").GetValue(session.Session, null).ToString(),
                OfficeCode = session.Session.GetType().GetProperty("OfficeCode").GetValue(session.Session, null).ToString(),
                AccountingCode = session.Session.GetType().GetProperty("AccountingCode").GetValue(session.Session, null).ToString(),
                RequestorID = new UniqueID_Type
                {
                    ID = "1",
                    Type = "1",
                    ID_Context = "1",
                    CompanyName = new CompanyNameType
                    {
                        Code = "SSW"
                    }
                },
            };

            var trip = new OTA_AirLowFareSearchRQOriginDestinationInformation[qtd];

            var dateFlexibility = new OTA_AirLowFareSearchRQOriginDestinationInformationTPA_ExtensionsDateFlexibility[1];

            dateFlexibility[0] = new OTA_AirLowFareSearchRQOriginDestinationInformationTPA_ExtensionsDateFlexibility
            {
                NbrOfDays = 0
            };

            trip[0] = new OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                RPH = "1",
                OriginLocation = new OriginDestinationInformationTypeOriginLocation
                {
                    LocationCode = obj.GetValue(availability, null).GetType().GetProperty("DepartureCode").GetValue(obj.GetValue(availability, null), null).ToString(),
                },
                DepartureDateTime = departDate.ToString("s"),
                DestinationLocation = new OriginDestinationInformationTypeDestinationLocation
                {
                    LocationCode = obj.GetValue(availability, null).GetType().GetProperty("ArrivalCode").GetValue(obj.GetValue(availability, null), null).ToString(),
                },
                TPA_Extensions = new OTA_AirLowFareSearchRQOriginDestinationInformationTPA_Extensions
                {
                    DateFlexibility = dateFlexibility,
                    SegmentType = new OTA_AirLowFareSearchRQOriginDestinationInformationTPA_ExtensionsSegmentType
                    {
                        Code = OTA_AirLowFareSearchRQOriginDestinationInformationTPA_ExtensionsSegmentTypeCode.O
                    }
                },

            };

            if (returnDate != null)
            {
                trip[1] = new OTA_AirLowFareSearchRQOriginDestinationInformation
                {
                    RPH = "1",
                    OriginLocation = new OriginDestinationInformationTypeOriginLocation
                    {
                        LocationCode = obj.GetValue(availability, null).GetType().GetProperty("ArrivalCode").GetValue(obj.GetValue(availability, null), null).ToString(),
                    },
                    DepartureDateTime = returnDate?.ToString("s"),
                    DestinationLocation = new OriginDestinationInformationTypeDestinationLocation
                    {
                        LocationCode = obj.GetValue(availability, null).GetType().GetProperty("DepartureCode").GetValue(obj.GetValue(availability, null), null).ToString(),
                    },
                    TPA_Extensions = new OTA_AirLowFareSearchRQOriginDestinationInformationTPA_Extensions
                    {
                        DateFlexibility = dateFlexibility,
                        SegmentType = new OTA_AirLowFareSearchRQOriginDestinationInformationTPA_ExtensionsSegmentType
                        {
                            Code = OTA_AirLowFareSearchRQOriginDestinationInformationTPA_ExtensionsSegmentTypeCode.O
                        }
                    },
                };
            }

            #region Cabecalho

            var from = new GolWS.gws.AdvancedAirShopping.MessageHeaderFromPartyId[1];
            from[0] = new GolWS.gws.AdvancedAirShopping.MessageHeaderFromPartyId
            {
                Value = session.Session.GetType().GetProperty("From").GetValue(session.Session, null).ToString(),
                type = "URI"
            };

            var to = new GolWS.gws.AdvancedAirShopping.MessageHeaderTOPartyId[1];
            to[0] = new GolWS.gws.AdvancedAirShopping.MessageHeaderTOPartyId
            {
                Value = session.Session.GetType().GetProperty("To").GetValue(session.Session, null).ToString(),
                type = "URI"
            };

            #endregion

            var request = new AdvancedAirShoppingRQRequest
            {
                MessageHeader = new GolWS.gws.AdvancedAirShopping.MessageHeader
                {
                    From = new GolWS.gws.AdvancedAirShopping.MessageHeaderFrom
                    {
                        PartyId = from
                    },
                    To = new GolWS.gws.AdvancedAirShopping.MessageHeaderTO
                    {
                        PartyId = to
                    },
                    CPAId = sessionResponse.MessageHeader.CPAId,
                    ConversationId = sessionResponse.MessageHeader.ConversationId,
                    Service = new GolWS.gws.AdvancedAirShopping.MessageHeaderService()
                    {
                        Value = "AdvancedAirShoppingRQ"
                    },
                    Action = "AdvancedAirShoppingRQ",
                    MessageData = new GolWS.gws.AdvancedAirShopping.MessageHeaderMessageData
                    {
                        MessageId = sessionResponse.MessageHeader.MessageData.MessageId,
                        Timestamp = DateTime.Now.AddHours(7).ToString("s")
                    }
                },
                Security = new GolWS.gws.AdvancedAirShopping.Security
                {
                    BinarySecurityToken = sessionResponse.Security.BinarySecurityToken
                },
                OTA_AirLowFareSearchRQ = new GolWS.gws.AdvancedAirShopping.OTA_AirLowFareSearchRQ
                {
                    Target = OTA_AirLowFareSearchRQTarget.Production,
                    Version = "5.4.0",
                    ResponseType = "OTA",
                    ResponseVersion = "5.1.0",
                    POS = pos,
                    OriginDestinationInformation = trip,
                    TravelPreferences = new AirSearchPrefsType
                    {
                        MaxStopsQuantity = session.Session.GetType().GetProperty("MaxStopsQuantity").GetValue(session.Session, null).ToString(),
                        TPA_Extensions = new AirSearchPrefsTypeTPA_Extensions
                        {
                            TripType = new AirSearchPrefsTypeTPA_ExtensionsTripType
                            {
                                Value = (returnDate != null) ? AirTripType.Return : AirTripType.OneWay
                            },
                            NumTrips = new NumTripsType
                            {
                                Number = 30
                            }
                        }
                    },
                    TravelerInfoSummary = new TravelerInfoSummaryType
                    {
                        AirTravelerAvail = ptc,
                        PriceRequestInformation = new PriceRequestInformationType
                        {
                            CurrencyCode = "BRL",
                            TPA_Extensions = new PriceRequestInformationTypeTPA_Extensions
                            {
                                PrivateFare = new PriceRequestInformationTypeTPA_ExtensionsPrivateFare
                                {
                                    Ind = true
                                }
                            }
                        }
                    },
                    TPA_Extensions = new OTA_AirLowFareSearchRQTPA_Extensions
                    {
                        IntelliSellTransaction = new TransactionType
                        {
                            Debug = false,
                            RequestType = new TransactionTypeRequestType
                            {
                                Name = "ADVBRD"
                            },
                            ServiceTag = new TransactionTypeServiceTag
                            {
                                Name = "G3"
                            }
                        },
                        SplitTaxes = new OTA_AirLowFareSearchRQTPA_ExtensionsSplitTaxes
                        {
                            ByLeg = true,
                            ByFareComponent = true
                        }
                    },
                    DirectFlightsOnly = false,
                    AvailableFlightsOnly = true,
                }
            };


            var xml = WCFXMLManager.SerializeObjectToXml<AdvancedAirShoppingRQRequest>(request);

            var resultO = _webserviceGwsAAS.AdvancedAirShoppingRQAsync(request.Security, request.MessageHeader, request.OTA_AirLowFareSearchRQ).Result;

            var xmlR = WCFXMLManager.SerializeObjectToXml<AdvancedAirShoppingRQResponse>(resultO);

            AvailabilityRS availabilityRS = new AvailabilityRS
            {
                Availability = resultO
            };

            return availabilityRS;
        }

        #endregion

        #region BWS
        public AvailabilityRS GetAvailabilityBws(SessionProvider session, object availability)
        {
            try
            {
                var obj = availability.GetType().GetProperty("availabilityRQDTO");

                var countTotalPassangers = obj.GetValue(availability, null).GetType().GetProperty("CountTotalPassangers").GetValue(obj.GetValue(availability, null), null);
                var countADT = obj.GetValue(availability, null).GetType().GetProperty("CountADT").GetValue(obj.GetValue(availability, null), null);
                var countCHD = obj.GetValue(availability, null).GetType().GetProperty("CountCHD").GetValue(obj.GetValue(availability, null), null);
                var returnDate = obj.GetValue(availability, null).GetType().GetProperty("ReturnDate").GetValue(obj.GetValue(availability, null), null);

                var agencyCode = session.AgencyCode;


                AvailabilityRequest availabilityRequest = _mapper.Map<AvailabilityRequest>(obj.GetValue(availability, null));


                var ptc = new PaxPriceType[(int)countTotalPassangers];


                for (int i = 0; i < (int)countTotalPassangers; i++)
                {
                    ptc[i] = new PaxPriceType() { PaxType = "ADT" };
                }

                for (int i = (int)countADT; i < (int)countADT + (int)countCHD; i++)
                {
                    ptc[i] = new PaxPriceType() { PaxType = "CHD" };
                }

                availabilityRequest.PaxPriceTypes = ptc;
                availabilityRequest.FareTypes = _fareTypes;

                int qtd = (returnDate == null) ? 1 : 2;
                var avRS = new AvailabilityRequest[qtd];

                avRS[0] = availabilityRequest;

                if (returnDate != null)
                {

                    AvailabilityRequest availabilityRequestReturn = _mapper.Map<AvailabilityRequest>(obj.GetValue(availability, null));
                    availabilityRequestReturn.PaxPriceTypes = ptc;
                    availabilityRequestReturn.FareTypes = _fareTypes;

                    availabilityRequestReturn.BeginDate = (DateTime)returnDate;
                    availabilityRequestReturn.EndDate = (DateTime)returnDate;

                    // Reversão dos sentidos da viagem para gerar o retorno corretamente conforme o request do WCF

                    var departure = availabilityRequestReturn.ArrivalStation.CloneObject();
                    var arrival = availabilityRequestReturn.DepartureStation.CloneObject();

                    availabilityRequestReturn.DepartureStation = departure;
                    availabilityRequestReturn.ArrivalStation = arrival;

                    avRS[1] = availabilityRequestReturn;
                }
                var trip = new TripAvailabilityRequest()
                {
                    AvailabilityRequests = avRS
                };


                var sessionBM = _mapper.Map<BWSSession>((GolWS.Homolog.SessionManager.BWSSession)session.Session);

                return new AvailabilityRS
                {
                    Availability = _webservice.GetAvailability(agencyCode, sessionBM, trip)
                };

            }
            catch (Exception ex)
            {
                throw new Exception("Erro obter disponibilidade Gol: " + ex.Message);
            }
        }
        #endregion
    }
}