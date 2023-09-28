using flights.application.Interfaces;
using flights.application.Services;
using flights.crosscutting.Messages.Interfaces;
using flights.crosscutting.Messages.Models;
using flights.crosscutting.Notifications.Email;
using flights.crosscutting.Notifications.Email.Interfaces;
using flights.data.mongodb.Repositories;
using flights.data.sqlserver.Context;
using flights.data.sqlserver.Repositories;
using flights.domain.Interfaces.Providers.Azul;
using flights.domain.Interfaces.Providers.Gol;
using flights.domain.Interfaces.Providers.Latam;
using flights.domain.Interfaces.Repositories;
using flights.provider.azul.Services;
using flights.provider.Azul.Services;
using flights.provider.gol.Services;
using flights.provider.latam.Services;
using flights.provider.Latam.Services;
using Microsoft.Extensions.DependencyInjection;

namespace flights.api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAvailabilityService, AvailabilityService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingProviderService, BookingProviderService>();


            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICredentialRepository, CredentialRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IAvailabilityDetailsRepository, AvailabilityDetailsRepository>();
            services.AddScoped<IAirlineDetailsRepository, AirlineDetailsRepository>();
            services.AddScoped<IAirportsRepository, AirportsRepository>();


            services.AddScoped<IAutheticationProviderService, AutheticationProviderService>();
            services.AddScoped<IAvailabilityProviderService, AvailabilityProviderService>();


            services.AddScoped<IAuthenticationAzulService, AuthenticationAzulService>();
            services.AddScoped<IAvailabilityAzulService, AvailabilityAzulService>();
            services.AddScoped<IBookingAzulService, BookingAzulService>();


            services.AddScoped<IAuthenticationLatamService, AuthenticationLatamService>();
            services.AddScoped<IAvailabilityLatamService, AvailabilityLatamService>();
            services.AddScoped<IBookingLatamService, BookingLatamService>();


            services.AddScoped<IAuthenticationGolService, AuthenticationGolService>();
            services.AddScoped<IAvailabilityGolService, AvailabilityGolService>();
            services.AddScoped<IBookingGolService, BookingGolService>();


            services.AddScoped<INotificator, Notificator>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<ContextDb>();
        }
    }
}
