using AutoMapper;
using flights.application.Interfaces;
using flights.crosscutting.Messages.Interfaces;
using flights.domain.Interfaces.Providers.Azul;
using flights.domain.Interfaces.Providers.Gol;
using flights.domain.Interfaces.Providers.Latam;
using flights.domain.Interfaces.Repositories;
using flights.domain.Models;
using flights.domain.Models.Availability;
using flights.domain.Models.Booking;
using flights.domain.Models.GetPrice;
using KissLog;
using System.Linq;

namespace flights.application.Services
{
    public partial class BookingProviderService : IBookingProviderService
    {
        private readonly IBookingLatamService _bookingLatamService;
        private readonly IBookingGolService _bookingGolService;
        private readonly IBookingAzulService _bookingAzulService;
        private readonly IMapper _mapper;
        private readonly INotificator _notificator;
        private readonly ICredentialRepository _credentialRepository;
        private readonly ILogger _logger;
        private readonly IAuthenticationLatamService _authenticationLatamService;
        private readonly IAuthenticationAzulService _authenticationAzulService;
        private readonly IAuthenticationGolService _authenticationGolService;

        public BookingProviderService(
            IBookingLatamService bookingLatamService, 
            IBookingGolService bookingGolService, 
            IBookingAzulService bookingAzulService, 
            IMapper mapper,
            ICredentialRepository credentialRepository, 
            INotificator notificator,
            IAuthenticationLatamService authenticationLatamService,
            IAuthenticationAzulService authenticationAzulService,
            IAuthenticationGolService authenticationGolService,
            ILogger logger)
        {
            _bookingGolService = bookingGolService;
            _mapper = mapper;
            _bookingAzulService = bookingAzulService;
            _bookingLatamService = bookingLatamService;
            _notificator = notificator;
            _credentialRepository = credentialRepository;
            _logger = logger;
            _authenticationLatamService = authenticationLatamService;
            _authenticationAzulService = authenticationAzulService;
            _authenticationGolService = authenticationGolService;
        }

        public Booking GetSellProvider(string provider, SessionProvider session, BookingDTO booking, Availability availability)
        {
            switch (provider)
            {
                case "AD":
                    return BookingAzul(provider, session, booking, availability);
                case "G3-BWS":
                    return BookingGolBws(provider, session, booking, availability);
                case "G3-GWS":
                    return BookingGol(provider, session, booking, availability);
                case "LA":
                    return BookingLatam(provider, session, booking, availability);
                default:
                    return new Booking();
            }
        }
    }
}