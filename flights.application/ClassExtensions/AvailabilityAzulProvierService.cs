using flights.crosscutting.DomainObjects;
using flights.crosscutting.Utils;
using flights.domain.Models;
using flights.domain.Models.Availability;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace flights.application.Services
{
    public partial class AvailabilityProviderService
    {
        private Availability ToAvailabilityAzul(AvailabilityRS availabilityResponse, SessionProvider session)
        {
            Availability availability = new Availability();

            List<Recommendation> recommendations = new List<Recommendation>();
            List<Recommendation> recommendationsVolta = new List<Recommendation>();
            List<Recommendation> recommendationsIda = new List<Recommendation>();

            List<FlightSegment> segments = new List<FlightSegment>();
            List<Journey> journeys = new List<Journey>();
            List<PriceClass> priceClasses = new List<PriceClass>();
            List<FareFamily> fareFamilies = new List<FareFamily>();
            List<ServiceItem> serviceItems = new List<ServiceItem>();
            List<BaggageInfo> baggageInfos = new List<BaggageInfo>();
            List<Airport> airports = new List<Airport>();
            List<Airline> airlines = new List<Airline>();

            try
            {
                List<PriceKeys> priceKeys = new List<PriceKeys>();
                List<crosscutting.DomainObjects.Passenger> passengers = new List<crosscutting.DomainObjects.Passenger>();

                var scheduleObject = availabilityResponse.Availability.GetType().GetProperty("Schedules").GetValue(availabilityResponse.Availability, null);

                IEnumerable ArrayOfJourneyDateMarket = scheduleObject.GetType().GetProperty("ArrayOfJourneyDateMarket").GetValue(scheduleObject, null) as IEnumerable;

                bool isReturn = false;

                foreach (var item in ArrayOfJourneyDateMarket)
                {
                    var journeyDateMarket = item.GetType().GetProperty("JourneyDateMarket").GetValue(item, null);
                    var journey = journeyDateMarket.GetType().GetProperty("Journeys").GetValue(journeyDateMarket, null);
                    IEnumerable inventoryJourneys = journey.GetType().GetProperty("InventoryJourney").GetValue(journey, null) as IEnumerable;

                    foreach (var inventoryJourney in inventoryJourneys)
                    {
                        #region Recomendação

                        Recommendation recommendationRS = new Recommendation()
                        {
                            Offers = new List<Offer>(),
                            Provider = "AZUL"
                        };

                        #region Jornada

                        // Verificar por que DepartureCode não vem!

                        Journey journeyRS = new Journey()
                        {
                            Id = "JRN-",
                            ArrivalCode = journeyDateMarket.GetType().GetProperty("ArrivalStation").GetValue(journeyDateMarket, null).ToString().ToUpper(),
                            DepartureCode = journeyDateMarket.GetType().GetProperty("DepartureStation").GetValue(journeyDateMarket, null).ToString().ToUpper(),
                            FlightSegmentsIds = new List<string>(),
                            SellKey = inventoryJourney.GetType().GetProperty("SellKey").GetValue(inventoryJourney, null).ToString()
                        };

                        bool segInicial = true;

                        #region Seguimento

                        var segmentsObject = inventoryJourney.GetType().GetProperty("Segments").GetValue(inventoryJourney, null);
                        IEnumerable segmentList = segmentsObject.GetType().GetProperty("Segment").GetValue(segmentsObject, null) as IEnumerable;
                        List<string> ids = new List<string>();
                        foreach (var segment in segmentList)
                        {
                            List<FlightPriceClassAssociation> flightPriceClasses = new List<FlightPriceClassAssociation>();

                            var legsObject = segment.GetType().GetProperty("Legs").GetValue(segment, null);
                            IEnumerable legList = legsObject.GetType().GetProperty("Leg").GetValue(legsObject, null) as IEnumerable;

                            int i = 0;
                            foreach (var leg in legList)
                            {
                                DateTime departTime = (DateTime)leg.GetType().GetProperty("STD").GetValue(leg, null);
                                DateTime arrTime = (DateTime)leg.GetType().GetProperty("STA").GetValue(leg, null);

                                string arrCode = leg.GetType().GetProperty("ArrivalStation").GetValue(leg, null).ToString().ToUpper();
                                string depCode = leg.GetType().GetProperty("DepartureStation").GetValue(leg, null).ToString().ToUpper();
                                var flightDesignator = segment.GetType().GetProperty("FlightDesignator").GetValue(segment, null);

                                string complementoId = i == 0 ? "" : i.ToString();
                                string segId = flightDesignator.GetType().GetProperty("CarrierCode").GetValue(flightDesignator, null).ToString().ToUpper() + flightDesignator.GetType().GetProperty("FlightNumber").GetValue(flightDesignator, null).ToString() + departTime.ToString("ddMMM").ToUpper() + complementoId;

                                if (segments.Any(x => x.Id.Contains(segId) && x.ArrivalCode == arrCode && x.DepartureCode == depCode))
                                {
                                    segId = segments.FirstOrDefault(x => x.Id.Contains(segId) && x.ArrivalCode == arrCode && x.DepartureCode == depCode)?.Id;
                                }

                                FlightSegment segmentRS = new FlightSegment()
                                {
                                    Id = segId,
                                    ArrivalCode = arrCode,
                                    DepartureCode = depCode,
                                    DepartureDateTime = departTime,
                                    ArrivalDateTime = arrTime,
                                    MarketingCarrierCode = flightDesignator.GetType().GetProperty("CarrierCode").GetValue(flightDesignator, null).ToString().ToUpper(),
                                    OperationCarrierCode = flightDesignator.GetType().GetProperty("CarrierCode").GetValue(flightDesignator, null).ToString().ToUpper(),
                                    FlightNumber = int.Parse(flightDesignator.GetType().GetProperty("FlightNumber").GetValue(flightDesignator, null).ToString()),
                                    Aircraft = leg.GetType().GetProperty("AircraftType").GetValue(leg, null).ToString(),
                                    Duration = (int)arrTime.Subtract(departTime).TotalMinutes,
                                };

                                if (!journeyRS.FlightSegmentsIds.Any(x => x == segmentRS.Id))
                                {
                                    journeyRS.Id += segmentRS.Id;
                                    journeyRS.FlightSegmentsIds.Add(segmentRS.Id);
                                }

                                if (!segments.Any(x => x.Id == segmentRS.Id))
                                {
                                    segments.Add(segmentRS);
                                }


                                flightPriceClasses.Add(new FlightPriceClassAssociation()
                                {
                                    FlightSegmentId = segmentRS.Id
                                });

                                #region Aeroportos
                                //AEROPORTOS
                                if (!airports.Any(x => x.AirportCode == segmentRS.ArrivalCode))
                                {
                                    airports.Add(new Airport()
                                    {
                                        AirportCode = segmentRS.ArrivalCode
                                    });
                                }
                                if (!airports.Any(x => x.AirportCode == segmentRS.DepartureCode))
                                {
                                    airports.Add(new Airport()
                                    {
                                        AirportCode = segmentRS.DepartureCode
                                    });
                                }
                                #endregion
                                i++;
                            }

                            var jrnIds = new List<string>();
                            if (!jrnIds.Contains(journeyRS.Id))
                            {
                                jrnIds.Add(journeyRS.Id);
                            }

                            #region Oferta

                            var faresList = segment.GetType().GetProperty("Fares").GetValue(segment, null);
                            IEnumerable faresObject = faresList.GetType().GetProperty("Fare").GetValue(faresList, null) as IEnumerable;

                            foreach (var fare in faresObject)
                            {
                                // adiciona priceKeys para request do PriceItineraryWithKey
                                var _priceKey = new PriceKeys();
                                _priceKey.JourneySellKey = inventoryJourney.GetType().GetProperty("SellKey").GetValue(inventoryJourney, null).ToString();
                                _priceKey.FareSellKey = fare.GetType().GetProperty("SellKey").GetValue(fare, null).ToString();

                                if (!priceKeys.Any(x => x.FareSellKey == _priceKey.FareSellKey && x.JourneySellKey == _priceKey.JourneySellKey))
                                {
                                    priceKeys.Add(_priceKey);
                                }


                                string carrier = fare.GetType().GetProperty("CarrierCode").GetValue(fare, null).ToString().ToUpper();

                                #region base tarifária

                                string fareBase = fare.GetType().GetProperty("FareBasis").GetValue(fare, null).ToString().ToUpper();
                                string id = carrier
                                        + "-" + fare.GetType().GetProperty("ClassOfService").GetValue(fare, null).ToString().ToUpper()
                                        + "-" + fareBase;
                                ids.Add(id);
                                foreach (var flightPriceClass in flightPriceClasses)
                                {
                                    flightPriceClass.PriceClassId = id;
                                }

                                if (!priceClasses.Any(x => x.Id == id))
                                {
                                    priceClasses.Add(new PriceClass()
                                    {
                                        Id = id,
                                        ClassOfService = fare.GetType().GetProperty("ClassOfService").GetValue(fare, null).ToString().ToUpper(),
                                        FareBasis = fare.GetType().GetProperty("FareBasis").GetValue(fare, null).ToString().ToUpper(),
                                        SellKey = fare.GetType().GetProperty("SellKey").GetValue(fare, null).ToString()
                                        //FareSequence = (short)fare.GetType().GetProperty("FareSequence").GetValue(fare, null),
                                        //RuleNumber = fare.GetType().GetProperty("RuleNumber").GetValue(fare, null).ToString().ToUpper()

                                    });
                                }
                                #endregion

                                var paxFaresObject = fare.GetType().GetProperty("PaxFares").GetValue(fare, null);
                                IEnumerable paxFaresList = paxFaresObject.GetType().GetProperty("PaxFare").GetValue(paxFaresObject, null) as IEnumerable;

                                string className = "";

                                var services = new List<Service>();

                                //Adicionando para todos
                                services.Add(new Service
                                {
                                    ServiceItemId = "SVC-MEAL",
                                    IncludedWithOfferItem = true,
                                    FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                });

                                var refund = new Refund
                                {
                                    AllowsRefund = false,
                                    Percentage = 0
                                };

                                #region Ítens da oferta (por pax)
                                List<OfferItem> offersItens = new List<OfferItem>();
                                //IEnumerable paxFaresObject = fare.GetType().GetProperty("PaxFares").GetValue(fare, null) as IEnumerable;

                                var passenger = new crosscutting.DomainObjects.Passenger();
                                passenger.PassengerNumber = passengers.Count == 0 ? 0 : passengers.Count + 1;


                                foreach (var paxFare in paxFaresList)
                                {
                                    #region familia

                                    bool allowsSeat = false;
                                    bool allowsExchange = false;
                                    decimal after = 0.0M;
                                    decimal before = 0.0M;

                                    string pClass = paxFare.GetType().GetProperty("ProductClass").GetValue(paxFare, null).ToString().ToUpper(); // alterar aqui

                                    switch (pClass)
                                    {
                                        // Domestico
                                        case "F+":
                                        case "OI":
                                        case "TP":
                                        case "ZK":
                                            className = "AZUL";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-0-0KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            allowsExchange = true;
                                            after = 330.0M;
                                            before = 250.0M;
                                            break;
                                        case "PR":
                                            className = "MAISAZUL";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-1-23KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            refund.AllowsRefund = true;
                                            refund.Percentage = 40;

                                            allowsExchange = true;
                                            after = 330.0M;
                                            before = 250.0M;
                                            break;
                                        case "OF":
                                            className = "OPERADORA";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-1-23KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            refund.AllowsRefund = true;
                                            refund.Percentage = 40;
                                            break;
                                        // América do Sul e Caiena
                                        case "TI":
                                            className = "MAISAZUL";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-1-23KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            refund.AllowsRefund = true;
                                            refund.Percentage = 40;

                                            allowsExchange = true;
                                            after = 330.0M;
                                            before = 250.0M;
                                            break;
                                        // América do Norte e Europa
                                        // Economy
                                        case "ZJ":
                                            className = "MAIS AZUL";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-1-23KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            refund.AllowsRefund = true;
                                            refund.Percentage = 40;

                                            allowsExchange = true;
                                            after = 330.0M;
                                            before = 250.0M;
                                            break;
                                        case "ZF":
                                            className = "AZUL SUPER";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-2-23KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            refund.AllowsRefund = true;
                                            refund.Percentage = 40;
                                            break;
                                        case "ZE":
                                            className = "BUSINESS";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-3-23KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            refund.AllowsRefund = true;
                                            refund.Percentage = 40;
                                            break;
                                    }

                                    var fareFamily = new FareFamily()
                                    {
                                        Id = carrier + "-" + className,
                                        Code = pClass,
                                        Owner = carrier,
                                        Name = className,
                                        DescriptionInfo = new DescriptionInfo()
                                        {
                                            BaggageId = services.FirstOrDefault(x => !string.IsNullOrEmpty(x.BaggageId))?.BaggageId,
                                            Refund = refund,
                                            AllowsSeatReservation = allowsSeat,
                                            Exchange = new Exchange
                                            {
                                                AllowsExchange = allowsExchange,
                                                ExchangeAmounts = new ExchangeAmounts
                                                {
                                                    After = new ExchangeAmount
                                                    {
                                                        Amount = after,
                                                        CurrencyCode = "BRL"
                                                    },
                                                    Before = new ExchangeAmount
                                                    {
                                                        Amount = before,
                                                        CurrencyCode = "BRL"
                                                    }
                                                }
                                            }
                                        }
                                        //links
                                    };

                                    if (!fareFamilies.Any(x => x.Id == fareFamily.Id))
                                    {
                                        fareFamilies.Add(fareFamily);
                                    }

                                    #endregion

                                    var serviceChangesObject = paxFare.GetType().GetProperty("InternalServiceCharges").GetValue(paxFare, null);
                                    IEnumerable serviceChanges = serviceChangesObject.GetType().GetProperty("ServiceCharge").GetValue(serviceChangesObject, null) as IEnumerable;

                                    if (!offersItens.Any(x => x.Ptc == paxFare.GetType().GetProperty("PaxType").GetValue(paxFare, null).ToString()))
                                    {
                                        var offerIten = new OfferItem()
                                        {
                                            Ptc = paxFare.GetType().GetProperty("PaxType").GetValue(paxFare, null).ToString(),
                                            BaseFare = new BaseFare(),
                                            EquivalentFare = new EquivalentFare(),
                                            Services = services,
                                            Taxes = new List<Tax>(),
                                            TotalPrice = new TotalPrice()
                                        };

                                        foreach (var serviceChange in serviceChanges)
                                        {

                                            var chargeType = serviceChange.GetType().GetProperty("ChargeType").GetValue(serviceChange, null).ToString();

                                            if (chargeType == "FarePrice")
                                            {
                                                offerIten.BaseFare.Amount = Convert.ToDecimal(serviceChange.GetType().GetProperty("Amount").GetValue(serviceChange, null));
                                                offerIten.BaseFare.CurrencyCode = serviceChange.GetType().GetProperty("CurrencyCode").GetValue(serviceChange, null).ToString();
                                            }
                                            if (chargeType == "Tax")
                                            {
                                                offerIten.Taxes.Add(new Tax()
                                                {
                                                    TaxCode = chargeType,
                                                    TaxAmount = new TaxAmount()
                                                    {
                                                        Amount = Convert.ToDecimal(serviceChange.GetType().GetProperty("Amount").GetValue(serviceChange, null)),
                                                        CurrencyCode = serviceChange.GetType().GetProperty("CurrencyCode").GetValue(serviceChange, null).ToString()
                                                    },
                                                });
                                            }
                                            if (chargeType == "Discount")
                                            {
                                                offerIten.BaseFare.Amount -= Convert.ToDecimal(serviceChange.GetType().GetProperty("Amount").GetValue(serviceChange, null));
                                            }
                                        }

                                        offersItens.Add(offerIten);
                                    }

                                    var PaxPriceType = new PaxPriceType();
                                    PaxPriceType.PaxType = paxFare.GetType().GetProperty("PaxType").GetValue(paxFare, null).ToString();

                                    passenger.PaxPriceType.Add(PaxPriceType);
                                }

                                if (!passengers.Any(p => p.PaxPriceType.Any(x => x.PaxType == passenger.PaxPriceType.Where(c => c.PaxType == x.PaxType).FirstOrDefault()?.PaxType)))
                                {
                                    passengers.Add(passenger);
                                }

                                #endregion

                                if (segInicial)
                                {
                                    recommendationRS.Offers.Add(new Offer()
                                    {
                                        Owner = carrier,
                                        OfferAssociations = new OfferAssociations()
                                        {
                                            //AccountCode
                                            JourneyIds = jrnIds,
                                            FareFamilyId = carrier + "-" + className,
                                            FlightPriceClassAssociations = ExtensionMethods.CloneObject(flightPriceClasses),
                                            CredentialId = session.CredentialId
                                        },
                                        OfferItems = ExtensionMethods.CloneObject(offersItens)
                                    });
                                }
                            }

                            if (!segInicial)
                            {
                                foreach (var o in recommendationRS.Offers)
                                {
                                    if (o.OfferAssociations.FlightPriceClassAssociations.Any(x => ids.Contains(x.PriceClassId)))
                                    {
                                        o.OfferAssociations.FlightPriceClassAssociations.AddRange(ExtensionMethods.CloneObject(flightPriceClasses));
                                        o.OfferAssociations.JourneyIds.Clear();
                                        o.OfferAssociations.JourneyIds = jrnIds;
                                    }
                                }
                            }
                            segInicial = false;
                            #endregion

                        }

                        #endregion

                        var segmentInicio = segments.Where(x => journeyRS.Id.Contains(x.Id)).OrderBy(x => x.DepartureDateTime).FirstOrDefault();
                        var segmentFim = segments.Where(x => journeyRS.Id.Contains(x.Id)).OrderByDescending(x => x.DepartureDateTime).FirstOrDefault();

                        journeyRS.DepartureDateTime = (DateTime)segmentInicio?.DepartureDateTime;
                        journeyRS.DepartureCode = segmentInicio?.DepartureCode;

                        journeyRS.ArrivalDateTime = (DateTime)segmentFim?.ArrivalDateTime;
                        journeyRS.ArrivalCode = segmentFim?.ArrivalCode;
                        journeyRS.Duration = (int)journeyRS.ArrivalDateTime.Subtract(journeyRS.DepartureDateTime).TotalMinutes;

                        journeys.Add(journeyRS);

                        #endregion

                        recommendations.Add(recommendationRS);

                        #endregion

                    }

                    if (!isReturn)
                    {
                        recommendationsIda = recommendations;
                    }
                    else
                    {
                        recommendationsVolta = recommendations;
                    }
                    recommendations = new List<Recommendation>();
                    isReturn = true;
                }

                #region agrupar roundTrip

                recommendations = new List<Recommendation>();

                #region Tarifar a primeira ida para saber se tem DU

                bool hasDU = false;

                var priceItineraryRequest = new PriceItineraryRequestWithKeys();

                priceItineraryRequest.PriceKeys = priceKeys.Where(x => x.JourneySellKey == journeys.FirstOrDefault().SellKey && x.FareSellKey == priceClasses.FirstOrDefault(x => x.Id == recommendationsIda.FirstOrDefault()?.Offers.FirstOrDefault()?.OfferAssociations.FlightPriceClassAssociations.FirstOrDefault()?.PriceClassId)?.SellKey).Distinct().ToList();
                priceItineraryRequest.CurrencyCode = "BR";
                priceItineraryRequest.PaxResidentCountry = "BR";
                priceItineraryRequest.Passengers = passengers;

                //
                try
                {
                    var priceXmlStringResponse = _bookingAzulService.PriceItineraryByKeys(session, priceItineraryRequest);

                    var verifica = WCFXMLManager.CheckIfExistValueElement(priceXmlStringResponse, "PriceItineraryByKeysResult", "ServiceCharge", "TicketCode", "DUFEE");

                    if (verifica)
                    {
                        hasDU = true;
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                #endregion

                foreach (var ida in recommendationsIda)
                {
                    //var bkpJourneysIds = ida.Offers.FirstOrDefault()?.OfferAssociations.JourneyIds.ToList();
                    if (recommendationsVolta.Count > 0)
                    {
                        //var bkpJourneysIds = ida.Offers.FirstOrDefault()?.OfferAssociations.JourneyIds.ToList();
                        foreach (var volta in recommendationsVolta)
                        {
                            Recommendation newRec = new Recommendation()
                            {
                                Provider = ida.Provider,
                                Offers = new List<Offer>()
                            };
                            foreach (var offerIda in ida.Offers)
                            {
                                var bkpOferIda = ExtensionMethods.CloneObject(offerIda);

                                foreach (var offerVOlta in volta.Offers)
                                {
                                    if (offerIda.OfferAssociations.FareFamilyId == offerVOlta.OfferAssociations.FareFamilyId)
                                    {

                                        #region Agrupar assossiações

                                        bkpOferIda.OfferAssociations.JourneyIds.AddRange(ExtensionMethods.CloneObject(offerVOlta.OfferAssociations.JourneyIds.Where(x => !offerIda.OfferAssociations.JourneyIds.Contains(x)).ToList()));
                                        bkpOferIda.OfferAssociations.FlightPriceClassAssociations.AddRange(ExtensionMethods.CloneObject(offerVOlta.OfferAssociations.FlightPriceClassAssociations.Where(x => !offerIda.OfferAssociations.FlightPriceClassAssociations.Contains(x)).ToList()));

                                        #endregion

                                        #region Agrupar valor / totalizar

                                        bkpOferIda.OfferItems.Clear();
                                        foreach (var itemIda in offerIda.OfferItems.CloneObject())
                                        {
                                            foreach (var itemVolta in offerVOlta.OfferItems.CloneObject())
                                            {
                                                if (itemIda.Ptc == itemVolta.Ptc)
                                                {
                                                    itemIda.BaseFare.Amount += itemVolta.BaseFare.Amount;
                                                    itemIda.EquivalentFare = new EquivalentFare
                                                    {
                                                        Amount = itemIda.BaseFare.Amount,
                                                        CurrencyCode = itemIda.BaseFare.CurrencyCode
                                                    };

                                                    var taxs = itemIda.Taxes.Concat(itemVolta.Taxes).GroupBy(p => p.TaxCode)
                                                      .Where(p => p.Count() > 1)
                                                      .Select(p => new Tax
                                                      {
                                                          TaxCode = p.Key.ToString(),
                                                          TaxAmount = new TaxAmount
                                                          {
                                                              Amount = p.Sum(q => q.TaxAmount.Amount),
                                                              CurrencyCode = p.FirstOrDefault()?.TaxAmount.CurrencyCode
                                                          }
                                                      })
                                                      .ToList();

                                                    itemIda.Taxes.Clear();
                                                    itemIda.Taxes = taxs;

                                                }
                                            }

                                            if (hasDU)
                                            {
                                                itemIda.Taxes.Add(new Tax()
                                                {
                                                    TaxAmount = new TaxAmount()
                                                    {
                                                        CurrencyCode = itemIda.BaseFare.CurrencyCode,
                                                        Amount = itemIda.BaseFare.Amount < 400 ? 40 : itemIda.BaseFare.Amount * (decimal)0.1
                                                    },
                                                    TaxCode = "DU",
                                                });
                                            }

                                            //Totalizar
                                            itemIda.TotalPrice.Amount = itemIda.BaseFare.Amount + itemIda.Taxes.Sum(x => x.TaxAmount.Amount);
                                            itemIda.TotalPrice.CurrencyCode = itemIda.BaseFare.CurrencyCode;

                                            itemIda.Services.ForEach(x =>
                                            {
                                                x.FlightSegmentsIds.AddRange(bkpOferIda.OfferAssociations.FlightPriceClassAssociations.Select(y => y.FlightSegmentId).Where(y => !x.FlightSegmentsIds.Contains(y)));
                                            });

                                            bkpOferIda.OfferItems.Add(itemIda);

                                        }
                                        bkpOferIda.Id = "AD-" + Guid.NewGuid().ToString();
                                        newRec.Offers.Add(bkpOferIda);

                                        #endregion
                                    }
                                }

                            }

                            if (newRec.Offers != null && newRec.Offers.Count > 0)
                            {
                                recommendations.Add(newRec);
                            }
                        }
                    }
                    else
                    {
                        if (ida.Offers != null && ida.Offers.Count > 0 && !ida.Offers.Any(x => x.OfferItems.Any(y => y.Taxes.Count == 0)))
                        {
                            ida.Offers.ForEach(offer =>
                            {
                                offer.Id = "AD-" + Guid.NewGuid().ToString();
                                offer.OfferItems.ForEach(itemIda =>
                                {
                                    itemIda.TotalPrice.Amount = itemIda.BaseFare.Amount + itemIda.Taxes.Sum(x => x.TaxAmount.Amount);
                                    itemIda.TotalPrice.CurrencyCode = itemIda.BaseFare.CurrencyCode;

                                    itemIda.EquivalentFare = new EquivalentFare
                                    {
                                        Amount = itemIda.BaseFare.Amount,
                                        CurrencyCode = itemIda.BaseFare.CurrencyCode
                                    };
                                    if (hasDU)
                                    {
                                        itemIda.Taxes.Add(new Tax()
                                        {
                                            TaxAmount = new TaxAmount()
                                            {
                                                CurrencyCode = itemIda.BaseFare.CurrencyCode,
                                                Amount = itemIda.BaseFare.Amount < 400 ? 40 : itemIda.BaseFare.Amount * (decimal)0.1
                                            },
                                            TaxCode = "DU",
                                        });
                                    }
                                }
                                );
                            });
                            recommendations.Add(ida);
                        }
                    }
                }

                #endregion

                availability = new Availability()
                {
                    Id = Guid.NewGuid().ToString(),
                    Recommendations = recommendations,
                    DataList = new DataList()
                    {
                        Journeys = journeys,
                        FlightSegments = segments,
                        PriceClasses = priceClasses,
                        FareFamilies = fareFamilies.Distinct().ToList(),
                        ServiceItems = serviceItems,
                        BaggageInfos = baggageInfos,
                        Airports = airports,
                        Airlines = airlines,
                    },
                };

            }
            catch (Exception ex)
            {
                _logger.Log(KissLog.LogLevel.Error, ex, "AZUL");
                return new Availability();
            }

            _authenticationAzulService.LogOff(session);
            return availability;
        }
    }
}