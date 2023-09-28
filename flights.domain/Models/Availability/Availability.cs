using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace flights.domain.Models.Availability
{
    public class Availability
    {
        public string Id { get; set; }

        [JsonIgnore]
        public string Time { get; set; }
        public List<Recommendation> Recommendations { get; set; }
        public DataList DataList { get; set; }


        public static implicit operator Task<object>(Availability v)
        {
            throw new NotImplementedException();
        }
    }
}
