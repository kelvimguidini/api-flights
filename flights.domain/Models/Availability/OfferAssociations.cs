using System;
using System.Collections.Generic;

namespace flights.domain.Models.Availability
{
    [Serializable]
    public class OfferAssociations
    {
        public string FareFamilyId { get; set; }
        public List<string> JourneyIds { get; set; }
        public List<FlightPriceClassAssociation> FlightPriceClassAssociations { get; set; }
        public string AccountCode { get; set; }
        public int CredentialId { get; set; }
    }
}
