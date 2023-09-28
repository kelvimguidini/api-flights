
using System.Text.Json.Serialization;

namespace flights.domain.Models.Availability
{
    public class PriceClass
    {
        public string Id { get; set; }
        public string ClassOfService { get; set; }
        public string FareBasis { get; set; }

        [JsonIgnore]
        public string SellKey { get; set; }

    }
}
