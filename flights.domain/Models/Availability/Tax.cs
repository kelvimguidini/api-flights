using System;
using System.Collections.Generic;
using System.Text;

namespace flights.domain.Models.Availability
{
    [Serializable]
    public class Tax
    {
        public string TaxCode { get; set; }
        public TaxAmount TaxAmount { get; set; }
    }

    [Serializable]
    public class TaxAmount
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
