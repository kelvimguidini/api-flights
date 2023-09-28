using flights.domain.Interfaces.Repositories;
using Quartz;
using System;
using System.Threading.Tasks;

namespace flights.application.Services
{
    public class JobAvailabilityService : IJob
    {

        private readonly IAvailabilityDetailsRepository _availabilityDetailsRepository;

        public JobAvailabilityService(IAvailabilityDetailsRepository availabilityDetailsRepository)
        {
            _availabilityDetailsRepository = availabilityDetailsRepository;
        }

        async Task IJob.Execute(IJobExecutionContext context)
        {
            #region Apagar Registros antigos

            _availabilityDetailsRepository.deleteByDate(DateTime.Now.AddHours(-10));

            #endregion
        }
    }
}
