using System;
using System.Collections.Generic;
using System.Text;

namespace flights.domain.Models.Availability
{
    [Serializable]
    public class Offer
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public List<OfferItem> OfferItems { get; set; }
        public OfferAssociations OfferAssociations { get; set; }
    }
}
