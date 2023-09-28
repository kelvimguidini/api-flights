using flights.domain.Models;
using flights.domain.Models.Availability;
using flights.domain.Models.Booking;
using flights.domain.Models.GetPrice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace flights.application.Services
{
    public partial class BookingProviderService
    {
        #region GWS
        private GetPriceRQ GeneratePriceRQGol(BookingDTO booking, Availability availability)
        {
            var jorneysSell = new List<JourneySell>();
            var paxT = _mapper.Map<List<PassangerSell>>(booking.Passengers);

            var rec = availability.Recommendations.FirstOrDefault(x => x.Offers.Any(o => o.Id == booking.OfferRefs.FirstOrDefault().OfferId));

            var offer = rec.Offers.FirstOrDefault(o => o.Id == booking.OfferRefs.FirstOrDefault().OfferId);

            foreach (var jorneyId in offer.OfferAssociations.JourneyIds)
            {
                var journey = availability.DataList.Journeys.FirstOrDefault(x => x.Id == jorneyId);

                var segments = availability.DataList.FlightSegments.Where(x => journey.FlightSegmentsIds.Contains(x.Id)).ToList();
                foreach (var segment in segments)
                {
                    jorneysSell.Add(new JourneySell
                    {
                        BrandID = availability.DataList.FareFamilies.FirstOrDefault(x => x.Id == offer.OfferAssociations.FareFamilyId)?.Code,
                        DepartureDateTime = segment.DepartureDateTime.AddHours(-3).ToString("s"),
                        ArrivalDateTime = segment.ArrivalDateTime.AddHours(-3).ToString("s"),
                        FlightNumber = segment.FlightNumber.ToString(),
                        NumberInParty = rec.SequenceNumber,
                        ResBookDesigCode = segment.ResBookDesigCode,
                        Status = "NN",
                        LocationCodeDestiation = segment.ArrivalCode,
                        LocationCodeOrigin = segment.DepartureCode,
                        MarketingCode = offer.Owner.Split('-').FirstOrDefault(),
                        MarriageGrp = segment.MarriageGrp
                    });
                }
            }

            return new GetPriceRQ
            {
                JorneysSell = jorneysSell,
                CurrencyCode = availability.Recommendations.FirstOrDefault()?.Offers.FirstOrDefault()?.OfferItems.FirstOrDefault()?.BaseFare.CurrencyCode,
                Passagers = paxT
            };
        }

        private Booking GetLocatorGol(string locator, BookingDTO bookingRQ, Availability availability, string offerId)
        {
            var offer = availability.Recommendations.FirstOrDefault(x => x.Offers.Any(y => y.Id == offerId))?.Offers.FirstOrDefault(y => y.Id == offerId);
            var jorneys = availability.DataList.Journeys.Where(x => offer.OfferAssociations.JourneyIds.Contains(x.Id)).ToList();
            List<FlightSegment> segments = availability.DataList.FlightSegments.Where(x => jorneys.Any(y => y.FlightSegmentsIds.Contains(x.Id))).ToList();
            var priceClasses = availability.DataList.PriceClasses.Where(x => offer.OfferAssociations.FlightPriceClassAssociations.Any(y => y.PriceClassId.Contains(x.Id))).ToList();
            var fareFamilies = availability.DataList.FareFamilies.Where(x => offer.OfferAssociations.FareFamilyId.Contains(x.Id)).ToList();
            var serviceItems = availability.DataList.ServiceItems.Where(x => offer.OfferItems.Any(y => y.Services.Any(z => z.ServiceItemId != null && z.ServiceItemId.Contains(x.Id)))).ToList();
            var baggageInfos = availability.DataList.BaggageInfos.Where(x => fareFamilies.Any(y => y.DescriptionInfo.BaggageId.Contains(x.Id))).ToList();
            var airports = availability.DataList.Airports.Where(x => segments.Any(y => y.ArrivalCode == x.AirportCode || y.DepartureCode == x.AirportCode)).ToList();
            var airlines = availability.DataList.Airlines.Where(x => x.AirlineCode == offer.Owner).ToList();

            Booking booking = new Booking()
            {
                OrderId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                OrderStatus = "CREATED",
                RecordLocator = locator.ToUpper(),
                Passengers = _mapper.Map<List<Passenger>>(bookingRQ.Passengers),
                OrderInfo = offer,
                DataList = new DataList()
                {
                    Journeys = jorneys,
                    FlightSegments = segments,
                    PriceClasses = priceClasses,
                    FareFamilies = fareFamilies,
                    ServiceItems = serviceItems,
                    BaggageInfos = baggageInfos,
                    Airports = airports,
                    Airlines = airlines,
                },
            };

            return booking;
        }

        private Booking BookingGol(string provider, SessionProvider session, BookingDTO booking, Availability availability)
        {
            var credentialParameters = _credentialRepository.GetCredentialParameters(session.CredentialId);

            var objSession = session.Session.GetType().GetProperty("Session").GetValue(session.Session, null);
            var objSegurity = objSession.GetType().GetProperty("Security").GetValue(objSession, null);
            string binarySecurityToken = objSegurity.GetType().GetProperty("BinarySecurityToken").GetValue(objSegurity, null).ToString();

            try
            {
                GetPriceRQ priceRQ = new GetPriceRQ();
                // LOGICA PARA MONTAR O REQUEST DO PriceItinerary
                priceRQ = GeneratePriceRQGol(booking, availability);

                var result = GetLocatorGol(_bookingGolService.Sell(session, priceRQ, credentialParameters), booking, availability, booking.OfferRefs.FirstOrDefault().OfferId);

                return result;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                _authenticationGolService.Logoff(credentialParameters, binarySecurityToken);
            }
        }
        #endregion

        #region BWS

        private Booking BookingGolBws(string provider, SessionProvider session, BookingDTO booking, Availability availability)
        {
            GetPriceRQ priceRQ = new GetPriceRQ();
            // LOGICA PARA MONTAR O REQUEST DO PriceItinerary
            priceRQ = GeneratePriceRQGolBws(booking, availability);
            var priceRS = _bookingGolService.PriceItinerary(session, priceRQ);

            //VALIDAR SE O PREÇO É O MESMO DA DISPONIBILIDADE
            if (ValidatePriceGol(priceRS, availability.Recommendations.FirstOrDefault(x => x.Offers.Any(y => y.Id == booking.OfferRefs.FirstOrDefault().OfferId))?.Offers.FirstOrDefault(y => y.Id == booking.OfferRefs.FirstOrDefault().OfferId)))
            {
                _notificator.notify("Erro obter tarifação Gol: Consulta expirou");
            }

            return GetLocatorGolBws(_bookingGolService.SellBws(session, priceRQ), booking, availability, booking.OfferRefs.FirstOrDefault().OfferId);
        }

        private GetPriceRQ GeneratePriceRQGolBws(BookingDTO booking, Availability availability)
        {
            var jorneysSell = new List<JourneySell>();
            var paxT = _mapper.Map<List<PassangerSell>>(booking.Passengers);

            var rec = availability.Recommendations.FirstOrDefault(x => x.Offers.Any(o => o.Id == booking.OfferRefs.FirstOrDefault().OfferId));

            var offer = rec.Offers.FirstOrDefault(o => o.Id == booking.OfferRefs.FirstOrDefault().OfferId);

            foreach (var jorneyId in offer.OfferAssociations.JourneyIds)
            {
                var journey = availability.DataList.Journeys.FirstOrDefault(x => x.Id == jorneyId);

                var segment = availability.DataList.FlightSegments.FirstOrDefault(x => x.Id == journey.FlightSegmentsIds.FirstOrDefault());
                jorneysSell.Add(new JourneySell
                {
                    FareSellKey = availability.DataList.PriceClasses.FirstOrDefault(x => x.Id == offer.OfferAssociations.FlightPriceClassAssociations.FirstOrDefault(y => y.FlightSegmentId == segment.Id)?.PriceClassId)?.SellKey,
                    JourneySellKey = journey.SellKey,
                });
            }

            return new GetPriceRQ
            {
                JorneysSell = jorneysSell,
                CurrencyCode = availability.Recommendations.FirstOrDefault()?.Offers.FirstOrDefault()?.OfferItems.FirstOrDefault()?.BaseFare.CurrencyCode,
                Passagers = paxT
            };
        }

        private bool ValidatePriceGol(GetPriceRS price, Offer offer)
        {

            var priceAvailabylity = offer.OfferItems.Where(x => x.Ptc == "ADT").Sum(x => x.TotalPrice.Amount);

            decimal totalPrice = 0;

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
                            if (pricePaxFare.GetType().GetProperty("PaxType").GetValue(pricePaxFare, null).ToString() == "ADT")
                            {
                                IEnumerable priceServiceCharges = pricePaxFare.GetType().GetProperty("ServiceCharges").GetValue(pricePaxFare, null) as IEnumerable;
                                foreach (var serviceCh in priceServiceCharges)
                                {
                                    if (serviceCh.GetType().GetProperty("TicketCode").GetValue(serviceCh, null).ToString() != "Discount")
                                    {
                                        totalPrice += (decimal)serviceCh.GetType().GetProperty("Amount").GetValue(serviceCh, null);
                                    }
                                    else
                                    {
                                        totalPrice -= (decimal)serviceCh.GetType().GetProperty("Amount").GetValue(serviceCh, null);
                                    }
                                }
                            }

                        }
                    }
                }
            }

            if (totalPrice != priceAvailabylity)
            {
                _logger.Log(KissLog.LogLevel.Warning, "Tarifação: " + totalPrice.ToString() + " - Disponibilidade: " + priceAvailabylity.ToString(), "GOL");
            }
            return totalPrice != priceAvailabylity;

        }

        private Booking GetLocatorGolBws(BookingRS bkResponse, BookingDTO bookingRQ, Availability availability, string offerId)
        {
            var successObject = bkResponse.Booking.GetType().GetProperty("Success").GetValue(bkResponse.Booking, null);

            var offer = availability.Recommendations.FirstOrDefault(x => x.Offers.Any(y => y.Id == offerId))?.Offers.FirstOrDefault(y => y.Id == offerId);
            var jorneys = availability.DataList.Journeys.Where(x => offer.OfferAssociations.JourneyIds.Contains(x.Id)).ToList();
            List<FlightSegment> segments = availability.DataList.FlightSegments.Where(x => jorneys.Any(y => y.FlightSegmentsIds.Contains(x.Id))).ToList();
            var priceClasses = availability.DataList.PriceClasses.Where(x => offer.OfferAssociations.FlightPriceClassAssociations.Any(y => y.PriceClassId.Contains(x.Id))).ToList();
            var fareFamilies = availability.DataList.FareFamilies.Where(x => offer.OfferAssociations.FareFamilyId.Contains(x.Id)).ToList();
            var serviceItems = availability.DataList.ServiceItems.Where(x => offer.OfferItems.Any(y => y.Services.Any(z => z.ServiceItemId.Contains(x.Id)))).ToList();
            var baggageInfos = availability.DataList.BaggageInfos.Where(x => fareFamilies.Any(y => y.DescriptionInfo.BaggageId.Contains(x.Id))).ToList();
            var airports = availability.DataList.Airports.Where(x => segments.Any(y => y.ArrivalCode == x.AirportCode || y.DepartureCode == x.AirportCode)).ToList();
            var airlines = availability.DataList.Airlines.Where(x => x.AirlineCode == offer.Owner).ToList();


            Booking booking = new Booking()
            {
                OrderId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                OrderStatus = "CREATED",
                RecordLocator = successObject.GetType().GetProperty("RecordLocator").GetValue(successObject, null).ToString().ToUpper(),
                Passengers = _mapper.Map<List<Passenger>>(bookingRQ.Passengers),
                OrderInfo = offer,
                DataList = new DataList()
                {
                    Journeys = jorneys,
                    FlightSegments = segments,
                    PriceClasses = priceClasses,
                    FareFamilies = fareFamilies,
                    ServiceItems = serviceItems,
                    BaggageInfos = baggageInfos,
                    Airports = airports,
                    Airlines = airlines,
                },
            };

            return booking;
        }

        #endregion
    }
}