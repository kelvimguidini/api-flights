using flights.data.mongodb.Context;
using flights.domain.Interfaces.Repositories;
using MongoDB.Driver;
using System;
using flights.domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace flights.data.mongodb.Repositories
{
    public class AirlineDetailsRepository : IAirlineDetailsRepository
    {
        private readonly MongoContext _db;
        public AirlineDetailsRepository()
        {
            _db = new MongoContext();
        }

        public async Task<IEnumerable<AirlineDetails>> GetByAirlinesCode(List<string> code)
        {
            try
            {
                return await _db.AirlineDetails.FindAsync(f => code.Contains( f.Airline.AirlineCode)).Result.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os detalhes da disponibilidade: " + ex.Message);
            }            
        }
    }
}
