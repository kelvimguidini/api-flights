using flights.data.mongodb.Context;
using flights.domain.Interfaces.Repositories;
using MongoDB.Driver;
using System;
using flights.domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace flights.data.mongodb.Repositories
{
    public class AirportsRepository : IAirportsRepository
    {
        private readonly MongoContext _db;
        public AirportsRepository()
        {
            _db = new MongoContext();
        }

        public async Task<IEnumerable<Airport>> GetByIata(List<string> iataList)
        {
            try
            {
                return await _db.Airports.FindAsync(f => iataList.Contains(f.iata)).Result.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os detalhes da disponibilidade: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Airport>> GetAutocomplete(string name)
        {
            try
            {
                return await _db.Airports.FindAsync(f => 
                    f.iata.ToUpper().Contains(name.ToUpper())
                    || f.name_en.ToUpper().Contains(name.ToUpper())
                    || f.name_pt.ToUpper().Contains(name.ToUpper())
                    || f.name_es.ToUpper().Contains(name.ToUpper())
                    || f.city.ToUpper().Contains(name.ToUpper())
                ).Result.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os detalhes da disponibilidade: " + ex.Message);
            }
        }
    }
}
