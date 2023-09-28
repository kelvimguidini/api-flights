using flights.domain.Models.Availability;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace flights.domain.Entities
{
    public class AvailabilityDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public Availability Availability { get; set; }
        public DateTime Included { get; set; }
    }
}
