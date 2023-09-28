using flights.data.mongodb.AppConfig;
using flights.domain.Entities;
using MongoDB.Driver;

namespace flights.data.mongodb.Context
{
    public class MongoContext 
    {
        private readonly IMongoDatabase _database;
        public MongoContext()
        {
            _database = new MongoClient(new AppConfiguration().ConnectionString).GetDatabase("fligths");
        }

        public IMongoCollection<AvailabilityDetails> AvailabilityDetails 
                                { get { return _database.GetCollection<AvailabilityDetails>("AvailabilityDetails"); } }

        public IMongoCollection<AirlineDetails> AirlineDetails 
                                { get { return _database.GetCollection<AirlineDetails>("AirlinesDetails"); } }

        public IMongoCollection<Airport> Airports
                            { get { return _database.GetCollection<Airport>("Airports"); } }
    }
}
