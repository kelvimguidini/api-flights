
using System;

namespace flights.domain.Models.Availability
{
    [Serializable]
    public class TotalPrice
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
