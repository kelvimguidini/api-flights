using AutoMapper;
using flights.domain.Interfaces.Providers.Latam;
using flights.domain.Models;
using flights.domain.Models.Availability;
using System;
using LatamWS.Homolog.AdvancedAirShopping;
using flights.crosscutting.AppConfig;
using flights.crosscutting.Utils;

namespace flights.provider.latam.Services
{
    public class AvailabilityLatamService : IAvailabilityLatamService
    {
        private readonly IMapper _mapper;

        public AvailabilityLatamService(IMapper mapper)
        {
            _mapper = mapper;
        }

        private readonly AdvancedAirShoppingPortType _webservice = new AdvancedAirShoppingPortTypeClient(AdvancedAirShoppingPortTypeClient.EndpointConfiguration.AdvancedAirShoppingPortType, new AppConfiguration("EndPoints", "Latam").Configuration);

        public AvailabilityRS GetAvailability(SessionProvider session, object availability)
        {
            try
            {
                var obj = availability.GetType().GetProperty("availabilityRQDTO");

                var countTotalPassangers = obj.GetValue(availability, null).GetType().GetProperty("CountTotalPassangers").GetValue(obj.GetValue(availability, null), null);
                var countADT = obj.GetValue(availability, null).GetType().GetProperty("CountADT").GetValue(obj.GetValue(availability, null), null);
                var countCHD = obj.GetValue(availability, null).GetType().GetProperty("CountCHD").GetValue(obj.GetValue(availability, null), null);
                var countINF = obj.GetValue(availability, null).GetType().GetProperty("CountINF").GetValue(obj.GetValue(availability, null), null);
                var returnDate = (DateTime?)obj.GetValue(availability, null).GetType().GetProperty("ReturnDate").GetValue(obj.GetValue(availability, null), null);
                var departDate = (DateTime)obj.GetValue(availability, null).GetType().GetProperty("DepartureDate").GetValue(obj.GetValue(availability, null), null);

                int qtd = (returnDate == null) ? 1 : 2;
                var pos = new SourceType[1];
                var trip = new OTA_AirLowFareSearchRQOriginDestinationInformation[qtd];

                pos[0] = new SourceType
                {
                    DefaultTicketingCarrier = "JJ",
                    PseudoCityCode = session.Session.GetType().GetProperty("PseudoCityCode").GetValue(session.Session, null).ToString(),
                    PersonalCityCode = session.Session.GetType().GetProperty("PersonalCityCode").GetValue(session.Session, null).ToString(),
                    OfficeCode = session.Session.GetType().GetProperty("OfficeCode").GetValue(session.Session, null).ToString(),
                    AccountingCode = session.Session.GetType().GetProperty("AccountingCode").GetValue(session.Session, null).ToString(),
                    RequestorID = new UniqueID_Type
                    {
                        Type = "1",
                        ID_Context = "1",
                        CompanyName = new CompanyNameType
                        {
                            Code = "SSW"
                        }
                    },
                };

                trip[0] = new OTA_AirLowFareSearchRQOriginDestinationInformation
                {
                    RPH = "1",
                    OriginLocation = new OriginDestinationInformationTypeOriginLocation
                    {
                        LocationCode = obj.GetValue(availability, null).GetType().GetProperty("DepartureCode").GetValue(obj.GetValue(availability, null), null).ToString(),
                    },
                    DestinationLocation = new OriginDestinationInformationTypeDestinationLocation
                    {
                        LocationCode = obj.GetValue(availability, null).GetType().GetProperty("ArrivalCode").GetValue(obj.GetValue(availability, null), null).ToString(),
                    },
                    ItemElementName = ItemChoiceType.DepartureDateTime,
                    Item = departDate.ToString("s")
                };

                if (returnDate != null)
                {
                    DateTime rd = (DateTime)returnDate;
                    trip[1] = new OTA_AirLowFareSearchRQOriginDestinationInformation
                    {
                        RPH = "2",
                        OriginLocation = new OriginDestinationInformationTypeOriginLocation
                        {
                            LocationCode = obj.GetValue(availability, null).GetType().GetProperty("ArrivalCode").GetValue(obj.GetValue(availability, null), null).ToString(),
                        },
                        DestinationLocation = new OriginDestinationInformationTypeDestinationLocation
                        {
                            LocationCode = obj.GetValue(availability, null).GetType().GetProperty("DepartureCode").GetValue(obj.GetValue(availability, null), null).ToString(),
                        },
                        ItemElementName = ItemChoiceType.DepartureDateTime,

                        Item = rd.ToString("s")
                    };
                }
                var from = new PartyId[1];

                from[0] = new PartyId
                {
                    Value = session.Session.GetType().GetProperty("From").GetValue(session.Session, null).ToString(),
                    type = "URI"
                };

                var to = new PartyId[1];

                to[0] = new PartyId
                {
                    Value = session.Session.GetType().GetProperty("To").GetValue(session.Session, null).ToString(),
                    type = "URI"
                };

                var desc = new Description[1];

                desc[0] = new Description
                {
                    lang = "en-us"
                };

                //var messageHeader = _mapper.Map<MessageHeader>(authentication.MessageHeader);
                MessageHeader messageHeader = new MessageHeader
                {
                    Action = "AdvancedAirShoppingRQ",
                    Service = new LatamWS.Homolog.AdvancedAirShopping.Service
                    {
                        Value = "AdvancedAirShoppingRQ"
                    },
                    From = new From
                    {
                        PartyId = from
                    },
                    To = new To
                    {
                        PartyId = to
                    },
                    CPAId = session.Session.GetType().GetProperty("CPAId").GetValue(session.Session, null).ToString(),
                    ConversationId = session.Session.GetType().GetProperty("CPAId").GetValue(session.Session, null).ToString(),
                    MessageData = new MessageData
                    {
                        MessageId = session.Session.GetType().GetProperty("MessageId").GetValue(session.Session, null).ToString(),
                        Timestamp = DateTime.Now.ToString("s"),
                    },
                    Description = desc
                };

                Security security = new Security
                {
                    BinarySecurityToken = session.Session.GetType().GetProperty("Token").GetValue(session.Session, null).ToString()
                };

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
                var inf = new PassengerTypeQuantityType
                {
                    Code = "INF",
                    Quantity = countINF.ToString()
                };


                int qtdPax = 0;
                if ((int)countADT > 0) {
                    qtdPax++;
                }
                if ((int)countCHD > 0) {
                    qtdPax++;
                }
                if ((int)countINF > 0) {
                    qtdPax++;
                }


                var paxType = new PassengerTypeQuantityType[qtdPax];
                if ((int)countADT > 0)
                {
                    paxType[qtdPax-1] = adt;
                    qtdPax--;
                }
                if ((int)countCHD > 0)
                {
                    paxType[qtdPax - 1] = chd;
                    qtdPax--;
                }
                if ((int)countINF > 0)
                {
                    paxType[qtdPax - 1] = inf;
                }

                ptc[0] = new TravelerInformationType
                {
                    PassengerTypeQuantity = paxType
                };

                var vendorType = new CompanyNamePrefType[2];
                vendorType[0] = new CompanyNamePrefType
                {
                    Code = "LA"
                };
                vendorType[0] = new CompanyNamePrefType
                {
                    Code = "JJ"
                };

                var cabinPref = new CabinPrefType[1];
                cabinPref[0] = new CabinPrefType
                {
                    Cabin = CabinType.Y,
                    PreferLevel = PreferLevelType.Only
                };

                var request = new AdvancedAirShoppingRQRequest
                {
                    MessageHeader = messageHeader,
                    Security = security,
                    OTA_AirLowFareSearchRQ = new OTA_AirLowFareSearchRQ
                    {
                        POS = pos,
                        OriginDestinationInformation = trip,
                        Version = "5.4.0",
                        DirectFlightsOnly = false,
                        AvailableFlightsOnly = true,
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
                        },
                        TPA_Extensions = new OTA_AirLowFareSearchRQTPA_Extensions
                        {
                            IntelliSellTransaction = new TransactionType
                            {
                                RequestType = new TransactionTypeRequestType
                                {
                                    Name = "LABRD"
                                },
                                ServiceTag = new TransactionTypeServiceTag
                                {
                                    Name = "LA"
                                }
                            }
                        }
                    }
                };

                var xml = WCFXMLManager.SerializeObjectToXml<AdvancedAirShoppingRQRequest>(request);

                var resultO = _webservice.AdvancedAirShoppingRQAsync(request).Result;
                var xmlR = WCFXMLManager.SerializeObjectToXml<AdvancedAirShoppingRQResponse>(resultO);

                AvailabilityRS availabilityRS = new AvailabilityRS
                {
                    Availability = resultO
                };

                return availabilityRS;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter disponibilidade Latam: " + ex.Message);
            }
        }
    }
}