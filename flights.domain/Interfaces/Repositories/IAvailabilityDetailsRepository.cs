using flights.domain.Entities;
using System;
using System.Threading.Tasks;

namespace flights.domain.Interfaces.Repositories
{
    public interface IAvailabilityDetailsRepository
    {
        AvailabilityDetails GetByAvailabilityId(string availabilityId);
        Task Add(AvailabilityDetails availabilityDetails);
        void deleteByDate(DateTime date);
    }
}
