using flights.crosscutting.DomainObjects;
using flights.crosscutting.Utils;
using flights.domain.Models;
using flights.domain.Models.Availability;
using flights.domain.Models.GetPrice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace flights.application.Services
{
    public partial class AvailabilityProviderService
    {
        private Availability ToAvailabilityGol(AvailabilityRS availabilityResponse, SessionProvider session)
        {
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

            List<PriceKeys> priceKeys = new List<PriceKeys>();
            string carrier = string.Empty;

            try
            {

                var searchRs = availabilityResponse.Availability.GetType().GetProperty("OTA_AirLowFareSearchRS")?.GetValue(availabilityResponse.Availability, null);

                #region Recomendação

                var pricedItinerariesObject = searchRs.GetType().GetProperty("PricedItineraries").GetValue(searchRs, null);
                IEnumerable pricedItineraryies = pricedItinerariesObject.GetType().GetProperty("PricedItinerary").GetValue(pricedItinerariesObject, null) as IEnumerable;

                foreach (var pricedItinerary in pricedItineraryies)
                {
                    var faresEObject = pricedItinerary.GetType().GetProperty("TPA_Extensions").GetValue(pricedItinerary, null);

                    IEnumerable AddFaresObject = faresEObject.GetType().GetProperty("AdditionalFares").GetValue(faresEObject, null) as IEnumerable;
                    if (AddFaresObject != null)
                    {
                        var airItinerary = pricedItinerary.GetType().GetProperty("AirItinerary").GetValue(pricedItinerary, null);
                        string sequence = pricedItinerary.GetType().GetProperty("SequenceNumber").GetValue(pricedItinerary, null).ToString();

                        List<Journey> journeysTemp = new List<Journey>();
                        Recommendation recommendationRS = new Recommendation()
                        {
                            Offers = new List<Offer>(),
                            Provider = "GOL",
                            SequenceNumber = sequence
                        };

                        IEnumerable jorneys = airItinerary.GetType().GetProperty("OriginDestinationOptions").GetValue(airItinerary, null) as IEnumerable;

                        #region Jornada

                        List<FlightPriceClassAssociation> flightPriceClasses = new List<FlightPriceClassAssociation>();
                        List<string> ids = new List<string>();
                        bool ida = true;
                        foreach (var jorney in jorneys)
                        {
                            Journey journeyRS = new Journey()
                            {
                                Id = "JRN-",
                                FlightSegmentsIds = new List<string>()
                            };

                            #region Seguimento

                            IEnumerable segmentList = jorney.GetType().GetProperty("FlightSegment").GetValue(jorney, null) as IEnumerable;

                            foreach (var segment in segmentList)
                            {
                                var marketingAirline = segment.GetType().GetProperty("MarketingAirline").GetValue(segment, null);
                                carrier = marketingAirline.GetType().GetProperty("Code").GetValue(marketingAirline, null).ToString().ToUpper();
                                DateTime departTime = DateTime.Parse(segment.GetType().GetProperty("DepartureDateTime").GetValue(segment, null).ToString());
                                string flightNumber = segment.GetType().GetProperty("FlightNumber").GetValue(segment, null).ToString();
                                string marriageGrp = segment.GetType().GetProperty("MarriageGrp").GetValue(segment, null).ToString();

                                var idSeguiment = carrier + flightNumber + departTime.ToString("ddMMM").ToUpper();

                                if (!segments.Any(x => x.Id == idSeguiment))
                                {
                                    DateTime arrTime = DateTime.Parse(segment.GetType().GetProperty("ArrivalDateTime").GetValue(segment, null).ToString());

                                    IEnumerable equipament = segment.GetType().GetProperty("Equipment").GetValue(segment, null) as IEnumerable;

                                    string equip = string.Empty;
                                    foreach (var t in equipament)
                                    {
                                        equip = t.GetType().GetProperty("AirEquipType").GetValue(t, null).ToString();
                                        break;
                                    }

                                    var arrival = segment.GetType().GetProperty("ArrivalAirport").GetValue(segment, null);
                                    var departure = segment.GetType().GetProperty("DepartureAirport").GetValue(segment, null);

                                    FlightSegment segmentRS = new FlightSegment()
                                    {
                                        Id = idSeguiment,
                                        ArrivalCode = arrival.GetType().GetProperty("LocationCode").GetValue(arrival, null).ToString().ToUpper(),
                                        DepartureCode = departure.GetType().GetProperty("LocationCode").GetValue(departure, null).ToString().ToUpper(),
                                        DepartureDateTime = departTime,
                                        ArrivalDateTime = arrTime,
                                        MarketingCarrierCode = carrier,
                                        OperationCarrierCode = carrier,
                                        FlightNumber = int.Parse(flightNumber),
                                        Aircraft = equip,
                                        Duration = (int)arrTime.Subtract(departTime).TotalMinutes,
                                        ResBookDesigCode = segment.GetType().GetProperty("ResBookDesigCode").GetValue(segment, null).ToString(),
                                        MarriageGrp = marriageGrp
                                    };


                                    segments.Add(segmentRS);

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

                                }

                                if (!journeyRS.FlightSegmentsIds.Any(x => x == idSeguiment))
                                {
                                    journeyRS.Id += idSeguiment;
                                    journeyRS.FlightSegmentsIds.Add(idSeguiment);
                                }

                                flightPriceClasses.Add(new FlightPriceClassAssociation()
                                {
                                    FlightSegmentId = idSeguiment,
                                    Directionality = ida ? "FROM" : "TO"
                                });

                                var jrnIds = new List<string>();
                                if (!jrnIds.Contains(journeyRS.Id))
                                {
                                    jrnIds.Add(journeyRS.Id);
                                }
                            }

                            #endregion

                            if (!journeys.Any(x => x.Id == journeyRS.Id))
                            {
                                var segmentInicio = segments.Where(x => journeyRS.Id.Contains(x.Id)).OrderBy(x => x.DepartureDateTime).FirstOrDefault();
                                var segmentFim = segments.Where(x => journeyRS.Id.Contains(x.Id)).OrderByDescending(x => x.DepartureDateTime).FirstOrDefault();
                                journeyRS.DepartureDateTime = (DateTime)segmentInicio?.DepartureDateTime;
                                journeyRS.ArrivalDateTime = (DateTime)segmentFim?.ArrivalDateTime;
                                journeyRS.Duration = (int)journeyRS.ArrivalDateTime.Subtract(journeyRS.DepartureDateTime).TotalMinutes;

                                journeyRS.DepartureCode = segmentInicio?.DepartureCode;
                                journeyRS.ArrivalCode = segmentFim?.ArrivalCode;

                                journeys.Add(journeyRS);
                            }

                            journeysTemp.Add(journeyRS);

                            ida = false;
                        }

                        #endregion

                        #region Oferta

                        foreach (var AddFare in AddFaresObject)
                        {
                            var fare = AddFare.GetType().GetProperty("AirItineraryPricingInfo").GetValue(AddFare, null);

                            if ((bool)fare.GetType().GetProperty("FareReturned").GetValue(fare, null))
                            {
                                #region base tarifária

                                List<OfferItem> offersItens = new List<OfferItem>();
                                string className = "";

                                #region familia

                                string pClass = fare.GetType().GetProperty("BrandID").GetValue(fare, null).ToString().ToUpper();
                                IEnumerable faresIn = fare.GetType().GetProperty("FareInfos").GetValue(fare, null) as IEnumerable;
                                string mealCode;
                                foreach (var faresInf in faresIn)
                                {
                                    var faresEIn = faresInf.GetType().GetProperty("TPA_Extensions").GetValue(faresInf, null);
                                    var meal = faresEIn.GetType().GetProperty("Meal").GetValue(faresEIn, null);
                                    if (meal != null)
                                    {
                                        mealCode = meal.GetType().GetProperty("Code").GetValue(meal, null).ToString();
                                    }
                                    break;
                                }

                                var services = new List<Service>();

                                var refund = new Refund
                                {
                                    AllowsRefund = false,
                                    Percentage = 0
                                };
                                bool allowsExchange = false;

                                bool allowsSeat = false;
                                decimal after = 0.0M;
                                decimal before = 0.0M;

                                var seguimentIds = new List<string>();
                                journeysTemp.ForEach(x => seguimentIds.AddRange(x.FlightSegmentsIds));
                                switch (pClass)
                                {
                                    case "PO":
                                        className = "PROMO";
                                        services.Add(new Service
                                        {
                                            BaggageId = "BAG-0-0KG",
                                            IncludedWithOfferItem = true,
                                            FlightSegmentsIds = seguimentIds
                                        });
                                        break;

                                    case "LT":
                                        className = "LIGHT";
                                        services.Add(new Service
                                        {
                                            BaggageId = "BAG-0-0KG",
                                            IncludedWithOfferItem = true,
                                            FlightSegmentsIds = seguimentIds
                                        });
                                        allowsExchange = true;
                                        before = 275.0M;
                                        after = 350.0M;
                                        break;
                                    case "PL":
                                        className = "PLUS";
                                        allowsSeat = true;
                                        services.Add(new Service
                                        {
                                            BaggageId = "BAG-1-23KG",
                                            IncludedWithOfferItem = true,
                                            FlightSegmentsIds = seguimentIds
                                        });
                                        refund.AllowsRefund = true;
                                        refund.Percentage = 40;
                                        allowsExchange = true;
                                        before = 250.0M;
                                        after = 330.0M;
                                        break;
                                    case "MX":
                                        className = "MAX";
                                        allowsSeat = true;
                                        services.Add(new Service
                                        {
                                            BaggageId = "BAG-2-23KG",
                                            IncludedWithOfferItem = true,
                                            FlightSegmentsIds = seguimentIds
                                        });
                                        allowsExchange = true;
                                        refund.AllowsRefund = true;
                                        refund.Percentage = 95;
                                        break;
                                    case "PE":
                                        className = "PREMIUM";
                                        services.Add(new Service
                                        {
                                            BaggageId = "BAG-2-23KG",
                                            IncludedWithOfferItem = true,
                                            FlightSegmentsIds = seguimentIds
                                        });
                                        break;
                                    case "EX":
                                        className = "EXECUTIVA";
                                        services.Add(new Service
                                        {
                                            BaggageId = "BAG-2-23KG",
                                            IncludedWithOfferItem = true,
                                            FlightSegmentsIds = seguimentIds
                                        });
                                        break;
                                }

                                var fareFamily = new FareFamily()
                                {
                                    Id = carrier + "-" + className,
                                    Code = pClass,
                                    Owner = "G3-GWS",
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
                                };

                                if (!fareFamilies.Any(x => x.Id == fareFamily.Id))
                                {
                                    fareFamilies.Add(fareFamily);
                                }

                                #endregion

                                IEnumerable ptcFares = fare.GetType().GetProperty("PTC_FareBreakdowns").GetValue(fare, null) as IEnumerable;

                                foreach (var ptcFare in ptcFares)
                                {
                                    #region Classe
                                    IEnumerable fareBasisCodes = ptcFare.GetType().GetProperty("FareBasisCodes").GetValue(ptcFare, null) as IEnumerable;

                                    foreach (var baseCode in fareBasisCodes)
                                    {
                                        carrier = baseCode.GetType().GetProperty("GovCarrier").GetValue(baseCode, null).ToString().ToUpper();
                                        string id = carrier
                                            + "-" + baseCode.GetType().GetProperty("BookingCode").GetValue(baseCode, null).ToString().ToUpper()
                                            + "-" + baseCode.GetType().GetProperty("Value").GetValue(baseCode, null).ToString().ToUpper();


                                        ids.Add(id);
                                        foreach (var flightPriceClass in flightPriceClasses.Where(x => x.Directionality == baseCode.GetType().GetProperty("FareComponentDirectionality").GetValue(baseCode, null).ToString().ToUpper()))
                                        {
                                            flightPriceClass.PriceClassId = id;
                                        }

                                        if (!priceClasses.Any(x => x.Id == id))
                                        {
                                            priceClasses.Add(new PriceClass()
                                            {
                                                Id = id,
                                                ClassOfService = baseCode.GetType().GetProperty("BookingCode").GetValue(baseCode, null).ToString().ToUpper(),
                                                FareBasis = baseCode.GetType().GetProperty("Value").GetValue(baseCode, null).ToString().ToUpper()
                                            });
                                        }
                                    }
                                    #endregion

                                    #region Ítens da oferta (por pax)

                                    var passengerTypeQuantity = ptcFare.GetType().GetProperty("PassengerTypeQuantity").GetValue(ptcFare, null);

                                    var offerIten = new OfferItem()
                                    {
                                        Ptc = passengerTypeQuantity.GetType().GetProperty("Code").GetValue(passengerTypeQuantity, null).ToString() == "CNN" ? "CHD" : passengerTypeQuantity.GetType().GetProperty("Code").GetValue(passengerTypeQuantity, null).ToString(),
                                        BaseFare = new BaseFare(),
                                        EquivalentFare = new EquivalentFare(),
                                        Services = services,
                                        Taxes = new List<Tax>(),
                                        TotalPrice = new TotalPrice()
                                    };


                                    var passengerFare = ptcFare.GetType().GetProperty("PassengerFare").GetValue(ptcFare, null);

                                    var baseFare = passengerFare.GetType().GetProperty("BaseFare").GetValue(passengerFare, null);

                                    offerIten.BaseFare.Amount = Convert.ToDecimal(baseFare.GetType().GetProperty("Amount").GetValue(baseFare, null));
                                    offerIten.BaseFare.CurrencyCode = "BRL";

                                    var equivFare = passengerFare.GetType().GetProperty("EquivFare").GetValue(passengerFare, null);

                                    offerIten.EquivalentFare.Amount = Convert.ToDecimal(equivFare.GetType().GetProperty("Amount").GetValue(equivFare, null));
                                    offerIten.EquivalentFare.CurrencyCode = "BRL";

                                    var totalFare = passengerFare.GetType().GetProperty("TotalFare").GetValue(passengerFare, null);

                                    offerIten.TotalPrice.Amount = Convert.ToDecimal(totalFare.GetType().GetProperty("Amount").GetValue(totalFare, null));
                                    offerIten.TotalPrice.CurrencyCode = "BRL";

                                    var taxes = passengerFare.GetType().GetProperty("Taxes").GetValue(passengerFare, null);
                                    IEnumerable fareComponentsTaxes = taxes.GetType().GetProperty("Tax").GetValue(taxes, null) as IEnumerable;

                                    foreach (var fareComponentTaxe in fareComponentsTaxes)
                                    {
                                        offerIten.Taxes.Add(new Tax()
                                        {
                                            TaxCode = fareComponentTaxe.GetType().GetProperty("TaxCode").GetValue(fareComponentTaxe, null).ToString(),
                                            TaxAmount = new TaxAmount()
                                            {
                                                Amount = Convert.ToDecimal(fareComponentTaxe.GetType().GetProperty("Amount").GetValue(fareComponentTaxe, null)),
                                                CurrencyCode = "BRL"
                                            },
                                        });
                                    }

                                    offersItens.Add(offerIten);

                                    #endregion

                                }

                                recommendationRS.Offers.Add(new Offer()
                                {
                                    Id = carrier + "-" + Guid.NewGuid().ToString(),
                                    Owner = "G3-GWS",
                                    OfferAssociations = new OfferAssociations()
                                    {
                                        JourneyIds = journeysTemp.Select(x => x.Id).ToList(),
                                        FareFamilyId = carrier + "-" + className,
                                        FlightPriceClassAssociations = flightPriceClasses,
                                        CredentialId = session.CredentialId
                                    },
                                    OfferItems = offersItens
                                });

                                #endregion
                            }
                        }
                        #endregion

                        recommendations.Add(recommendationRS);
                    }
                }

                #endregion

                Availability availability = new Availability()
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

                return availability;
            }
            catch (Exception ex)
            {
                var searchRs = availabilityResponse.Availability.GetType().GetProperty("OTA_AirLowFareSearchRS").GetValue(availabilityResponse.Availability, null);
                string errosLatam = string.Empty;
                IEnumerable items = searchRs.GetType().GetProperty("Items").GetValue(searchRs, null) as IEnumerable;

                foreach (var item in items)
                {
                    IEnumerable errors = item.GetType().GetProperty("Error").GetValue(item, null) as IEnumerable;
                    foreach (var error in errors)
                    {
                        errosLatam += "\n" + error.GetType().GetProperty("Code").GetValue(error, null).ToString() + ": " + error.GetType().GetProperty("ShortText").GetValue(error, null).ToString();
                    }
                }
                _logger.Log(KissLog.LogLevel.Error, errosLatam, "Gol");

                return new Availability();
            }
        }


        private Availability ToAvailabilityGolBws(AvailabilityRS availabilityResponse, SessionProvider session)
        {
            Availability availability = new Availability();

            List<Recommendation> recommendations = new List<Recommendation>();
            List<Recommendation> recommendationsVolta = new List<Recommendation>();
            List<Recommendation> recommendationsIda = new List<Recommendation>();

            List<FlightSegment> segments = new List<FlightSegment>();
            List<Journey> journeys = new List<Journey>();
            List<PriceClass> priceClasses = new List<PriceClass>();
            List<FareFamily> fareFamilies = new List<FareFamily>();
            List<Airport> airports = new List<Airport>();
            List<Airline> airlines = new List<Airline>();

            try
            {
                var obj = availabilityResponse.Availability.GetType().GetProperty("Schedules");

                IEnumerable schedulesObject = availabilityResponse.Availability.GetType().GetProperty("Schedules").GetValue(availabilityResponse.Availability, null) as IEnumerable;

                bool isReturn = false;
                foreach (var s in schedulesObject)
                {
                    IEnumerable sObject = s as IEnumerable;
                    foreach (var scheduleObject in sObject)
                    {
                        IEnumerable journeysObject = scheduleObject.GetType().GetProperty("Journeys").GetValue(scheduleObject, null) as IEnumerable;

                        foreach (var journey in journeysObject)
                        {
                            #region Recomendação

                            Recommendation recommendationRS = new Recommendation()
                            {
                                Offers = new List<Offer>(),
                                Provider = "GOL"
                            };

                            #region Jornada

                            Journey journeyRS = new Journey()
                            {
                                Id = "JRN-",
                                ArrivalCode = scheduleObject.GetType().GetProperty("ArrivalStation").GetValue(scheduleObject, null).ToString().ToUpper(),
                                DepartureCode = scheduleObject.GetType().GetProperty("DepartureStation").GetValue(scheduleObject, null).ToString().ToUpper(),
                                FlightSegmentsIds = new List<string>(),
                                SellKey = journey.GetType().GetProperty("JourneySellKey").GetValue(journey, null).ToString()
                            };

                            #region Seguimento

                            //Lógica para impedir de duplicar as ofertas caso tenha paradas
                            bool segInicial = true;

                            IEnumerable segmentsObject = journey.GetType().GetProperty("Segments").GetValue(journey, null) as IEnumerable;
                            foreach (var segment in segmentsObject)
                            {
                                List<FlightPriceClassAssociation> flightPriceClasses = new List<FlightPriceClassAssociation>();

                                int i = 0;
                                IEnumerable legsObject = segment.GetType().GetProperty("Legs").GetValue(segment, null) as IEnumerable;
                                foreach (var leg in legsObject)
                                {
                                    DateTime departTime = (DateTime)leg.GetType().GetProperty("STD").GetValue(leg, null);
                                    DateTime arrTime = (DateTime)leg.GetType().GetProperty("STA").GetValue(leg, null);
                                    var flightDesignator = segment.GetType().GetProperty("FlightDesignator").GetValue(segment, null);

                                    var legInfo = leg.GetType().GetProperty("LegInfo").GetValue(leg, null);

                                    string complementoId = i == 0 ? "" : i.ToString();
                                    FlightSegment segmentRS = new FlightSegment()
                                    {
                                        Id = flightDesignator.GetType().GetProperty("CarrierCode").GetValue(flightDesignator, null).ToString().ToUpper() + flightDesignator.GetType().GetProperty("FlightNumber").GetValue(flightDesignator, null).ToString() + departTime.ToString("ddMMM").ToUpper() + complementoId,
                                        ArrivalCode = leg.GetType().GetProperty("ArrivalStation").GetValue(leg, null).ToString().ToUpper(),
                                        DepartureCode = leg.GetType().GetProperty("DepartureStation").GetValue(leg, null).ToString().ToUpper(),
                                        DepartureDateTime = departTime,
                                        ArrivalDateTime = arrTime,
                                        MarketingCarrierCode = flightDesignator.GetType().GetProperty("CarrierCode").GetValue(flightDesignator, null).ToString().ToUpper(),
                                        OperationCarrierCode = flightDesignator.GetType().GetProperty("CarrierCode").GetValue(flightDesignator, null).ToString().ToUpper(),
                                        FlightNumber = int.Parse(flightDesignator.GetType().GetProperty("FlightNumber").GetValue(flightDesignator, null).ToString()),
                                        Aircraft = legInfo.GetType().GetProperty("EquipmentType").GetValue(legInfo, null).ToString(),
                                        Duration = (int)arrTime.Subtract(departTime).TotalMinutes
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

                                IEnumerable faresObject = segment.GetType().GetProperty("Fares").GetValue(segment, null) as IEnumerable;

                                foreach (var fare in faresObject)
                                {
                                    string carrier = fare.GetType().GetProperty("CarrierCode").GetValue(fare, null).ToString().ToUpper();

                                    #region familia

                                    string pClass = fare.GetType().GetProperty("ProductClass").GetValue(fare, null).ToString().ToUpper();
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

                                    bool allowsSeat = false;
                                    bool allowsExchange = false;
                                    decimal after = 0.0M;
                                    decimal before = 0.0M;

                                    switch (pClass)
                                    {
                                        case "PO":
                                            className = "PROMO";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-0-0KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            break;
                                        case "LT":
                                            className = "LIGHT";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-0-0KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            allowsExchange = true;
                                            before = 275.0M;
                                            after = 350.0M;
                                            break;
                                        case "PL":
                                            className = "PLUS";
                                            allowsSeat = true;
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-1-23KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            refund.AllowsRefund = true;
                                            refund.Percentage = 40;
                                            allowsExchange = true;
                                            before = 250.0M;
                                            after = 330.0M;
                                            break;
                                        case "MX":
                                            className = "MAX";
                                            allowsSeat = true;
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-2-23KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            allowsExchange = true;
                                            refund.AllowsRefund = true;
                                            refund.Percentage = 95;
                                            break;
                                        case "PE":
                                            className = "PREMIUM";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-2-23KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            break;
                                        case "EX":
                                            className = "EXECUTIVA";
                                            services.Add(new Service
                                            {
                                                BaggageId = "BAG-2-23KG",
                                                IncludedWithOfferItem = true,
                                                FlightSegmentsIds = journeyRS.FlightSegmentsIds
                                            });
                                            break;
                                    }

                                    if (!fareFamilies.Any(x => x.Id == carrier + "-" + className))
                                    {
                                        fareFamilies.Add(new FareFamily()
                                        {
                                            Id = carrier + "-" + className,
                                            Code = pClass,
                                            Owner = "G3-BWS",
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
                                        });
                                    }

                                    #endregion

                                    #region base tarifária


                                    string fareBase = fare.GetType().GetProperty("FareBasisCode").GetValue(fare, null).ToString().ToUpper();
                                    string id = carrier
                                            + "-" + fare.GetType().GetProperty("ClassOfService").GetValue(fare, null).ToString().ToUpper()
                                            + "-" + fareBase;

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
                                            FareBasis = fare.GetType().GetProperty("FareBasisCode").GetValue(fare, null).ToString().ToUpper(),
                                            SellKey = fare.GetType().GetProperty("FareSellKey").GetValue(fare, null).ToString()
                                        });
                                    }
                                    #endregion


                                    #region Ítens da oferta (por pax)

                                    List<OfferItem> offersItens = new List<OfferItem>();
                                    IEnumerable paxFaresObject = fare.GetType().GetProperty("PaxFares").GetValue(fare, null) as IEnumerable;

                                    foreach (var paxFare in paxFaresObject)
                                    {
                                        IEnumerable serviceChanges = paxFare.GetType().GetProperty("ServiceCharges").GetValue(paxFare, null) as IEnumerable;
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
                                                    offerIten.BaseFare.Amount = (decimal)serviceChange.GetType().GetProperty("Amount").GetValue(serviceChange, null);
                                                    offerIten.BaseFare.CurrencyCode = serviceChange.GetType().GetProperty("CurrencyCode").GetValue(serviceChange, null).ToString();
                                                }
                                                if (chargeType == "Tax")
                                                {
                                                    offerIten.Taxes.Add(new Tax()
                                                    {
                                                        TaxCode = chargeType,
                                                        TaxAmount = new TaxAmount()
                                                        {
                                                            Amount = (decimal)serviceChange.GetType().GetProperty("Amount").GetValue(serviceChange, null),
                                                            CurrencyCode = serviceChange.GetType().GetProperty("CurrencyCode").GetValue(serviceChange, null).ToString()
                                                        },
                                                    });
                                                }
                                                if (chargeType == "Discount")
                                                {
                                                    offerIten.BaseFare.Amount -= (decimal)serviceChange.GetType().GetProperty("Amount").GetValue(serviceChange, null);
                                                }
                                            }

                                            offersItens.Add(offerIten);
                                        }
                                    }

                                    #endregion

                                    if (segInicial)
                                    {
                                        recommendationRS.Offers.Add(new Offer()
                                        {
                                            Owner = "G3-BWS",
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
                                    else
                                    {
                                        foreach (var o in recommendationRS.Offers)
                                        {
                                            if (o.OfferAssociations.FlightPriceClassAssociations.Any(x => x.PriceClassId == id))
                                            {
                                                o.OfferAssociations.FlightPriceClassAssociations.AddRange(ExtensionMethods.CloneObject(flightPriceClasses));
                                                o.OfferAssociations.JourneyIds.Clear();
                                                o.OfferAssociations.JourneyIds = jrnIds;
                                            }
                                        }
                                    }
                                }

                                #endregion

                                segInicial = false;
                            }
                            #endregion

                            var segmentInicio = segments.Where(x => journeyRS.Id.Contains(x.Id)).OrderBy(x => x.DepartureDateTime).FirstOrDefault();
                            var segmentFim = segments.Where(x => journeyRS.Id.Contains(x.Id)).OrderByDescending(x => x.DepartureDateTime).FirstOrDefault();

                            journeyRS.DepartureDateTime = (DateTime)segmentInicio?.DepartureDateTime;
                            journeyRS.ArrivalDateTime = (DateTime)segmentFim?.ArrivalDateTime;
                            journeyRS.DepartureCode = segmentInicio?.DepartureCode;
                            journeyRS.ArrivalCode = segmentFim?.ArrivalCode;
                            journeyRS.Duration = (int)journeyRS.ArrivalDateTime.Subtract(journeyRS.DepartureDateTime).TotalMinutes;


                            journeys.Add(journeyRS);

                            #endregion

                            recommendations.Add(recommendationRS);

                            #endregion
                        }

                        if (!isReturn)
                        {
                            recommendationsIda = ExtensionMethods.CloneObject(recommendations);
                        }
                        else
                        {
                            recommendationsVolta = ExtensionMethods.CloneObject(recommendations);
                        }
                        recommendations = new List<Recommendation>();
                        isReturn = true;

                    }

                }

                #region agrupar roundTrip

                recommendations = new List<Recommendation>();

                #region Tarifar a primeira ida para saber se tem DU

                bool hasDU = false;

                var jorneysSell = new List<JourneySell>();
                var paxT = new List<PassangerSell>();
                paxT.Add(new PassangerSell { PaxType = recommendationsIda.FirstOrDefault()?.Offers.FirstOrDefault()?.OfferItems.FirstOrDefault()?.Ptc });

                jorneysSell.Add(new JourneySell
                {
                    FareSellKey = priceClasses.FirstOrDefault(x => x.Id == recommendationsIda.FirstOrDefault()?.Offers.FirstOrDefault()?.OfferAssociations.FlightPriceClassAssociations.FirstOrDefault()?.PriceClassId)?.SellKey,
                    JourneySellKey = journeys.FirstOrDefault(x => x.Id == recommendationsIda.FirstOrDefault()?.Offers.FirstOrDefault()?.OfferAssociations.JourneyIds.FirstOrDefault()).SellKey
                });

                GetPriceRQ getPriceRQ = new GetPriceRQ
                {
                    Passagers = paxT,
                    JorneysSell = jorneysSell,
                    CurrencyCode = recommendationsIda.FirstOrDefault()?.Offers.FirstOrDefault()?.OfferItems.FirstOrDefault()?.BaseFare.CurrencyCode,
                };
                try
                {

                    var price = _bookingGolService.PriceItinerary(session, getPriceRQ);

                    IEnumerable priceJorneys = price.Price.GetType().GetProperty("Journeys").GetValue(price.Price, null) as IEnumerable;
                    foreach (var priceJorney in priceJorneys)
                    {
                        IEnumerable priceSegments = priceJorney.GetType().GetProperty("Segments").GetValue(priceJorney, null) as IEnumerable;
                        foreach (var priceSegment in priceSegments)
                        {
                            IEnumerable priceFares = priceSegment.GetType().GetProperty("Fares").GetValue(priceSegment, null) as IEnumerable;
                            foreach (var priceFare in priceFares)
                            {
                                IEnumerable pricePaxFares = priceFare.GetType().GetProperty("PaxFares").GetValue(priceFare, null) as IEnumerable;
                                foreach (var pricePaxFare in pricePaxFares)
                                {
                                    IEnumerable priceServiceCharges = pricePaxFare.GetType().GetProperty("ServiceCharges").GetValue(pricePaxFare, null) as IEnumerable;
                                    foreach (var serviceCh in priceServiceCharges)
                                    {
                                        if (serviceCh.GetType().GetProperty("TicketCode").GetValue(serviceCh, null).ToString() == "DU")
                                        {
                                            hasDU = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }


                #endregion

                foreach (var ida in recommendationsIda)
                {
                    //var bkpJourneysIds = ida.Offers.FirstOrDefault()?.OfferAssociations.JourneyIds.ToList();
                    if (recommendationsVolta.Count > 0)
                    {
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

                                        bkpOferIda.Id = "G3-" + Guid.NewGuid().ToString();
                                        newRec.Offers.Add(bkpOferIda);

                                        #endregion
                                    }
                                }
                            }
                            if (newRec.Offers != null && newRec.Offers.Count > 0 && !newRec.Offers.Any(x => x.OfferItems.Any(y => y.Taxes.Count == 0)))
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
                                offer.Id = "G3-" + Guid.NewGuid().ToString();
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
                        Airports = airports,
                        Airlines = airlines,
                    },
                };
            }
            catch (Exception ex)
            {
                _logger.Log(KissLog.LogLevel.Error, ex, "GOL");
                return new Availability();
            }
            return availability;
        }

    }
}