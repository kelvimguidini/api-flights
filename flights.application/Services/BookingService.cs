using flights.application.Interfaces;
using flights.domain.Entities;
using flights.domain.Interfaces.Repositories;
using flights.domain.Models;
using flights.domain.Models.Booking;
using KissLog;
using System;
using System.Linq;

namespace flights.application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IAvailabilityDetailsRepository _availabilityDetailsRepository;
        private readonly IBookingProviderService _bookingProviderService;
        private readonly IUserRepository _userRepository;
        private readonly ICredentialRepository _credentialRepository;
        private readonly IAutheticationProviderService _autheticationProviderService;
        private readonly ILogger _logger;

        public BookingService(
            IAvailabilityDetailsRepository availabilityDetailsRepository,
            IBookingProviderService bookingProviderService,
            IUserRepository userRepository,
            ICredentialRepository credentialRepository,
            IAutheticationProviderService autheticationProviderService,
            ILogger logger)
        {
            _availabilityDetailsRepository = availabilityDetailsRepository;
            _bookingProviderService = bookingProviderService;
            _userRepository = userRepository;
            _credentialRepository = credentialRepository;
            _autheticationProviderService = autheticationProviderService;
            _logger = logger;
        }

        public Booking Sell(BookingDTO booking, string username)
        {
            try
            {
                AvailabilityDetails availabilityDetails = _availabilityDetailsRepository.GetByAvailabilityId(booking.OfferRefs.FirstOrDefault().AirShoppingId);

                var rec = availabilityDetails.Availability.Recommendations.FirstOrDefault(x => x.Offers.Any(o => o.Id == booking.OfferRefs.FirstOrDefault().OfferId));
                string provider = rec.Offers.FirstOrDefault(o => o.Id == booking.OfferRefs.FirstOrDefault().OfferId)?.Owner;

                var sessionProvider = GetCredentialProvider(username, provider);

                return _bookingProviderService.GetSellProvider(provider, sessionProvider, booking, availabilityDetails.Availability);
            }
            catch (Exception ex)
            {
                _logger.Log(KissLog.LogLevel.Error, ex);
                throw new Exception("Erro obter reservar: " + ex.Message);
            }
        }


        private SessionProvider GetCredentialProvider(string username, string provider)
        {
            var user = _userRepository.GetByUsername(username);

            var credencialContext = _credentialRepository.GetCredentialByProvider(provider);

            return _autheticationProviderService.AuthenticationProvider(credencialContext.CredentialId);
        }

    }
}
