using System.Collections.Generic;

namespace flights.domain.Models.Booking
{
    public class BookingDTO
    {
        public List<OfferRefsDTO> OfferRefs { get; set; }
        public List<PassengerDTO> Passengers { get; set; }

        public PaymentMethodDTO PaymentMethod { get; set; }
        public string ExternalId { get; set; }
    }
}
