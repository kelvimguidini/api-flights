using System.Collections.Generic;

namespace flights.domain.Models.Availability
{
    public class Airline
    {
        public string AirlineDesignator { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public List<Link> Links { get; set; }
    }
}
