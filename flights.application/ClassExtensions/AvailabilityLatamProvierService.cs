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
        private Availability ToAvailabilityLatam(AvailabilityRS availabilityResponse, SessionProvider session)
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
                var searchRs = availabilityResponse.Availability.GetType().GetProperty("OTA_AirLowFareSearchRS").GetValue(availabilityResponse.Availability, null);

                #region Recomendação

                var pricedItinerariesObject = searchRs.GetType().GetProperty("PricedItineraries").GetValue(searchRs, null);
                IEnumerable pricedItineraryies = pricedItinerariesObject.GetType().GetProperty("PricedItinerary").GetValue(pricedItinerariesObject, null) as IEnumerable;

                foreach (var pricedItinerary in pricedItineraryies)
                {
                    var airItinerary = pricedItinerary.GetType().GetProperty("AirItinerary").GetValue(pricedItinerary, null);
                    string sequence = pricedItinerary.GetType().GetProperty("SequenceNumber").GetValue(pricedItinerary, null).ToString();

                    List<Journey> journeysTemp = new List<Journey>();
                    Recommendation recommendationRS = new Recommendation()
                    {
                        Offers = new List<Offer>(),
                        Provider = "LATAM",
                        SequenceNumber = sequence
                    };

                    IEnumerable jorneys = airItinerary.GetType().GetProperty("OriginDestinationOptions").GetValue(airItinerary, null) as IEnumerable;

                    #region Jornada

                    List<FlightPriceClassAssociation> flightPriceClasses = new List<FlightPriceClassAssociation>();

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

                    var faresEObject = pricedItinerary.GetType().GetProperty("TPA_Extensions").GetValue(pricedItinerary, null);

                    IEnumerable AddFaresObject = faresEObject.GetType().GetProperty("AdditionalFares").GetValue(faresEObject, null) as IEnumerable;

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
                                case "SN":
                                    className = "PROMO";
                                    services.Add(new Service
                                    {
                                        BaggageId = "BAG-0-0KG",
                                        IncludedWithOfferItem = true,
                                        FlightSegmentsIds = seguimentIds
                                    });
                                    break;
                                case "SL":
                                    className = "LIGHT";
                                    services.Add(new Service
                                    {
                                        BaggageId = "BAG-0-0KG",
                                        IncludedWithOfferItem = true,
                                        FlightSegmentsIds = seguimentIds
                                    });
                                    allowsExchange = true;
                                    after = 360.0M;
                                    before = 275.0M;
                                    break;
                                case "SE":
                                    className = "PLUS";
                                    services.Add(new Service
                                    {
                                        BaggageId = "BAG-1-23KG",
                                        IncludedWithOfferItem = true,
                                        FlightSegmentsIds = seguimentIds
                                    });
                                    refund.AllowsRefund = true;
                                    refund.Percentage = 40;

                                    allowsExchange = true;
                                    after = 340.0M;
                                    before = 250.0M;
                                    break;
                                case "SF":
                                    className = "TOP";
                                    services.Add(new Service
                                    {
                                        BaggageId = "BAG-2-23KG",
                                        IncludedWithOfferItem = true,
                                        FlightSegmentsIds = seguimentIds
                                    });
                                    refund.AllowsRefund = true;
                                    refund.Percentage = 100;
                                    allowsSeat = true;
                                    allowsExchange = true;
                                    break;
                                case "RA":
                                    className = "PREMIUM ECONOMY PLUS";
                                    services.Add(new Service
                                    {
                                        BaggageId = "BAG-1-23KG",
                                        IncludedWithOfferItem = true,
                                        FlightSegmentsIds = seguimentIds
                                    });
                                    allowsExchange = true;
                                    before = 250.0M;
                                    before = 340.0M;
                                    break;
                                case "RY":
                                    className = "PREMIUM ECONOMY TOP";
                                    services.Add(new Service
                                    {
                                        BaggageId = "BAG-3-23KG",
                                        IncludedWithOfferItem = true,
                                        FlightSegmentsIds = seguimentIds
                                    });
                                    allowsExchange = true;

                                    allowsExchange = true;
                                    break;
                                case "EV":
                                    className = "PREMIUM BUSINESS PLUS";
                                    services.Add(new Service
                                    {
                                        BaggageId = "BAG-1-23KG",
                                        IncludedWithOfferItem = true,
                                        FlightSegmentsIds = seguimentIds
                                    });
                                    break;
                                case "EJ":
                                    className = "PREMIUM BUSINESS TOP";
                                    services.Add(new Service
                                    {
                                        BaggageId = "BAG-3-23KG",
                                        IncludedWithOfferItem = true,
                                        FlightSegmentsIds = seguimentIds
                                    });
                                    refund.AllowsRefund = true;
                                    refund.Percentage = 40;

                                    allowsExchange = true;
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
                                offerIten.BaseFare.CurrencyCode = "BRL";// baseFare.GetType().GetProperty("CurrencyCode").GetValue(baseFare, null).ToString();

                                var equivFare = passengerFare.GetType().GetProperty("EquivFare").GetValue(passengerFare, null);

                                offerIten.EquivalentFare.Amount = Convert.ToDecimal(equivFare.GetType().GetProperty("Amount").GetValue(equivFare, null));
                                offerIten.EquivalentFare.CurrencyCode = "BRL";//equivFare.GetType().GetProperty("CurrencyCode").GetValue(equivFare, null).ToString();

                                var totalFare = passengerFare.GetType().GetProperty("TotalFare").GetValue(passengerFare, null);

                                offerIten.TotalPrice.Amount = Convert.ToDecimal(totalFare.GetType().GetProperty("Amount").GetValue(totalFare, null));
                                offerIten.TotalPrice.CurrencyCode = "BRL";//totalFare.GetType().GetProperty("CurrencyCode").GetValue(totalFare, null).ToString();

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
                                            CurrencyCode = "BRL"//fareComponentTaxe.GetType().GetProperty("CurrencyCode").GetValue(fareComponentTaxe, null).ToString()
                                        },
                                    });
                                }

                                offersItens.Add(offerIten);

                                #endregion

                            }

                            recommendationRS.Offers.Add(new Offer()
                            {
                                Id = carrier + "-" + Guid.NewGuid().ToString(),
                                Owner = carrier,
                                OfferAssociations = new OfferAssociations()
                                {
                                    JourneyIds = journeysTemp.Select(x => x.Id).ToList(),
                                    FareFamilyId = carrier + "-" + className,
                                    FlightPriceClassAssociations = ExtensionMethods.CloneObject(flightPriceClasses),
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
                _logger.Log(KissLog.LogLevel.Error, errosLatam, "LATAM");

                return new Availability();
            }
        }
    }
}