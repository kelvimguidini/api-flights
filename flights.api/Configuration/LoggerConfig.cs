using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KissLog;
using KissLog.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace flights.api.Configuration
{
    public static class LoggerConfig
    {
        public static void AddLoggingConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ILogger>((context) =>
            {
                return Logger.Factory.Get();
            });

            services.AddLogging(logging =>
            {
                logging.AddKissLog();
            });

        }

    }
}
