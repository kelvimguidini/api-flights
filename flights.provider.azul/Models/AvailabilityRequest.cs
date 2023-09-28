using AzulWS;
using flights.crosscutting.DomainObjects;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace flights.provider.azul.Models
{
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.navitaire.com/Messages/Booking")]

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "GetAvailability", Namespace = "http://schemas.navitaire.com/Common")]
    //[DataContract(Name = "GetAvailability")]
    public class SessionContextRequest : WCFStructure
    {
        //[XmlElement(Namespace = "http://schemas.navitaire.com/ClientServices/BookingManager/BookingManagerClient")]
        [DataMember(Name = "session")]
        public SessionAzulRequest session { get; set; }

        //[XmlElement(Namespace = "http://schemas.navitaire.com/ClientServices/BookingManager/BookingManagerClient")]
        [DataMember(Name = "availabilityRequest")]
        public AvailabilityAzulRequest availabilityRequest { get; set; }

    }

    #region Session
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "session", Namespace = "http://schemas.navitaire.com/Common")]
    //[DataContract(Name = "session", Namespace = "http://schemas.navitaire.com/Common")]
    public class SessionAzulRequest
    {
        public SessionControl SessionControl { get; set; }

        [DataMember]
        public SystemType SystemType { get; set; }

        [DataMember(Order = 2)]
        public long SessionID { get; set; }

        [DataMember(Order = 3)]
        public int SequenceNumber { get; set; }

        [DataMember(Order = 4)]
        public int MessageVersion { get; set; }

        [DataMember(Order = 5)]
        public string Signature { get; set; }

        [DataMember(Order = 6)]
        public Principal SessionPrincipal { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public string LocationCode { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 8)]
        public string CultureCode { get; set; }

        [DataMember(Order = 9)]
        public ChannelType ChannelType { get; set; }

        [DataMember(Order = 10)]
        public bool InTransaction { get; set; }

        [DataMember(Order = 11)]
        public short TransactionDepth { get; set; }

        [DataMember(Order = 12)]
        public long TransactionCount { get; set; }

        [DataMember(Order = 13)]
        public string SecureToken { get; set; }
    }

    [DataContract(Name = "SessionControl", Namespace = "http://schemas.navitaire.com/Common/Enumerations")]
    public enum SessionControl : int
    {
        [EnumMember()]
        OneOnly = 0,

        [EnumMember()]
        First = 1,

        [EnumMember()]
        Middle = 2,

        [EnumMember()]
        Last = 3,
    }

    [DataContract(Name = "SystemType", Namespace = "http://schemas.navitaire.com/Common/Enumerations")]
    public enum SystemType : int
    {

        [EnumMember()]
        Default = 0,

        [EnumMember()]
        WinRez = 1,

        [EnumMember()]
        FareManager = 2,

        [EnumMember()]
        ScheduleManager = 3,

        [EnumMember()]
        WinManager = 4,

        [EnumMember()]
        ConsoleRez = 5,

        [EnumMember()]
        WebRez = 6,

        [EnumMember()]
        WebServicesAPI = 7,

        [EnumMember()]
        WebServicesESC = 8,

        [EnumMember()]
        InternalService = 9,

        [EnumMember()]
        WebReporting = 10,

        [EnumMember()]
        TaxAndFeeManager = 11,
    }
    #endregion

    #region AvailabilityRequest
    [DataContract(Name = "availabilityRequest", Namespace = "http://schemas.navitaire.com/Messages/Booking")]
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
   // [XmlType(Namespace = "http://schemas.navitaire.com/Messages/Booking")]
    public class AvailabilityAzulRequest
    {
        [DataMember(Order = 0)]
        public DateTime TADepartureDate { get; set; }

        
        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string TAArrivalStation { get; set; }

        
        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string TADepartureStation { get; set; }

        
        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string TAJourneyType { get; set; }

        
        [DataMember(Order = 4)]
        public string DepartureStation { get; set; }

        
        [DataMember(Order = 5)]
        public string ArrivalStation { get; set; }

        
        [DataMember(Order = 6)]
        public DateTime BeginDate { get; set; }

        
        [DataMember(Order = 7)]
        public DateTime EndDate { get; set; }

        
        [DataMember(Order = 8)]
        public string CarrierCode { get; set; }

        
        [DataMember(EmitDefaultValue = false, Order = 9)]
        public string FlightNumber { get; set; }

        
        [DataMember(Order = 10)]
        public FlightType FlightType { get; set; }

        
        [DataMember(Order = 11)]
        public short PaxCount { get; set; }

        
        [DataMember(Order = 12)]
        public DOW Dow { get; set; }

        
        [DataMember(Order = 13)]
        public string CurrencyCode { get; set; }

        
        [DataMember(Order = 14)]
        public string DisplayCurrencyCode { get; set; }

        
        [DataMember(EmitDefaultValue = false, Order = 15)]
        public string DiscountCode { get; set; }

        
        [DataMember(EmitDefaultValue = false, Order = 16)]
        public string PromotionCode { get; set; }

        
        [DataMember(Order = 17)]
        public AvailabilityType AvailabilityType { get; set; }

        
        [DataMember(Order = 18)]
        public string SourceOrganization { get; set; }

        
        [DataMember(Order = 19)]
        public short MaximumConnectingFlights { get; set; }

        
        [DataMember(Order = 20)]
        public AvailabilityFilter AvailabilityFilter { get; set; }

        
        [DataMember(Order = 21)]
        public FareClassControl FareClassControl { get; set; }

        
        [DataMember(Order = 22)]
        public decimal MinimumFarePrice { get; set; }

        
        [DataMember(Order = 23)]
        public decimal MaximumFarePrice { get; set; }

        
        [DataMember(EmitDefaultValue = false, Order = 24)]
        public string ProductClassCode { get; set; }

        
        [DataMember(Order = 25)]
        public SSRCollectionsMode SSRCollectionsMode { get; set; }

        
        [DataMember(Order = 26)]
        public InboundOutbound InboundOutbound { get; set; }

        
        [DataMember(Order = 27)]
        public int NightsStay { get; set; }

        
        [DataMember(Order = 28)]
        public bool IncludeAllotments { get; set; }

        
        [DataMember(Order = 29)]
        public Time BeginTime { get; set; }

        
        [DataMember(Order = 30)]
        public Time EndTime { get; set; }

        
        [DataMember(Order = 31)]
        public string[] DepartureStations { get; set; }

        
        [DataMember(Order = 32)]
        public string[] ArrivalStations { get; set; }

        
        [DataMember(Order = 33)]
        public string[] FareTypes { get; set; }

        
        [DataMember(Order = 34)]
        public string[] FareClasses { get; set; }

        
        [DataMember(Order = 35)]
        public string[] ProductClasses { get; set; }

        
        [DataMember(Order = 36)]
        //[XmlArrayItem(Namespace = "http://schemas.navitaire.com/Messages/Itinerary")]
        public PaxPriceType[] PaxPriceTypes { get; set; }

        
        [DataMember(Order = 37)]
        public JourneySortKey[] JourneySortKeys { get; set; }
    }

    [DataContract(Namespace = "http://schemas.navitaire.com/Common")]
    public partial class Time
    {
        //[System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public int TotalMinutes { get; set; }
    }

    public class PaxPriceType
    {
        //[DataMember(Order = 0)]
        public string PaxType { get; set; }

        /// <remarks/>
        //[DataMember(EmitDefaultValue = false, Order = 1)]
        public string PaxDiscountCode { get; set; }
    }

    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.navitaire.com/Common/Enumerations")]
    public enum FlightType
    {
        None,
        NonStop,
        Through,
        Direct,
        Connect,
        All,
    }

    //[System.FlagsAttribute()]
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.navitaire.com/Common/Enumerations")]
    public enum DOW
    {
        None = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        WeekDay = 64,
        Saturday = 128,
        Sunday = 256,
        WeekEnd = 512,
        Daily = 1024,
    }

    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.navitaire.com/Common/Enumerations")]
    public enum AvailabilityType
    {
        Default,
        Standby,
        Overbook,
        Move,
    }

    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.navitaire.com/Common/Enumerations")]
    public enum AvailabilityFilter
    {
        Default,
        ExcludeDeparted,
        ExcludeImminent,
        ExcludeUnavailable,
    }

    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.navitaire.com/Common/Enumerations")]
    public enum FareClassControl
    {
        LowestFareClass,
        CompressByProductClass,
        Default,
    }

    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.navitaire.com/Common/Enumerations")]
    public enum SSRCollectionsMode
    {
        None,
        Leg,
        Segment,
        All,
    }

    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.navitaire.com/Common/Enumerations")]
    public enum InboundOutbound
    {
        None,
        Inbound,
        Outbound,
        Both,
        RoundFrom,
        RoundTo,
    }

    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.navitaire.com/Common/Enumerations")]
    public enum JourneySortKey
    {
        ServiceType,
        ShortestTravelTime,
        LowestFare,
        HighestFare,
        EarliestDeparture,
        LatestDeparture,
        EarliestArrival,
        LatestArrival,
        NoSort,
    }
    #endregion



    public class teste
    {
        public testeb testeb { get; set; }
    }

    public class testeb
    {
        public string titulo { get; set; }
    }
}

