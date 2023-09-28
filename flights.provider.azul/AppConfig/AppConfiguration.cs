using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flights.provider.azul.AppConfig
{
    public class AppConfiguration
    {
        public readonly string _urlSVCBaseAzul = string.Empty;
        public readonly string _soapURLGetAvailabilityByTripAzul = string.Empty;
        public readonly string _soapURLPriceItineraryByKeys = string.Empty;
        public readonly string _soapURLSellByKey = string.Empty;
        public readonly string _soapURLCommitSellByKey = string.Empty;

        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();

            _urlSVCBaseAzul = root.GetSection("SOAPUrls").GetSection("UrlSVCBaseAzul").Value;
            _soapURLGetAvailabilityByTripAzul = root.GetSection("SOAPUrls").GetSection("SoapURLGetAvailabilityByTripAzul").Value;
            _soapURLPriceItineraryByKeys = root.GetSection("SOAPUrls").GetSection("SoapURLPriceItineraryByKeys").Value;
            _soapURLSellByKey = root.GetSection("SOAPUrls").GetSection("SoapURLSellByKey").Value;
            _soapURLCommitSellByKey = root.GetSection("SOAPUrls").GetSection("SoapURLCommitSellByKey").Value;
        }
        public string UrlSVCBaseAzul
        {
            get => _urlSVCBaseAzul;
        }

        public string SoapURLGetAvailabilityByTripAzul
        {
            get => _soapURLGetAvailabilityByTripAzul;
        }

        public string SoapURLPriceItineraryByKeys
        {
            get => _soapURLPriceItineraryByKeys;
        }

        public string SoapURLCommitSellByKey
        {
            get => _soapURLCommitSellByKey;
        }

        public string SoapURLSellByKey
        {
            get => _soapURLSellByKey;
        }

    }
}
