using AutoMapper;
using flights.domain.Models.GetPrice;
using System.Collections.Generic;
using AzulWS;
using flights.provider.azul.Models;
using System.Linq;
using flights.domain.Models;
using flights.domain.Models.Provider.Azul;

namespace flights.provider.azul.Mapper
{
    public class AutoMapperSetup : Profile
    {
        public AutoMapperSetup()
        {
            #region sessão
            CreateMap<AzulWS.LogonResponse, AzulWS.SessionContext>();

            CreateMap<SessionAzulRequest, AzulWS.SessionContext>().ReverseMap();
            CreateMap<SessionAzulRequest, SessionProvider>().ReverseMap();

            //CreateMap<GolWS.Homolog.SessionManager.BWSSession, GolWS.Homolog.BookingManager.BWSSession>();
            #endregion

            #region Availability

            //CreateMap<object, GolWS.Homolog.BookingManager.AvailabilityRequest>()
            //    .ForMember(a => a.DepartureStation, b => b.MapFrom(c => c.GetType().GetProperty("DepartureCode").GetValue(c, null)))
            //    .ForMember(a => a.ArrivalStation, b => b.MapFrom(c => c.GetType().GetProperty("ArrivalCode").GetValue(c, null)))
            //    .ForMember(a => a.BeginDate, b => b.MapFrom(c => c.GetType().GetProperty("DepartureDate").GetValue(c, null)))
            //    .ForMember(a => a.EndDate, b => b.MapFrom(c => c.GetType().GetProperty("DepartureDate").GetValue(c, null)))
            //    .ForMember(a => a.PaxCount, b => b.MapFrom(c => c.GetType().GetProperty("CountTotalPassangers").GetValue(c, null)))
            //    .ForMember(a => a.FlightType, b => b.MapFrom(c => GolWS.Homolog.BookingManager.FlightType.All))
            //    .ForMember(a => a.Dow, b => b.MapFrom(c => GolWS.Homolog.BookingManager.DOW.Daily))
            //    .ForMember(a => a.AvailabilityType, b => b.MapFrom(c => GolWS.Homolog.BookingManager.AvailabilityType.Default))
            //    .ForMember(a => a.FareClassControl, b => b.MapFrom(c => GolWS.Homolog.BookingManager.FareClassControl.CompressByProductClass))
            //    .ForMember(a => a.SSRCollectionsMode, b => b.MapFrom(c => GolWS.Homolog.BookingManager.SSRCollectionsMode.None))
            //    .ForMember(a => a.InboundOutbound, b => b.MapFrom(c => GolWS.Homolog.BookingManager.InboundOutbound.None))
            //    .ForMember(a => a.CurrencyCode, b => b.MapFrom(c => "BRL"))
            //    .ForMember(a => a.MaximumConnectingFlights, b => b.MapFrom(c => 99))
            //    .ForMember(a => a.NightsStay, b => b.MapFrom(c => 0))
            //    .ForMember(a => a.IncludeAllotments, b => b.MapFrom(c => false))
            //    .ForMember(a => a.IncludeTaxesAndFees, b => b.MapFrom(c => true))
            //    .ForMember(a => a.BeginTime, b => b.MapFrom(c => new GolWS.Homolog.BookingManager.Time() { TotalMinutes = 0 }))
            //    .ForMember(a => a.EndTime, b => b.MapFrom(c => new GolWS.Homolog.BookingManager.Time() { TotalMinutes = 1439 }))
            //;

            CreateMap<AzulWS.AvailabilityRequest, AvailabilityAzulRequest>()
                .ForMember(a => a.PaxPriceTypes, b => b.MapFrom(c => c.PaxPriceTypes.ToList()));

            CreateMap<AzulWS.PaxPriceType, flights.provider.azul.Models.PaxPriceType>();

            CreateMap<AzulWS.Time, flights.provider.azul.Models.Time>();
            //CreateMap<AvailabilityAzulRequest, object>().ReverseMap();
            #endregion

            #region Disponibilidade
            CreateMap<object, AzulWS.AvailabilityRequest>()
                .ForMember(a => a.DepartureStation, b => b.MapFrom(c => c.GetType().GetProperty("DepartureCode").GetValue(c, null).ToString().ToUpper()))
                .ForMember(a => a.ArrivalStation, b => b.MapFrom(c => c.GetType().GetProperty("ArrivalCode").GetValue(c, null).ToString().ToUpper()))
                .ForMember(a => a.BeginDate, b => b.MapFrom(c => c.GetType().GetProperty("DepartureDate").GetValue(c, null)))
                .ForMember(a => a.EndDate, b => b.MapFrom(c => c.GetType().GetProperty("ReturnDate").GetValue(c, null)))
                //.ForMember(a => a.CarrierCode, b => b.MapFrom(c => "AD"))
                //.ForMember(a => a.PaxCount, b => b.MapFrom(c => c.GetType().GetProperty("CountTotalPassangers").GetValue(c, null))) //****
                .ForMember(a => a.FlightType, b => b.MapFrom(c => AzulWS.FlightType.All))
                .ForMember(a => a.Dow, b => b.MapFrom(c => AzulWS.DOW.Daily))
                .ForMember(a => a.AvailabilityType, b => b.MapFrom(c => AzulWS.AvailabilityType.Default))
                //.ForMember(a => a.SourceOrganization, b => b.MapFrom(c => "01401552")) //////***************
                .ForMember(a => a.FareClassControl, b => b.MapFrom(c => AzulWS.FareClassControl.CompressByProductClass))
                //.ForMember(a => a.SSRCollectionsMode, b => b.MapFrom(c => AzulWS.SSRCollectionsMode.None))
                .ForMember(a => a.InboundOutbound, b => b.MapFrom(c => AzulWS.InboundOutbound.None))
                .ForMember(a => a.CurrencyCode, b => b.MapFrom(c => "BRL"))
                .ForMember(a => a.DisplayCurrencyCode, b => b.MapFrom(c => "BRL"))
                .ForMember(a => a.MaximumConnectingFlights, b => b.MapFrom(c => 99)) // estava 99
                .ForMember(a => a.NightsStay, b => b.MapFrom(c => 0))
                .ForMember(a => a.IncludeAllotments, b => b.MapFrom(c => false))
                //.ForMember(a => a.IncludeTaxesAndFees, b => b.MapFrom(c => true))
                .ForMember(a => a.AvailabilityFilter, b => b.MapFrom(c => AzulWS.AvailabilityFilter.ExcludeUnavailable)) 
                //.ForMember(a => a.BeginTime, b => b.MapFrom(c => new AzulWS.Time() { TotalMinutes = 0 }))
                //.ForMember(a => a.EndTime, b => b.MapFrom(c => new AzulWS.Time() { TotalMinutes = 1439 }))
                //.ForMember(a => a.PaxPriceTypes, b => b.MapFrom(c => new List<AzulWS.PaxPriceType> { new PaxPriceType() { PaxType = "ADT" }, new PaxPriceType() { PaxType = "CHD" } }))
            ;
            #endregion

            #region Tarifação

            CreateMap<GetPriceRQ, AzulWS.PriceItineraryRequest>()
                //.ForMember(a => a.PriceItineraryBy, b => b.MapFrom(c => AzulWS.BookingManager.PriceItineraryBy.JourneyBySellKey))
                //.ForMember(a => a.SellByKeyRequest, b => b.MapFrom(c => c))
                ;

            //CreateMap<GetPriceRQ, AzulWS.SellJourneyByKeyRequestData>()
            //   .ForMember(a => a.PaxPriceType, b => b.MapFrom(c => c.PaxTypes))
            //    .ForMember(a => a.CurrencyCode, b => b.MapFrom(c => c.CurrencyCode))
            //    .ForMember(a => a.PaxCount, b => b.MapFrom(c => 1))
            //    .ForMember(a => a.LoyaltyFilter, b => b.MapFrom(c => AzulWS.Homolog.BookingManager.LoyaltyFilter.MonetaryOnly))
            //    .ForMember(a => a.JourneySellKeys, b => b.MapFrom(c => c.JorneysSell))
            //    ;

            //CreateMap<PaxPriceType, AzulWS.Homolog.BookingManager.PaxPriceType>()
            //    .ForMember(a => a.PaxType, b => b.MapFrom(c => c.PaxType));

            //CreateMap<JourneySell, AzulWS.Homolog.BookingManager.SellKeyList>()
            //            .ForMember(a => a.JourneySellKey, b => b.MapFrom(c => c.JourneySellKey))
            //.ForMember(a => a.FareSellKey, b => b.MapFrom(c => c.FareSellKey))
            //    ;
            #endregion

            #region Booking
            CreateMap<PriceItineraryRequestWithKeys, SellWithKeyRequest>();

            CreateMap<object, SellWithKeyRequest>();

            CreateMap<object, SellRequestWithKeys>();

            CreateMap<SellRequestWithKeys, SellWithKeyRequest>()
                .ForMember(a => a.SellKeyList, b => b.MapFrom(c => c.SellKeyList))
                .ForMember(a => a.PaxPriceTypes, b => b.MapFrom(c => c.PaxPriceTypes))
                .ReverseMap();

            CreateMap< AzulWS.SellKeys, flights.domain.Models.Provider.Azul.SellKeys>().ReverseMap();

            CreateMap<AzulWS.PaxPriceType, flights.domain.Models.Provider.Azul.PaxPriceType>().ReverseMap();



            #endregion

        }
    }
}
