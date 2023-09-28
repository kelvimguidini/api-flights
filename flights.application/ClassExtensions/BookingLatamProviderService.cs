using flights.domain.Models;
using flights.domain.Models.Availability;
using flights.domain.Models.Booking;
using flights.domain.Models.GetPrice;
using System;
using System.Collections.Generic;
using System.Linq;


namespace flights.application.Services
{
    public partial class BookingProviderService
    {
        private GetPriceRQ GeneratePriceRQLatam(BookingDTO booking, Availability availability)
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
                        //ResBookDesigCode = segment.ResBookDesigCode,
                        ResBookDesigCode = availability.DataList.PriceClasses.FirstOrDefault(x => x.Id == offer.OfferAssociations.FlightPriceClassAssociations.FirstOrDefault(f => f.FlightSegmentId == segment.Id)?.PriceClassId)?.ClassOfService,
                        Status = "NN",
                        LocationCodeDestiation = segment.ArrivalCode,
                        LocationCodeOrigin = segment.DepartureCode,
                        MarketingCode = offer.Owner,
                        MarriageGrp = segment.MarriageGrp
                    });
                }
            }

            return new GetPriceRQ
            {
                Passagers = paxT,
                JorneysSell = jorneysSell,
                CurrencyCode = availability.Recommendations.FirstOrDefault()?.Offers.FirstOrDefault()?.OfferItems.FirstOrDefault()?.BaseFare.CurrencyCode,
            };
        }

        private Booking GetLocatorLatam(string locator, BookingDTO bookingRQ, Availability availability, string offerId)
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

        private Booking BookingLatam(string provider, SessionProvider session, BookingDTO booking, Availability availability)
        {
            var obj = session.Session.GetType().GetProperty("Security");

            string binarySecurityToken = obj.GetValue(session.Session, null).GetType().GetProperty("BinarySecurityToken").GetValue(obj.GetValue(session.Session, null), null).ToString();

            try
            {
                // LOGICA PARA MONTAR O REQUEST DO PriceItinerary
                GetPriceRQ priceRQ = GeneratePriceRQLatam(booking, availability);

                var bookingRS = GetLocatorLatam(_bookingLatamService.AirBook(priceRQ, binarySecurityToken, _credentialRepository.GetCredentialParameters(session.CredentialId), session), booking, availability, booking.OfferRefs.FirstOrDefault().OfferId);

                return bookingRS;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                var credentialParameters = _credentialRepository.GetCredentialParameters(session.CredentialId);
                _authenticationLatamService.Logoff(credentialParameters, binarySecurityToken);
            }
        }
    }
}