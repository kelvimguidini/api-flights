using System;
using System.Text.Json.Serialization;

namespace flights.domain.Models.Availability
{
    public class FlightSegment
    {
        public string Id { get; set; }
        public string MarketingCarrierCode { get; set; }
        public string OperationCarrierCode { get; set; }
        public int FlightNumber { get; set; }
        public string DepartureCode { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string ArrivalCode { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string Aircraft { get; set; }
        public int Duration { get; set; }
        public string Mileage { get; set; }
        public string Stops { get; set; }

        [JsonIgnore]
        public string ResBookDesigCode { get; set; }
        [JsonIgnore]
        public string MarriageGrp { get; set; }
    }
}
