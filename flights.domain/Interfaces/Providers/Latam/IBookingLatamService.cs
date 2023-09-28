using flights.domain.Entities;
using flights.domain.Models;
using flights.domain.Models.GetPrice;
using System.Collections.Generic;

namespace flights.domain.Interfaces.Providers.Latam
{
    public interface IBookingLatamService
    {
        public string AirBook(GetPriceRQ priceRQ, string security, List<CredentialParameters> credentialParameters, SessionProvider session);
    }
}