using flights.crosscutting.DomainObjects;
using flights.crosscutting.Utils;
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
        private PriceItineraryRequestWithKeys GeneratePriceRQAzul(BookingDTO booking, Availability availability)
        {

            List<PriceKeys> priceKeys = new List<PriceKeys>();
            List<crosscutting.DomainObjects.Passenger> passengers = new List<crosscutting.DomainObjects.Passenger>();

            var rec = availability.Recommendations.FirstOrDefault(x => x.Offers.Any(o => o.Id == booking.OfferRefs.FirstOrDefault().OfferId));
            var offer = rec.Offers.FirstOrDefault(o => o.Id == booking.OfferRefs.FirstOrDefault().OfferId);

            foreach (var jorneyId in offer.OfferAssociations.JourneyIds)
            {
                var journey = availability.DataList.Journeys.FirstOrDefault(x => x.Id == jorneyId);

                var segment = availability.DataList.FlightSegments.FirstOrDefault(x => x.Id == journey.FlightSegmentsIds.FirstOrDefault());
                priceKeys.Add(new PriceKeys
                {
                    FareSellKey = availability.DataList.PriceClasses.FirstOrDefault(x => x.Id == offer.OfferAssociations.FlightPriceClassAssociations.FirstOrDefault(y => y.FlightSegmentId == segment.Id)?.PriceClassId)?.SellKey,
                    JourneySellKey = journey.SellKey,
                });
            }

            foreach (var passenger in booking.Passengers)
            {
                var _passenger = new crosscutting.DomainObjects.Passenger();

                var PaxPriceType = new PaxPriceType();
                PaxPriceType.PaxType = passenger.Ptc.ToUpper();

                _passenger.FirstName = passenger.GivenName;
                _passenger.LastName = passenger.Surname;
                _passenger.PtcFlights = passenger.Ptc.ToUpper();
                _passenger.DateBirth = passenger.DateOfBirth;
                _passenger.DocType = passenger.Documents.FirstOrDefault()?.DocumentType;
                _passenger.DocNumber = passenger.Documents.FirstOrDefault()?.DocumentNumber;
                _passenger.Email = passenger.Contacts.FirstOrDefault()?.EmailContacts.FirstOrDefault().Email;
                _passenger.PhoneType = passenger.Contacts.FirstOrDefault()?.PhoneContacts.FirstOrDefault()?.PhoneType;
                _passenger.PhoneCountryCode = passenger.Contacts.FirstOrDefault()?.PhoneContacts.FirstOrDefault()?.CountryCode;
                _passenger.PhoneNumber = passenger.Contacts.FirstOrDefault()?.PhoneContacts.FirstOrDefault()?.PhoneNumber;
                _passenger.PhoneLocalCode = passenger.Contacts.FirstOrDefault()?.PhoneContacts.FirstOrDefault()?.LocalCode;
               
                _passenger.PaxPriceType.Add(PaxPriceType);

                passengers.Add(_passenger);
            }

            var priceItineraryRequest = new PriceItineraryRequestWithKeys();

            priceItineraryRequest.PriceKeys = priceKeys;
            priceItineraryRequest.CurrencyCode = "BR";
            priceItineraryRequest.PaxResidentCountry = "BR";
            priceItineraryRequest.Passengers = passengers;

            priceItineraryRequest.PaxCount = passengers.Count();

            return priceItineraryRequest;
        }

        private Booking GetLocatorAzul(BookingRS bkResponse, BookingDTO bookingRQ, Availability availability, string offerId)
        {
            //var successObject = bkResponse.Booking.GetType().GetProperty("Success").GetValue(bkResponse.Booking, null);
            var successObject = WCFXMLManager.ElementValue(bkResponse.Booking.ToString(), "CommitResult", "RecordLocator");

            var offer = availability.Recommendations.FirstOrDefault(x => x.Offers.Any(y => y.Id == offerId))?.Offers.FirstOrDefault(y => y.Id == offerId);
            var jorneys = availability.DataList.Journeys.Where(x => offer.OfferAssociations.JourneyIds.Contains(x.Id)).ToList();
            List<FlightSegment> segments = availability.DataList.FlightSegments.Where(x => jorneys.Any(y => y.FlightSegmentsIds.Contains(x.Id))).ToList();
            var priceClasses = availability.DataList.PriceClasses.Where(x => offer.OfferAssociations.FlightPriceClassAssociations.Any(y => y.PriceClassId != null && y.PriceClassId.Contains(x.Id))).ToList();
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
                RecordLocator = successObject,
                Passengers = _mapper.Map<List<domain.Models.Passenger>>(bookingRQ.Passengers), //ver essa parte
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

        private bool ValidatePriceAzul(string priceXmlStringResponse, Offer offer, int qtdPax)
        {
            
            var priceAvailabylity = offer.OfferItems.Sum(x => x.TotalPrice.Amount) * qtdPax;

            decimal totalPrice = Convert.ToDecimal(WCFXMLManager.ElementValue(priceXmlStringResponse, "PriceItineraryByKeysResult","Total"));

            bool isValid = totalPrice == priceAvailabylity || (totalPrice / 10000) == priceAvailabylity;
            if (!isValid) {
                //_logger.Log(KissLog.LogLevel.Warning, "x 1000 : " + (totalPrice / 10000), "Azul");
                _logger.Log(KissLog.LogLevel.Warning, "Tarifação: " + totalPrice.ToString() + " - Disponibilidade: " + priceAvailabylity.ToString(), "Azul");
            }
            return isValid;

        }

        private Booking BookingAzul(string provider, SessionProvider session, BookingDTO booking, Availability availability)
        {
            var priceRQAzul = GeneratePriceRQAzul(booking, availability);
            var priceItineraryResponse = _bookingAzulService.PriceItineraryByKeys(session, priceRQAzul);
            //VALIDAR SE O PREÇO É O MESMO DA DISPONIBILIDADE
            if (!booking.Passengers.Any(x => x.Ptc == "CHD") && !ValidatePriceAzul(priceItineraryResponse, availability.Recommendations.FirstOrDefault(x => x.Offers.Any(y => y.Id == booking.OfferRefs.FirstOrDefault().OfferId))?.Offers.FirstOrDefault(y => y.Id == booking.OfferRefs.FirstOrDefault().OfferId), booking.Passengers.Count))
            {
                _notificator.notify("Erro obter tarifação Azul: Consulta expirou");
            }

            var sellWithKeyRequest = _bookingAzulService.GenerateSellRequest(priceRQAzul);
            sellWithKeyRequest.ActionStatusCode = "NN";
            sellWithKeyRequest.CurrencyCode = "BRL";
            var result = GetLocatorAzul(_bookingAzulService.SellByKey(session, sellWithKeyRequest), booking, availability, booking.OfferRefs.FirstOrDefault().OfferId);
            _authenticationAzulService.LogOff(session);
            return result; 

        }
    }
}