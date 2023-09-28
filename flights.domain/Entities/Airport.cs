using flights.domain.Models.Availability;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace flights.domain.Entities
{
    public class Airport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string iata { get; set; }

        public string name_pt { get; set; }

        public string name_en { get; set; }

        public string name_es { get; set; }

        public string city { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }

        public bool activated { get; set; }

        public double weight { get; set; }

        public IEnumerable<string> tags { get; set; }

        public string _class { get; set; }
    }
}