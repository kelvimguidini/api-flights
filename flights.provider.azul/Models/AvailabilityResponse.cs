using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace flights.provider.azul.Models
{
	[XmlRoot(ElementName = "GetAvailabilityByTripResult")]
	public class GetAvailabilityByTripResult
	{

		[XmlElement(ElementName = "Schedules")]
		public Schedules Schedules { get; set; }
	}

	[XmlRoot(ElementName = "Schedules")]
	public class Schedules
	{

		[XmlElement(ElementName = "ArrayOfJourneyDateMarket")]
		public List<ArrayOfJourneyDateMarket> ArrayOfJourneyDateMarket { get; set; }
	}

	[XmlRoot(ElementName = "ArrayOfJourneyDateMarket")]
	public class ArrayOfJourneyDateMarket
	{

		[XmlElement(ElementName = "JourneyDateMarket")]
		public JourneyDateMarket JourneyDateMarket { get; set; }
	}


	[XmlRoot(ElementName = "FlightDesignator")]
	public class FlightDesignator
	{

		[XmlElement(ElementName = "CarrierCode")]
		public string CarrierCode { get; set; }

		[XmlElement(ElementName = "FlightNumber")]
		public int FlightNumber { get; set; }

		[XmlElement(ElementName = "OpSuffix")]
		public int OpSuffix { get; set; }
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
		public FlightDesignator FlightDesignator { get; set; }

		[XmlElement(ElementName = "Status")]
		public string Status { get; set; }

		[XmlElement(ElementName = "InventoryLegKey")]
		public string InventoryLegKey { get; set; }

		[XmlElement(ElementName = "AircraftType")]
		public string AircraftType { get; set; }

		[XmlElement(ElementName = "AircraftTypeSuffix")]
		public object AircraftTypeSuffix { get; set; }

		[XmlElement(ElementName = "PRBCCode")]
		public string PRBCCode { get; set; }

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
	}

	[XmlRoot(ElementName = "Legs")]
	public class Legs
	{

		[XmlElement(ElementName = "Leg")]
		public List<Leg> Leg { get; set; }
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
	}

	[XmlRoot(ElementName = "PaxFares")]
	public class PaxFares
	{

		[XmlElement(ElementName = "PaxFare")]
		public List<PaxFare> PaxFare { get; set; }
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
		public List<Fare> Fare { get; set; }
	}

	[XmlRoot(ElementName = "Segment")]
	public class Segment
	{

		[XmlElement(ElementName = "State")]
		public string State { get; set; }

		[XmlElement(ElementName = "DepartureStation")]
		public string DepartureStation { get; set; }

		[XmlElement(ElementName = "STD")]
		public DateTime STD { get; set; }

		[XmlElement(ElementName = "ArrivalStation")]
		public string ArrivalStation { get; set; }

		[XmlElement(ElementName = "STA")]
		public DateTime STA { get; set; }

		[XmlElement(ElementName = "FlightDesignator")]
		public FlightDesignator FlightDesignator { get; set; }

		[XmlElement(ElementName = "SegmentType")]
		public object SegmentType { get; set; }

		[XmlElement(ElementName = "Legs")]
		public Legs Legs { get; set; }

		[XmlElement(ElementName = "SellKey")]
		public object SellKey { get; set; }

		[XmlElement(ElementName = "TrafficRestriction")]
		public int TrafficRestriction { get; set; }

		[XmlElement(ElementName = "DeptTerminalOverride")]
		public object DeptTerminalOverride { get; set; }

		[XmlElement(ElementName = "ArrvTerminalOverride")]
		public object ArrvTerminalOverride { get; set; }

		[XmlElement(ElementName = "JointOpInfoOverride")]
		public object JointOpInfoOverride { get; set; }

		[XmlElement(ElementName = "International")]
		public bool International { get; set; }

		[XmlElement(ElementName = "InventorySegmentSSRNests")]
		public object InventorySegmentSSRNests { get; set; }

		[XmlElement(ElementName = "Fares")]
		public Fares Fares { get; set; }

		[XmlElement(ElementName = "SegmentClasses")]
		public object SegmentClasses { get; set; }

		[XmlElement(ElementName = "XRefFlightDesignator")]
		public XRefFlightDesignator XRefFlightDesignator { get; set; }
	}

	[XmlRoot(ElementName = "Segments")]
	public class SegmentsItinerary
	{

		[XmlElement(ElementName = "Segment")]
		public List<Segment> Segment { get; set; }
	}

	[XmlRoot(ElementName = "InventoryJourney")]
	public class InventoryJourney
	{

		[XmlElement(ElementName = "Segments")]
		public SegmentsItinerary Segments { get; set; }

		[XmlElement(ElementName = "NotForGeneralUse")]
		public bool NotForGeneralUse { get; set; }

		[XmlElement(ElementName = "SellKey")]
		public string SellKey { get; set; }

		[XmlElement(ElementName = "Fares")]
		public Fares Fares { get; set; }
	}

	[XmlRoot(ElementName = "XRefFlightDesignator")]
	public class XRefFlightDesignator
	{

		[XmlElement(ElementName = "CarrierCode")]
		public string CarrierCode { get; set; }

		[XmlElement(ElementName = "FlightNumber")]
		public int FlightNumber { get; set; }

		[XmlElement(ElementName = "OpSuffix")]
		public int OpSuffix { get; set; }
	}

	[XmlRoot(ElementName = "Journeys")]
	public class Journeys
	{

		[XmlElement(ElementName = "InventoryJourney")]
		public List<InventoryJourney> InventoryJourney { get; set; }
	}

	[XmlRoot(ElementName = "JourneyDateMarket")]
	public class JourneyDateMarket
	{

		[XmlElement(ElementName = "DepartureDate")]
		public DateTime DepartureDate { get; set; }

		[XmlElement(ElementName = "DepartureStation")]
		public string DepartureStation { get; set; }

		[XmlElement(ElementName = "ArrivalStation")]
		public string ArrivalStation { get; set; }

		[XmlElement(ElementName = "Journeys")]
		public Journeys Journeys { get; set; }
	}

}

