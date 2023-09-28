using System;
using System.Collections.Generic;

namespace flights.domain.Models.GetPrice
{
    public class GetPriceRQ
    {
        public List<JourneySell> JorneysSell { get; set; }
        public string CurrencyCode { get; set; }
        public List<PassangerSell> Passagers { get; set; }
    }

    public class PassangerSell
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
        public EmergencyContact EmergencyContact { get; set; }

    }

    public class EmergencyContact
    {
        public string Email { get; set; }
        public string PhoneType { get; set; }
        public string PhoneCountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneLocalCode { get; set; }
    }

    public class JourneySell
    {
        public string JourneySellKey { get; set; }
        public string FareSellKey { get; set; }

        //Para Latam

        public string BrandID { get; set; }
        public string DepartureDateTime { get; set; }
        public string ArrivalDateTime { get; set; }
        public string FlightNumber { get; set; }
        public string NumberInParty { get; set; }
        public string ResBookDesigCode { get; set; }
        public string Status { get; set; }
        public string LocationCodeDestiation { get; set; }
        public string LocationCodeOrigin { get; set; }
        public string MarketingCode { get; set; }
        public string MarriageGrp { get; set; }
    }
}