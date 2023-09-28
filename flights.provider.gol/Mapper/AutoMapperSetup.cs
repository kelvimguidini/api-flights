using AutoMapper;
using flights.domain.Models.GetPrice;

namespace flights.provider.gol.Mapper
{
    public class AutoMapperSetup : Profile
    {
        public AutoMapperSetup()
        {
            #region sessão
            CreateMap<GolWS.Homolog.SessionManager.BWSSession, GolWS.Homolog.BookingManager.BWSSession>();


            CreateMap<GolWS.gws.SessionCreate.MessageHeaderFrom, GolWS.gws.ContextChange.MessageHeaderFrom>()
                .ForMember(a => a.PartyId, b => b.MapFrom(c => c.PartyId))
                .ForMember(a => a.Role, b => b.MapFrom(c => c.Role))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderFrom, GolWS.gws.DesignatePrinter.MessageHeaderFrom>()
                .ForMember(a => a.PartyId, b => b.MapFrom(c => c.PartyId))
                .ForMember(a => a.Role, b => b.MapFrom(c => c.Role))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderFrom, GolWS.gws.AdvancedAirShopping.MessageHeaderFrom>()
                .ForMember(a => a.PartyId, b => b.MapFrom(c => c.PartyId))
                .ForMember(a => a.Role, b => b.MapFrom(c => c.Role))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderFrom, GolWS.gws.EnhancedAirBook.MessageHeaderFrom>()
                .ForMember(a => a.PartyId, b => b.MapFrom(c => c.PartyId))
                .ForMember(a => a.Role, b => b.MapFrom(c => c.Role))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderFrom, GolWs.gws.PassengerDetails.MessageHeaderFrom>()
                .ForMember(a => a.PartyId, b => b.MapFrom(c => c.PartyId))
                .ForMember(a => a.Role, b => b.MapFrom(c => c.Role))
                ;

            CreateMap<GolWS.gws.SessionCreate.MessageHeaderFromPartyId, GolWS.gws.ContextChange.MessageHeaderFromPartyId>()
                .ForMember(a => a.type, b => b.MapFrom(c => c.type))
                .ForMember(a => a.Value, b => b.MapFrom(c => c.Value))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderFromPartyId, GolWS.gws.AdvancedAirShopping.MessageHeaderFromPartyId>()
                .ForMember(a => a.type, b => b.MapFrom(c => c.type))
                .ForMember(a => a.Value, b => b.MapFrom(c => c.Value))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderFromPartyId, GolWS.gws.DesignatePrinter.MessageHeaderFromPartyId>()
                .ForMember(a => a.type, b => b.MapFrom(c => c.type))
                .ForMember(a => a.Value, b => b.MapFrom(c => c.Value))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderFromPartyId, GolWS.gws.EnhancedAirBook.MessageHeaderFromPartyId>()
                .ForMember(a => a.type, b => b.MapFrom(c => c.type))
                .ForMember(a => a.Value, b => b.MapFrom(c => c.Value))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderFromPartyId, GolWs.gws.PassengerDetails.MessageHeaderFromPartyId>()
                .ForMember(a => a.type, b => b.MapFrom(c => c.type))
                .ForMember(a => a.Value, b => b.MapFrom(c => c.Value))
                ;

            CreateMap<GolWS.gws.SessionCreate.MessageHeaderTO, GolWS.gws.AdvancedAirShopping.MessageHeaderTO>()
                .ForMember(a => a.PartyId, b => b.MapFrom(c => c.PartyId))
                .ForMember(a => a.Role, b => b.MapFrom(c => c.Role))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderTO, GolWS.gws.ContextChange.MessageHeaderTO>()
                .ForMember(a => a.PartyId, b => b.MapFrom(c => c.PartyId))
                .ForMember(a => a.Role, b => b.MapFrom(c => c.Role))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderTO, GolWS.gws.DesignatePrinter.MessageHeaderTO>()
                .ForMember(a => a.PartyId, b => b.MapFrom(c => c.PartyId))
                .ForMember(a => a.Role, b => b.MapFrom(c => c.Role))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderTO, GolWS.gws.EnhancedAirBook.MessageHeaderTO>()
                .ForMember(a => a.PartyId, b => b.MapFrom(c => c.PartyId))
                .ForMember(a => a.Role, b => b.MapFrom(c => c.Role))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderTO, GolWs.gws.PassengerDetails.MessageHeaderTO>()
                .ForMember(a => a.PartyId, b => b.MapFrom(c => c.PartyId))
                .ForMember(a => a.Role, b => b.MapFrom(c => c.Role))
                ;

            CreateMap<GolWS.gws.SessionCreate.MessageHeaderTOPartyId, GolWS.gws.ContextChange.MessageHeaderTOPartyId>()
                .ForMember(a => a.type, b => b.MapFrom(c => c.type))
                .ForMember(a => a.Value, b => b.MapFrom(c => c.Value))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderTOPartyId, GolWS.gws.AdvancedAirShopping.MessageHeaderTOPartyId>()
                .ForMember(a => a.type, b => b.MapFrom(c => c.type))
                .ForMember(a => a.Value, b => b.MapFrom(c => c.Value))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderTOPartyId, GolWS.gws.DesignatePrinter.MessageHeaderTOPartyId>()
                .ForMember(a => a.type, b => b.MapFrom(c => c.type))
                .ForMember(a => a.Value, b => b.MapFrom(c => c.Value))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderTOPartyId, GolWS.gws.EnhancedAirBook.MessageHeaderTOPartyId>()
                .ForMember(a => a.type, b => b.MapFrom(c => c.type))
                .ForMember(a => a.Value, b => b.MapFrom(c => c.Value))
                ;
            CreateMap<GolWS.gws.SessionCreate.MessageHeaderTOPartyId, GolWs.gws.PassengerDetails.MessageHeaderTOPartyId>()
                .ForMember(a => a.type, b => b.MapFrom(c => c.type))
                .ForMember(a => a.Value, b => b.MapFrom(c => c.Value))
                ;

            #endregion

            #region Disponibilidade

            CreateMap<object, GolWS.Homolog.BookingManager.AvailabilityRequest>()
                .ForMember(a => a.DepartureStation, b => b.MapFrom(c => c.GetType().GetProperty("DepartureCode").GetValue(c, null)))
                .ForMember(a => a.ArrivalStation, b => b.MapFrom(c => c.GetType().GetProperty("ArrivalCode").GetValue(c, null)))
                .ForMember(a => a.BeginDate, b => b.MapFrom(c => c.GetType().GetProperty("DepartureDate").GetValue(c, null)))
                .ForMember(a => a.EndDate, b => b.MapFrom(c => c.GetType().GetProperty("DepartureDate").GetValue(c, null)))
                .ForMember(a => a.PaxCount, b => b.MapFrom(c => c.GetType().GetProperty("CountTotalPassangers").GetValue(c, null)))
                .ForMember(a => a.FlightType, b => b.MapFrom(c => GolWS.Homolog.BookingManager.FlightType.All))
                .ForMember(a => a.Dow, b => b.MapFrom(c => GolWS.Homolog.BookingManager.DOW.Daily))
                .ForMember(a => a.AvailabilityType, b => b.MapFrom(c => GolWS.Homolog.BookingManager.AvailabilityType.Default))
                .ForMember(a => a.FareClassControl, b => b.MapFrom(c => GolWS.Homolog.BookingManager.FareClassControl.CompressByProductClass))
                .ForMember(a => a.SSRCollectionsMode, b => b.MapFrom(c => GolWS.Homolog.BookingManager.SSRCollectionsMode.None))
                .ForMember(a => a.InboundOutbound, b => b.MapFrom(c => GolWS.Homolog.BookingManager.InboundOutbound.None))
                .ForMember(a => a.CurrencyCode, b => b.MapFrom(c => "BRL"))
                .ForMember(a => a.MaximumConnectingFlights, b => b.MapFrom(c => 99))
                .ForMember(a => a.NightsStay, b => b.MapFrom(c => 0))
                .ForMember(a => a.IncludeAllotments, b => b.MapFrom(c => false))
                .ForMember(a => a.IncludeTaxesAndFees, b => b.MapFrom(c => true))
                .ForMember(a => a.BeginTime, b => b.MapFrom(c => new GolWS.Homolog.BookingManager.Time() { TotalMinutes = 0 }))
                .ForMember(a => a.EndTime, b => b.MapFrom(c => new GolWS.Homolog.BookingManager.Time() { TotalMinutes = 1439 }))
            ;
            #endregion

            #region Tarifação
            CreateMap<GetPriceRQ, GolWS.Homolog.BookingManager.ItineraryPriceRequest>()
                .ForMember(a => a.PriceItineraryBy, b => b.MapFrom(c => GolWS.Homolog.BookingManager.PriceItineraryBy.JourneyBySellKey))
                .ForMember(a => a.SellByKeyRequest, b => b.MapFrom(c => c))
                ;

            CreateMap<GetPriceRQ, GolWS.Homolog.BookingManager.SellJourneyByKeyRequestData>()
               .ForMember(a => a.PaxPriceType, b => b.MapFrom(c => c.Passagers))
                .ForMember(a => a.CurrencyCode, b => b.MapFrom(c => c.CurrencyCode))
                .ForMember(a => a.PaxCount, b => b.MapFrom(c => 1))
                .ForMember(a => a.LoyaltyFilter, b => b.MapFrom(c => GolWS.Homolog.BookingManager.LoyaltyFilter.MonetaryOnly))
                .ForMember(a => a.JourneySellKeys, b => b.MapFrom(c => c.JorneysSell))
                .ForMember(a => a.ActionStatusCode, b => b.MapFrom(c => "NN"))
                ;

            CreateMap<PassangerSell, GolWS.Homolog.BookingManager.PaxPriceType>()
                .ForMember(a => a.PaxType, b => b.MapFrom(c => c.PaxType));

            CreateMap<JourneySell, GolWS.Homolog.BookingManager.SellKeyList>()
                .ForMember(a => a.JourneySellKey, b => b.MapFrom(c => c.JourneySellKey))
                .ForMember(a => a.FareSellKey, b => b.MapFrom(c => c.FareSellKey))
                ;
            #endregion

            #region Booking
            CreateMap<GetPriceRQ, GolWS.Homolog.BookingManager.SellRequestData>()
                .ForMember(a => a.SellBy, b => b.MapFrom(c => GolWS.Homolog.BookingManager.PriceItineraryBy.JourneyBySellKey))
                .ForMember(a => a.SellJourneyByKeyRequest, b => b.MapFrom(c => c))
                ;

            CreateMap<GetPriceRQ, GolWS.Homolog.BookingManager.SellJourneyByKeyRequest>()
               .ForMember(a => a.SellJourneyByKeyRequestData, b => b.MapFrom(c => c))
                ;

            #endregion
        }
    }
}
