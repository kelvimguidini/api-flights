using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace flights.provider.azul.Models
{
	/*
	[XmlRoot(ElementName = "CurrencyCode")]
	public class CurrencyCode
	{

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Total")]
	public class Total
	{

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlText]
		public double Text { get; set; }
	}

	[XmlRoot(ElementName = "PriceStatus")]
	public class PriceStatus
	{

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "State")]
	public class State
	{

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "FlightDesignator")]
	public class FlightDesignatorItinerary
	{

		[XmlElement(ElementName = "CarrierCode")]
		public string CarrierCode { get; set; }

		[XmlElement(ElementName = "FlightNumber")]
		public int FlightNumber { get; set; }

		[XmlElement(ElementName = "OpSuffix")]
		public int OpSuffix { get; set; }

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Leg")]
	public class Leg
	{

		[XmlElement(ElementName = "DepartureStation")]
		public string DepartureStation { get; set; }

		[XmlElement(ElementName = "STD")]
		public DateTime STD { get; set; }

		[XmlElement(ElementName = "ArrivalStation")]
		public string ArrivalStation { get; set; }

		[XmlElement(ElementName = "STA")]
		public DateTime STA { get; set; }

		[XmlElement(ElementName = "FlightDesignator")]
		public FlightDesignatorItinerary FlightDesignator { get; set; }

		[XmlElement(ElementName = "Status")]
		public string Status { get; set; }

		[XmlElement(ElementName = "InventoryLegKey")]
		public string InventoryLegKey { get; set; }

		[XmlElement(ElementName = "AircraftType")]
		public object AircraftType { get; set; }

		[XmlElement(ElementName = "AircraftTypeSuffix")]
		public object AircraftTypeSuffix { get; set; }

		[XmlElement(ElementName = "PRBCCode")]
		public object PRBCCode { get; set; }

		[XmlElement(ElementName = "PaxSTD")]
		public DateTime PaxSTD { get; set; }

		[XmlElement(ElementName = "DeptLTV")]
		public int DeptLTV { get; set; }

		[XmlElement(ElementName = "DepartureTerminal")]
		public object DepartureTerminal { get; set; }

		[XmlElement(ElementName = "PaxSTA")]
		public DateTime PaxSTA { get; set; }

		[XmlElement(ElementName = "ArrvLTV")]
		public int ArrvLTV { get; set; }

		[XmlElement(ElementName = "ArrivalTerminal")]
		public object ArrivalTerminal { get; set; }

		[XmlElement(ElementName = "ScheduleServiceType")]
		public int ScheduleServiceType { get; set; }

		[XmlElement(ElementName = "OnTime")]
		public int OnTime { get; set; }

		[XmlElement(ElementName = "ETicket")]
		public bool ETicket { get; set; }

		[XmlElement(ElementName = "IROP")]
		public bool IROP { get; set; }

		[XmlElement(ElementName = "FLIFOUpdated")]
		public bool FLIFOUpdated { get; set; }

		[XmlElement(ElementName = "SubjectToGovtApproval")]
		public bool SubjectToGovtApproval { get; set; }

		[XmlElement(ElementName = "Capacity")]
		public int Capacity { get; set; }

		[XmlElement(ElementName = "Lid")]
		public int Lid { get; set; }

		[XmlElement(ElementName = "AdjustedCapacity")]
		public int AdjustedCapacity { get; set; }

		[XmlElement(ElementName = "Sold")]
		public int Sold { get; set; }

		[XmlElement(ElementName = "SoldNonStop")]
		public int SoldNonStop { get; set; }

		[XmlElement(ElementName = "SoldConnect")]
		public int SoldConnect { get; set; }

		[XmlElement(ElementName = "OutMoveDays")]
		public int OutMoveDays { get; set; }

		[XmlElement(ElementName = "BackMoveDays")]
		public int BackMoveDays { get; set; }

		[XmlElement(ElementName = "SoldThru")]
		public int SoldThru { get; set; }

		[XmlElement(ElementName = "CodeShareIndicator")]
		public int CodeShareIndicator { get; set; }

		[XmlElement(ElementName = "OperatingCarrier")]
		public object OperatingCarrier { get; set; }

		[XmlElement(ElementName = "OperatingFlightNumber")]
		public object OperatingFlightNumber { get; set; }

		[XmlElement(ElementName = "OperatingOPSuffix")]
		public int OperatingOPSuffix { get; set; }

		[XmlElement(ElementName = "OperatedByText")]
		public object OperatedByText { get; set; }

		[XmlElement(ElementName = "TransitLayover")]
		public int TransitLayover { get; set; }

		[XmlElement(ElementName = "TransitDays")]
		public int TransitDays { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Legs")]
	public class Legs
	{

		[XmlElement(ElementName = "Leg")]
		public Leg Leg { get; set; }
	}

	[XmlRoot(ElementName = "ServiceCharge")]
	public class ServiceCharge
	{

		[XmlElement(ElementName = "ChargeType")]
		public string ChargeType { get; set; }

		[XmlElement(ElementName = "CollectType")]
		public string CollectType { get; set; }

		[XmlElement(ElementName = "ChargeCode")]
		public object ChargeCode { get; set; }

		[XmlElement(ElementName = "TicketCode")]
		public object TicketCode { get; set; }

		[XmlElement(ElementName = "CurrencyCode")]
		public string CurrencyCode { get; set; }

		[XmlElement(ElementName = "Amount")]
		public double Amount { get; set; }

		[XmlElement(ElementName = "ChargeDetail")]
		public object ChargeDetail { get; set; }

		[XmlElement(ElementName = "ForeignCurrencyCode")]
		public string ForeignCurrencyCode { get; set; }

		[XmlElement(ElementName = "ForeignAmount")]
		public double ForeignAmount { get; set; }
	}

	[XmlRoot(ElementName = "InternalServiceCharges")]
	public class InternalServiceCharges
	{

		[XmlElement(ElementName = "ServiceCharge")]
		public List<ServiceCharge> ServiceCharge { get; set; }
	}

	[XmlRoot(ElementName = "PaxFare")]
	public class PaxFare
	{

		[XmlElement(ElementName = "PaxType")]
		public string PaxType { get; set; }

		[XmlElement(ElementName = "DiscountCode")]
		public object DiscountCode { get; set; }

		[XmlElement(ElementName = "RuleTariff")]
		public object RuleTariff { get; set; }

		[XmlElement(ElementName = "CarrierCode")]
		public string CarrierCode { get; set; }

		[XmlElement(ElementName = "RuleNumber")]
		public string RuleNumber { get; set; }

		[XmlElement(ElementName = "FareBasis")]
		public string FareBasis { get; set; }

		[XmlElement(ElementName = "FareDiscountCode")]
		public object FareDiscountCode { get; set; }

		[XmlElement(ElementName = "ActionStatusCode")]
		public string ActionStatusCode { get; set; }

		[XmlElement(ElementName = "ProductClass")]
		public string ProductClass { get; set; }

		[XmlElement(ElementName = "FareStatus")]
		public string FareStatus { get; set; }

		[XmlElement(ElementName = "InternalServiceCharges")]
		public InternalServiceCharges InternalServiceCharges { get; set; }

		[XmlElement(ElementName = "LastModified")]
		public DateTime LastModified { get; set; }

		[XmlElement(ElementName = "AvailableCount")]
		public int AvailableCount { get; set; }

		[XmlElement(ElementName = "Status")]
		public string Status { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "PaxFares")]
	public class PaxFares
	{

		[XmlElement(ElementName = "PaxFare")]
		public PaxFare PaxFare { get; set; }
	}

	[XmlRoot(ElementName = "Fare")]
	public class Fare
	{

		[XmlElement(ElementName = "ClassOfService")]
		public string ClassOfService { get; set; }

		[XmlElement(ElementName = "RuleTariff")]
		public object RuleTariff { get; set; }

		[XmlElement(ElementName = "CarrierCode")]
		public string CarrierCode { get; set; }

		[XmlElement(ElementName = "RuleNumber")]
		public string RuleNumber { get; set; }

		[XmlElement(ElementName = "FareBasis")]
		public string FareBasis { get; set; }

		[XmlElement(ElementName = "PaxFares")]
		public PaxFares PaxFares { get; set; }

		[XmlElement(ElementName = "FareApplicationType")]
		public string FareApplicationType { get; set; }

		[XmlElement(ElementName = "InboundOutbound")]
		public string InboundOutbound { get; set; }

		[XmlElement(ElementName = "IsAllotmentMarketFare")]
		public bool IsAllotmentMarketFare { get; set; }

		[XmlElement(ElementName = "SellKey")]
		public string SellKey { get; set; }

		[XmlElement(ElementName = "TripTypeRule")]
		public string TripTypeRule { get; set; }
	}

	[XmlRoot(ElementName = "Fares")]
	public class Fares
	{

		[XmlElement(ElementName = "Fare")]
		public Fare Fare { get; set; }
	}

	[XmlRoot(ElementName = "PaxLegService")]
	public class PaxLegService
	{

		[XmlElement(ElementName = "LegNumber")]
		public int LegNumber { get; set; }

		[XmlElement(ElementName = "LegSSRServices")]
		public object LegSSRServices { get; set; }

		[XmlElement(ElementName = "LegBaggageList")]
		public object LegBaggageList { get; set; }
	}

	[XmlRoot(ElementName = "PaxLegServices")]
	public class PaxLegServices
	{

		[XmlElement(ElementName = "PaxLegService")]
		public PaxLegService PaxLegService { get; set; }
	}

	[XmlRoot(ElementName = "PaxSegmentService")]
	public class PaxSegmentService
	{

		[XmlElement(ElementName = "PassengerID")]
		public int PassengerID { get; set; }

		[XmlElement(ElementName = "PassengerNumber")]
		public int PassengerNumber { get; set; }

		[XmlElement(ElementName = "PaxLegServices")]
		public PaxLegServices PaxLegServices { get; set; }

		[XmlElement(ElementName = "BoardingSequence")]
		public object BoardingSequence { get; set; }

		[XmlElement(ElementName = "LiftStatus")]
		public string LiftStatus { get; set; }

		[XmlElement(ElementName = "SeatAssignmentLevel")]
		public string SeatAssignmentLevel { get; set; }

		[XmlElement(ElementName = "ActivityDate")]
		public DateTime ActivityDate { get; set; }
	}

	[XmlRoot(ElementName = "PaxSegmentServices")]
	public class PaxSegmentServices
	{

		[XmlElement(ElementName = "PaxSegmentService")]
		public PaxSegmentService PaxSegmentService { get; set; }
	}

	[XmlRoot(ElementName = "Segment")]
	public class SegmentItinerary
	{

		[XmlElement(ElementName = "State")]
		public State State { get; set; }

		[XmlElement(ElementName = "DepartureStation")]
		public string DepartureStation { get; set; }

		[XmlElement(ElementName = "STD")]
		public DateTime STD { get; set; }

		[XmlElement(ElementName = "ArrivalStation")]
		public string ArrivalStation { get; set; }

		[XmlElement(ElementName = "STA")]
		public DateTime STA { get; set; }

		[XmlElement(ElementName = "FlightDesignator")]
		public FlightDesignatorItinerary FlightDesignator { get; set; }

		[XmlElement(ElementName = "SegmentType")]
		public object SegmentType { get; set; }

		[XmlElement(ElementName = "Legs")]
		public Legs Legs { get; set; }

		[XmlElement(ElementName = "SellKey")]
		public string SellKey { get; set; }

		[XmlElement(ElementName = "TrafficRestriction")]
		public int TrafficRestriction { get; set; }

		[XmlElement(ElementName = "International")]
		public bool International { get; set; }

		[XmlElement(ElementName = "Fares")]
		public Fares Fares { get; set; }

		[XmlElement(ElementName = "CancelSegment")]
		public bool CancelSegment { get; set; }

		[XmlElement(ElementName = "ActionStatusCode")]
		public string ActionStatusCode { get; set; }

		[XmlElement(ElementName = "ClassOfService")]
		public string ClassOfService { get; set; }

		[XmlElement(ElementName = "ClassType")]
		public object ClassType { get; set; }

		[XmlElement(ElementName = "PaxCount")]
		public int PaxCount { get; set; }

		[XmlElement(ElementName = "PriorityCode")]
		public object PriorityCode { get; set; }

		[XmlElement(ElementName = "PriorityDate")]
		public DateTime PriorityDate { get; set; }

		[XmlElement(ElementName = "ChangeReasonCode")]
		public object ChangeReasonCode { get; set; }

		[XmlElement(ElementName = "TimeChanged")]
		public bool TimeChanged { get; set; }

		[XmlElement(ElementName = "TripType")]
		public string TripType { get; set; }

		[XmlElement(ElementName = "CheckInStatus")]
		public string CheckInStatus { get; set; }

		[XmlElement(ElementName = "CreatedDate")]
		public DateTime CreatedDate { get; set; }

		[XmlElement(ElementName = "PaxSegmentServices")]
		public PaxSegmentServices PaxSegmentServices { get; set; }

		[XmlElement(ElementName = "FareClassOfService")]
		public string FareClassOfService { get; set; }

		[XmlElement(ElementName = "CabinOfService")]
		public int CabinOfService { get; set; }

		[XmlElement(ElementName = "AVSStatusIndicator")]
		public string AVSStatusIndicator { get; set; }

		[XmlElement(ElementName = "SegmentID")]
		public int SegmentID { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Segments")]
	public class Segments
	{

		[XmlElement(ElementName = "Segment")]
		public SegmentItinerary Segment { get; set; }
	}

	[XmlRoot(ElementName = "JourneyService")]
	public class JourneyService
	{

		[XmlElement(ElementName = "Segments")]
		public SegmentsItinerary Segments { get; set; }

		[XmlElement(ElementName = "NotForGeneralUse")]
		public bool NotForGeneralUse { get; set; }

		[XmlElement(ElementName = "SellKey")]
		public string SellKey { get; set; }

		[XmlElement(ElementName = "Fares")]
		public Fares Fares { get; set; }

		[XmlElement(ElementName = "JourneyClasses")]
		public object JourneyClasses { get; set; }

		[XmlElement(ElementName = "SalesDate")]
		public DateTime SalesDate { get; set; }

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "JourneyServices")]
	public class JourneyServices
	{

		[XmlElement(ElementName = "JourneyService")]
		public List<JourneyService> JourneyService { get; set; }

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "PaxPriceType")]
	public class PaxPriceType
	{

		[XmlElement(ElementName = "PaxType")]
		public string PaxType { get; set; }

		[XmlElement(ElementName = "PaxDiscountCode")]
		public object PaxDiscountCode { get; set; }

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "BookingPassenger")]
	public class BookingPassenger
	{

		[XmlElement(ElementName = "CustomerNumber")]
		public object CustomerNumber { get; set; }

		[XmlElement(ElementName = "PassengerNumber")]
		public int PassengerNumber { get; set; }

		[XmlElement(ElementName = "PaxPriceType")]
		public PaxPriceType PaxPriceType { get; set; }

		[XmlElement(ElementName = "PassengerFees")]
		public object PassengerFees { get; set; }

		[XmlElement(ElementName = "PassengerID")]
		public int PassengerID { get; set; }

		[XmlElement(ElementName = "FamilyNumber")]
		public int FamilyNumber { get; set; }

		[XmlElement(ElementName = "Gender")]
		public string Gender { get; set; }

		[XmlElement(ElementName = "WeightCategory")]
		public string WeightCategory { get; set; }

		[XmlElement(ElementName = "Infant")]
		public bool Infant { get; set; }

		[XmlElement(ElementName = "Nationality")]
		public object Nationality { get; set; }

		[XmlElement(ElementName = "ResidentCountry")]
		public object ResidentCountry { get; set; }

		[XmlElement(ElementName = "TotalCost")]
		public int TotalCost { get; set; }

		[XmlElement(ElementName = "BalanceDue")]
		public int BalanceDue { get; set; }

		[XmlElement(ElementName = "PassengerAddresses")]
		public object PassengerAddresses { get; set; }

		[XmlElement(ElementName = "PassengerTravelDocs")]
		public object PassengerTravelDocs { get; set; }

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "BookingPassengers")]
	public class BookingPassengers
	{

		[XmlElement(ElementName = "BookingPassenger")]
		public BookingPassenger BookingPassenger { get; set; }

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "PriceItineraryByKeysResult")]
	public class PriceItineraryByKeysResult
	{

		[XmlElement(ElementName = "CurrencyCode")]
		public CurrencyCode CurrencyCode { get; set; }

		[XmlElement(ElementName = "Total")]
		public Total Total { get; set; }

		[XmlElement(ElementName = "PriceStatus")]
		public PriceStatus PriceStatus { get; set; }

		[XmlElement(ElementName = "JourneyServices")]
		public JourneyServices JourneyServices { get; set; }

		[XmlElement(ElementName = "BookingPassengers")]
		public BookingPassengers BookingPassengers { get; set; }
	}

	[XmlRoot(ElementName = "Root")]
	public class Root
	{

		[XmlElement(ElementName = "PriceItineraryByKeysResult")]
		public PriceItineraryByKeysResult PriceItineraryByKeysResult { get; set; }

		[XmlAttribute(AttributeName = "xsi")]
		public string Xsi { get; set; }

		[XmlText]
		public string Text { get; set; }
	}
	*/

}
