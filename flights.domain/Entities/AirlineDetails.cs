using flights.domain.Models.Availability;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace flights.domain.Entities
{
    public class AirlineDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public Airline Airline { get; set; }
    }
}
