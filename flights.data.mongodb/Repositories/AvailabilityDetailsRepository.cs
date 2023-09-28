using flights.data.mongodb.Context;
using flights.domain.Entities;
using flights.domain.Interfaces.Repositories;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace flights.data.mongodb.Repositories
{
    public class AvailabilityDetailsRepository : IAvailabilityDetailsRepository
    {
        private readonly MongoContext _db;
        public AvailabilityDetailsRepository()
        {
            _db = new MongoContext();
        }

        public AvailabilityDetails GetByAvailabilityId(string availabilityId)
        {
            try
            {

                var availabilityDetails = _db.AvailabilityDetails.Find(f => f.Availability.Id == availabilityId).FirstOrDefault();
                if (availabilityDetails == null)
                {
                    throw new Exception("Erro ao obter os detalhes da disponibilidade: Disponibilidade não encontrada");
                }
                return availabilityDetails;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os detalhes da disponibilidade: " + ex.Message);
            }
        }

        public void deleteByDate(DateTime date)
        {
            try
            {
                var availabilityDetails = _db.AvailabilityDetails.DeleteMany(f => f.Included <= date);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os detalhes da disponibilidade: " + ex.Message);
            }
        }

        public async Task Add(AvailabilityDetails availabilityDetails)
        {
            try
            {
                _db.AvailabilityDetails.InsertOneAsync(availabilityDetails);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao salvar os detalhes da disponibilidade: " + ex.Message);
            }
        }
    }
}
