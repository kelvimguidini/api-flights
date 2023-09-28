using System;
using System.Text.Json.Serialization;

namespace flights.domain.Models.Availability
{
    [Serializable]
    public class FlightPriceClassAssociation
    {
        public string FlightSegmentId { get; set; }
        public string PriceClassId { get; set; }
        [JsonIgnore]
        public string Directionality { get; set; }
    }
}
