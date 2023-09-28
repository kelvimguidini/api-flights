using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace flights.domain.Models.Availability
{
    [Serializable]
    public class Recommendation
    {
        public List<Offer> Offers { get; set; }
        public string Provider { get; set; }

        [JsonIgnore]
        public string SequenceNumber { get; set; }
    }
}
