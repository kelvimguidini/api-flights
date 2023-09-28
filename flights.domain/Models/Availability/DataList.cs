using System.Collections.Generic;

namespace flights.domain.Models.Availability
{
    public class DataList
    {
        public List<Journey> Journeys { get; set; }
        public List<FlightSegment> FlightSegments { get; set; }
        public List<PriceClass> PriceClasses { get; set; }
        public List<FareFamily> FareFamilies { get; set; }
        public List<ServiceItem> ServiceItems { get; set; }
        public List<BaggageInfo> BaggageInfos { get; set; }
        public List<Airport> Airports { get; set; }
        public List<Airline> Airlines { get; set; }

    }
}
