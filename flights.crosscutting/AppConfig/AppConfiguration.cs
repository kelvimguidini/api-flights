using Microsoft.Extensions.Configuration;
using System.IO;

namespace flights.crosscutting.AppConfig
{
    public class AppConfiguration
    {
        private readonly string _connectionString = string.Empty;
        public AppConfiguration(string grupo, string chave)
        {
            var configurationBuilder = new ConfigurationBuilder();
#if DEBUG
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Development.json");
#else
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
#endif

            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            _connectionString = root.GetSection(grupo).GetSection(chave).Value;
        }
        public string Configuration
        {
            get => _connectionString;
        }

    }
}
