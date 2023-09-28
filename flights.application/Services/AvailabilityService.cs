using AutoMapper;
using flights.application.DTO;
using flights.application.Interfaces;
using flights.domain.Entities;
using flights.domain.Interfaces.Repositories;
using flights.domain.Models;
using flights.domain.Models.Availability;
using KissLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace flights.application.Services
{
    public class AvailabilityService : IAvailabilityService
    {

        private readonly IAutheticationProviderService _autheticationProviderService;
        private readonly IAvailabilityProviderService _availabilityProviderService;
        private readonly IProviderRepository _providerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICredentialRepository _credentialRepository;
        private readonly IAvailabilityDetailsRepository _availabilityDetailsRepository;
        private readonly IAirlineDetailsRepository _airlineDetailsRepository;
        private readonly IAirportsRepository _airportsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AvailabilityService(
            IAutheticationProviderService autheticationProviderService,
            IAvailabilityProviderService availabilityProviderService,
            IProviderRepository providerRepository,
            IUserRepository userRepository,
            ICredentialRepository credentialRepository,
            IAvailabilityDetailsRepository availabilityDetailsRepository,
            IAirlineDetailsRepository airlineDetailsRepository,
            IAirportsRepository airportsRepository, IMapper mapper,
            ILogger logger)
        {
            _autheticationProviderService = autheticationProviderService;
            _availabilityProviderService = availabilityProviderService;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
            _credentialRepository = credentialRepository;
            _availabilityDetailsRepository = availabilityDetailsRepository;
            _airlineDetailsRepository = airlineDetailsRepository;
            _airportsRepository = airportsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Availability> GetAvailability(AvailabilityRQDTO availabilityRQDTO, string username)
        {
            var providers = _providerRepository.GetProvider();

            var availabilityRQ = new
            {
                availabilityRQDTO
            };

            var availabilityRS = new List<Availability>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();


            //MANTER COMENTADO PARA FACILITAR O DEBUG, DEPOIS DE TERMINAR O AVAILABILITY DA AZUL TROCAR PARA TESTAR PERFORMANCE
            List<Task> tasks = new List<Task>();
            foreach (var provider in providers)
            {
                try
                {
                    if (provider.Active)
                    {
                        var sessionProvider = GetCredentialProvider(username, provider.ProviderId, provider.Initials == "LA");
                        Task task = Task.Run(() =>
                        {
                            availabilityRS.Add(_availabilityProviderService.GetAvailabilityProvider(provider.Initials, sessionProvider, availabilityRQ));
                        });
                        tasks.Add(task);

                    }
                }
                catch (Exception e)
                {
                    _logger.Log(KissLog.LogLevel.Error, e);
                }
            }
            Task.WaitAll(tasks.ToArray());


            #region Serviços
            //BAGS
            var serviceItems = new List<ServiceItem>();
            var baggageInfos = new List<BaggageInfo>();

            serviceItems.Add(new ServiceItem
            {
                Id = "SVC-MEAL",
                Code = "MEAL",
                Owner = null,
                Description = "Lanche à bordo",
                ServiceItemAmount = 0.0M
            });

            baggageInfos.Add(new BaggageInfo()
            {
                Id = "BAG-0-0KG",
                CheckedBaggage = new CheckedBaggage()
                {
                    Weight = 23,
                    Unit = "KG",
                    Pieces = 0
                }
            });
            baggageInfos.Add(new BaggageInfo()
            {
                Id = "BAG-1-23KG",
                CheckedBaggage = new CheckedBaggage()
                {
                    Weight = 23,
                    Unit = "KG",
                    Pieces = 1
                }
            });

            baggageInfos.Add(new BaggageInfo()
            {
                Id = "BAG-2-23KG",
                CheckedBaggage = new CheckedBaggage()
                {
                    Weight = 23,
                    Unit = "KG",
                    Pieces = 2
                }
            });

            baggageInfos.Add(new BaggageInfo()
            {
                Id = "BAG-3-23KG",
                CheckedBaggage = new CheckedBaggage()
                {
                    Weight = 23,
                    Unit = "KG",
                    Pieces = 3
                }
            });

            #endregion

            #region Agrupa response de todas as companhias 
            var availability = new Availability
            {
                Id = Guid.NewGuid().ToString(),
                Recommendations = new List<Recommendation>(),
                DataList = new DataList()
                {
                    Journeys = new List<Journey>(),
                    FlightSegments = new List<FlightSegment>(),
                    PriceClasses = new List<PriceClass>(),
                    FareFamilies = new List<FareFamily>(),
                    ServiceItems = serviceItems,
                    BaggageInfos = baggageInfos,
                    Airports = new List<domain.Models.Availability.Airport>(),
                    Airlines = _airlineDetailsRepository.GetByAirlinesCode(providers.Select(x => x.Initials).ToList()).Result.Select(x => x.Airline).ToList(),
                }
            };

            foreach (var rs in availabilityRS)
            {
                if (rs.Recommendations != null)
                {
                    availability.Recommendations.AddRange(rs.Recommendations);

                    availability.DataList.Journeys.AddRange(rs.DataList.Journeys);
                    availability.DataList.FlightSegments.AddRange(rs.DataList.FlightSegments);
                    availability.DataList.PriceClasses.AddRange(rs.DataList.PriceClasses);
                    availability.DataList.FareFamilies.AddRange(rs.DataList.FareFamilies);
                    availability.DataList.Airports.AddRange(rs.DataList.Airports);
                }
            }

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            availability.Time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            // Complemento aeroportos
            var airportList = availability.DataList.Airports.Select(x => x.AirportCode).ToList();
            var list = _airportsRepository.GetByIata(airportList).Result.ToList();

            availability.DataList.Airports = _mapper.Map<List<domain.Entities.Airport>, List<domain.Models.Availability.Airport>>(list);

            //ORDENAR RECOMENDAÇÕES PELA OFERTA MAIS BARATA
            availability.Recommendations = availability.Recommendations.OrderBy(x => x.Offers.FirstOrDefault()?.OfferItems.Select(z => z.TotalPrice.Amount).FirstOrDefault()).ToList();

            #endregion

            _ = _availabilityDetailsRepository.Add(new AvailabilityDetails
            {
                Availability = availability,
                Included = DateTime.Now
            });

            return availability;

        }

        public async Task<IEnumerable<domain.Entities.Airport>> AirportAutocomplete(string name)
        {
            var list = _airportsRepository.GetAutocomplete(name);
            return await list;
        }

        private SessionProvider GetCredentialProvider(string username, int providerId, bool token = false)
        {
            var user = _userRepository.GetByUsername(username);

            var credencialContext = _credentialRepository.GetCredentialContext(user.ApplicationId, providerId);

            return _autheticationProviderService.AuthenticationProvider(credencialContext.Credential.CredentialId, token);
        }
    }
}