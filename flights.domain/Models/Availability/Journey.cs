using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace flights.domain.Models.Availability
{
    public class Journey
    {
        public string Id { get; set; }
        public string DepartureCode { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string ArrivalCode { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int Duration { get; set; }
        public string Mileage { get; set; }
        public List<string> FlightSegmentsIds { get; set; }
        [JsonIgnore]
        public string SellKey { get; set; }
    }
}
