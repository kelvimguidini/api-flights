using System;
using System.Collections.Generic;


namespace flights.domain.Models.Availability
{
    [Serializable]
    public class Service
    {
        public string ServiceItemId { get; set; }
        public List<string> FlightSegmentsIds { get; set; }
        public string BaggageId { get; set; }
        public bool IncludedWithOfferItem { get; set; }
    }
}
