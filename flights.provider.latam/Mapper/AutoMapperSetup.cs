using AutoMapper;
using LatamWS.Homolog.AdvancedAirShopping;
using System;

namespace flights.provider.latam.Mapper
{
    public class AutoMapperSetup : Profile
    {
        public AutoMapperSetup()
        {

            #region Disponibilidade

            //MESSAGEHEADER
            CreateMap<LatamWS.Homolog.SessionCreate.MessageHeader, MessageHeader>();
            CreateMap<LatamWS.Homolog.SessionCreate.To, To>();
            CreateMap<LatamWS.Homolog.SessionCreate.From, From>();
            CreateMap<LatamWS.Homolog.SessionCreate.Service, Service>();
            CreateMap<LatamWS.Homolog.SessionCreate.MessageData, MessageData>();
            CreateMap<LatamWS.Homolog.SessionCreate.Description, Description>();
            CreateMap<LatamWS.Homolog.SessionCreate.PartyId, PartyId>();

            //SECURITY
            CreateMap<LatamWS.Homolog.SessionCreate.Security, Security>();
            CreateMap<LatamWS.Homolog.SessionCreate.SecurityUsernameToken, SecurityUsernameToken>();

            #endregion

            #region Tarifação
            //CreateMap<GetPriceRQ, GolWS.Homolog.BookingManager.ItineraryPriceRequest>()
            //    .ForMember(a => a.PriceItineraryBy, b => b.MapFrom(c => GolWS.Homolog.BookingManager.PriceItineraryBy.JourneyBySellKey))
            //    .ForMember(a => a.SellByKeyRequest, b => b.MapFrom(c => c))
            //    ;

            //CreateMap<GetPriceRQ, GolWS.Homolog.BookingManager.SellJourneyByKeyRequestData>()
            //   .ForMember(a => a.PaxPriceType, b => b.MapFrom(c => c.PaxTypes))
            //    .ForMember(a => a.CurrencyCode, b => b.MapFrom(c => c.CurrencyCode))
            //    .ForMember(a => a.PaxCount, b => b.MapFrom(c => 1))
            //    .ForMember(a => a.LoyaltyFilter, b => b.MapFrom(c => GolWS.Homolog.BookingManager.LoyaltyFilter.MonetaryOnly))
            //    .ForMember(a => a.JourneySellKeys, b => b.MapFrom(c => c.JorneysSell))
            //    .ForMember(a => a.ActionStatusCode, b => b.MapFrom(c => "NN"))
            //    ;

            //CreateMap<PaxPriceType, GolWS.Homolog.BookingManager.PaxPriceType>()
            //    .ForMember(a => a.PaxType, b => b.MapFrom(c => c.PaxType));

            //CreateMap<JourneySell, GolWS.Homolog.BookingManager.SellKeyList>()
            //    .ForMember(a => a.JourneySellKey, b => b.MapFrom(c => c.JourneySellKey))
            //    .ForMember(a => a.FareSellKey, b => b.MapFrom(c => c.FareSellKey))
            //    ;
            #endregion

            #region Booking
            //CreateMap<GetPriceRQ, GolWS.Homolog.BookingManager.SellRequestData>()
            //    .ForMember(a => a.SellBy, b => b.MapFrom(c => GolWS.Homolog.BookingManager.PriceItineraryBy.JourneyBySellKey))
            //    .ForMember(a => a.SellJourneyByKeyRequest, b => b.MapFrom(c => c))
            //    ;

            //CreateMap<GetPriceRQ, GolWS.Homolog.BookingManager.SellJourneyByKeyRequest>()
            //   .ForMember(a => a.SellJourneyByKeyRequestData, b => b.MapFrom(c => c))
            //    ;

            #endregion
        }
    }
}
