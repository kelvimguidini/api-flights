using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flights.domain.Models.Provider.Azul
{

    public class SellWithKeyRequest
    {
        public List<SellKeys> SellKeyList { get; set; }
        public List<PaxPriceType> PaxPriceTypes { get; set; }
        public string ActionStatusCode { get; set; }
        public string CurrencyCode { get; set; }
        public int PaxCount { get; set; }
        public string PaxResidentCountry { get; set; }
    }

    public class SellKeys
    {
        public string JourneySellKey { get; set; }
        public string FareSellKey { get; set; }
    }

    public class PaxPriceType
    {
        public string PaxType { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PtcFlights { get; set; }
        public DateTime DateBirth { get; set; }
        public string DocType { get; set; }
        public string DocNumber { get; set; }
        public string Email { get; set; }
        public string PhoneType { get; set; }
        public string PhoneCountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneLocalCode { get; set; }
    }


    //public class SellRequest
    //{
    //    public JourneyRequest[] JourneyRequests { get;  set; }

    //    /// <remarks/>
    //    public PriceRequest PriceRequest { get; set; }
    //}
    //public class JourneyRequest
    //{
    //    public string RuleTariff { get; set; }

    //    public string CarrierCode { get; set; }

    //    public string RuleNumber { get; set; }

    //    public string FareBasis { get; set; }

    //    public string ClassOfService { get; set; }

    //    public InboundOutbound InboundOutbound { get; set; }
    //}

    //public class PriceRequest
    //{
    //    public bool RefareItinerary { get; set; }

    //    public string CurrencyCode { get; set; }

    //    public PointOfSale SourcePointOfSale { get; set; }

    //    public string PaxResidentCountry { get; set; }

    //    public bool ApplyPromotion { get; set; }

    //    public string PromotionCode { get; set; }

    //    public PricingType PricingType { get; set; }

    //    public Passenger[] Passengers { get; set; }

    //    public string[] FareTypes { get; set; }
    //}

    //public class PointOfSale
    //{
    //    public string AgentCode { get; set; }

    //    public string OrganizationCode { get; set; }

    //    public string DomainCode { get; set; }

    //    public string LocationCode { get; set; }
    //}

    //public enum PricingType
    //{
    //    Route,
    //    RouteAndSector,
    //    UseSystemSetting,
    //}


    //public enum InboundOutbound
    //{
    //    None,
    //    Inbound,
    //    Outbound,
    //    Both,
    //    RoundFrom,
    //    RoundTo,
    //}

}
