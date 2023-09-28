
using System;
using System.Collections.Generic;

namespace flights.domain.Models.Availability
{
    [Serializable]
    public class OfferItem
    {
        public string Ptc { get; set; }
        public TotalPrice TotalPrice { get; set; }
        public BaseFare BaseFare { get; set; }
        public EquivalentFare EquivalentFare { get; set; }
        public List<Tax> Taxes { get; set; }
        public List<Service> Services { get; set; }

    }
}
