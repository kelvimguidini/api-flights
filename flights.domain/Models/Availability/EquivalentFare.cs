
using System;

namespace flights.domain.Models.Availability
{
    [Serializable]
    public class EquivalentFare
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
