using AutoMapper;
using flights.application.Interfaces;
using flights.domain.Interfaces.Providers.Azul;
using flights.domain.Interfaces.Providers.Gol;
using flights.domain.Interfaces.Providers.Latam;
using flights.domain.Interfaces.Repositories;
using flights.domain.Models;
using flights.domain.Models.Availability;
using KissLog;

namespace flights.application.Services
{
    public partial class AvailabilityProviderService : IAvailabilityProviderService
    {
        private readonly IAvailabilityAzulService _availabilityAzulService;
        private readonly IAvailabilityGolService _availabilityGolService;
        private readonly IAvailabilityLatamService _availabilityLatamService;
        private readonly IBookingLatamService _bookingLatamService;
        private readonly IBookingGolService _bookingGolService;
        private readonly IBookingAzulService _bookingAzulService;
        private readonly IAuthenticationGolService _authenticationGolService;
        private readonly IAuthenticationAzulService _authenticationAzulService;
        private readonly ICredentialRepository _credentialRepository;

        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AvailabilityProviderService(
            IAvailabilityLatamService availabilityLatamService,
            IAvailabilityGolService availabilityGolService,
            IAvailabilityAzulService availabilityAzulService,
            IBookingLatamService bookingLatamService,
            IAuthenticationGolService authenticationGolService,
            IAuthenticationAzulService authenticationAzulService,
            IBookingGolService bookingGolService,
            IBookingAzulService bookingAzulService,
            ICredentialRepository credentialRepository,
            IMapper mapper,
            ILogger logger)
        {
            _availabilityGolService = availabilityGolService;
            _availabilityAzulService = availabilityAzulService;
            _availabilityLatamService = availabilityLatamService;
            _mapper = mapper;
            _logger = logger;
            _bookingLatamService = bookingLatamService;
            _bookingGolService = bookingGolService;
            _bookingAzulService = bookingAzulService;
            _credentialRepository = credentialRepository;
            _authenticationGolService = authenticationGolService;
            _authenticationAzulService = authenticationAzulService;
        }
         
        public Availability GetAvailabilityProvider(string provider, SessionProvider session, object availability)
        {
            switch (provider)
            {
                case "AD":
                    return ToAvailabilityAzul(_availabilityAzulService.GetAvailability(session, availability), session);
                case "G3-BWS":
                    return ToAvailabilityGolBws(_availabilityGolService.GetAvailabilityBws(session, availability), session);
                case "G3-GWS":

                    var disp = ToAvailabilityGol(_availabilityGolService.GetAvailability(session, availability), session);
                    
                    var objSession = session.Session.GetType().GetProperty("Session").GetValue(session.Session, null);
                    var obj = objSession.GetType().GetProperty("Security").GetValue(objSession, null);
                    string binarySecurityToken = obj.GetType().GetProperty("BinarySecurityToken").GetValue(obj, null).ToString();
                    
                    var credentialParameters = _credentialRepository.GetCredentialParameters(session.CredentialId);


                    _authenticationGolService.Logoff(credentialParameters, binarySecurityToken);
                    return disp;
                case "LA":
                    return ToAvailabilityLatam(_availabilityLatamService.GetAvailability(session, availability), session);
                default:
                    return new Availability();
            }
        }
    }
}