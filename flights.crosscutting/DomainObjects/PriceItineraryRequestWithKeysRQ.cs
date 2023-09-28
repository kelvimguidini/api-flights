using System;
using System.Collections.Generic;

namespace flights.crosscutting.DomainObjects
{
    public class PriceItineraryRequestWithKeys
	{
		public string CurrencyCode { get; set; }
		public string PaxResidentCountry { get; set; }
		public List<PriceKeys> PriceKeys { get; set; } = new List<PriceKeys>();
		public List<Passenger> Passengers { get; set; } = new List<Passenger>();
		public int PaxCount { get; set; }
        public string ActionStatusCode { get; set; }
    }

	public class PriceKeys
	{
		public string JourneySellKey { get; set; }
		public string FareSellKey { get; set; }
	}

	public class Passenger
	{
		public int PassengerNumber { get; set; }
		public List<PaxPriceType> PaxPriceType { get; set; } = new List<PaxPriceType>();

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

	public class PaxPriceType
	{
		public string PaxType { get; set; }
		public string PaxDiscountCode { get; set; }
	}
}
