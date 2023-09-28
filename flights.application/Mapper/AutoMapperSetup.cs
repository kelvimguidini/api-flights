using AutoMapper;
using flights.domain.Entities;
using flights.domain.Models;
using flights.domain.Models.GetPrice;
using System;
using System.Linq;

namespace flights.application.Mapper
{
    public class AutoMapperSetup : Profile
    {
        public AutoMapperSetup()
        {
            #region DTO To Domain

            CreateMap<PassengerDTO, Passenger>()
                .ForMember(a => a.GivenName, b => b.MapFrom(c => c.GivenName))
                .ForMember(a => a.Surname, b => b.MapFrom(c => c.Surname))
                .ForMember(a => a.Id, b => b.MapFrom(c => Guid.NewGuid().ToString()))
                .ForMember(a => a.Ptc, b => b.MapFrom(c => c.Ptc))
                .ForMember(a => a.DateOfBirth, b => b.MapFrom(c => c.DateOfBirth))
                .ForMember(a => a.Documents, b => b.MapFrom(c => c.Documents))
                .ForMember(a => a.Contacts, b => b.MapFrom(c => c.Contacts))
                ;

            CreateMap<DocumentDTO, Document>()
                .ForMember(a => a.Number, b => b.MapFrom(c => c.DocumentNumber))
                .ForMember(a => a.Type, b => b.MapFrom(c => c.DocumentType))
                ;

            CreateMap<ContactDTO, Contact>()
                .ForMember(a => a.EmailContacts, b => b.MapFrom(c => c.EmailContacts))
                .ForMember(a => a.PhoneContacts, b => b.MapFrom(c => c.PhoneContacts))
                ;

            CreateMap<PhoneDTO, Phone>()
                .ForMember(a => a.Number, b => b.MapFrom(c => c.PhoneNumber))
                .ForMember(a => a.Type, b => b.MapFrom(c => c.PhoneType))
                .ForMember(a => a.CountryCode, b => b.MapFrom(c => c.CountryCode))
                .ForMember(a => a.LocalCode, b => b.MapFrom(c => c.LocalCode))
                ;

            CreateMap<EmailContactDTO, EmailContact>()
                .ForMember(a => a.Email, b => b.MapFrom(c => c.Email))
                ;

            CreateMap<PassengerDTO, PassangerSell>()
                .ForMember(a => a.DocType, b => b.MapFrom(c => c.Documents.FirstOrDefault().DocumentType))
                .ForMember(a => a.FirstName, b => b.MapFrom(c => c.GivenName))
                .ForMember(a => a.LastName, b => b.MapFrom(c => c.Surname))
                .ForMember(a => a.PtcFlights, b => b.MapFrom(c => c.Ptc))
                .ForMember(a => a.DateBirth, b => b.MapFrom(c => c.DateOfBirth))
                .ForMember(a => a.Email, b => b.MapFrom(c => c.Contacts.FirstOrDefault().EmailContacts.FirstOrDefault().Email))
                .ForMember(a => a.DocNumber, b => b.MapFrom(c => c.Documents.FirstOrDefault().DocumentNumber))
                .ForMember(a => a.PhoneType, b => b.MapFrom(c => c.Contacts.FirstOrDefault().PhoneContacts.FirstOrDefault().PhoneType))
                .ForMember(a => a.PhoneCountryCode, b => b.MapFrom(c => c.Contacts.FirstOrDefault().PhoneContacts.FirstOrDefault().CountryCode))
                .ForMember(a => a.PhoneNumber, b => b.MapFrom(c => c.Contacts.FirstOrDefault().PhoneContacts.FirstOrDefault().PhoneNumber))
                .ForMember(a => a.PhoneLocalCode, b => b.MapFrom(c => c.Contacts.FirstOrDefault().PhoneContacts.FirstOrDefault().LocalCode))
                .ForMember(a => a.EmergencyContact, b => b.MapFrom(c => c.EmergencyContacts.FirstOrDefault()))
                .ForMember(a => a.PaxType, b => b.MapFrom(c => c.Ptc.ToUpper()));


            CreateMap<ContactDTO, EmergencyContact>()
                .ForMember(a => a.Email, b => b.MapFrom(c => c.EmailContacts.FirstOrDefault().Email))
                .ForMember(a => a.PhoneType, b => b.MapFrom(c => c.PhoneContacts.FirstOrDefault().PhoneType))
                .ForMember(a => a.PhoneCountryCode, b => b.MapFrom(c => c.PhoneContacts.FirstOrDefault().CountryCode))
                .ForMember(a => a.PhoneNumber, b => b.MapFrom(c => c.PhoneContacts.FirstOrDefault().PhoneNumber))
                .ForMember(a => a.PhoneLocalCode, b => b.MapFrom(c => c.PhoneContacts.FirstOrDefault().LocalCode))
                ;



            CreateMap<domain.Models.Availability.Airport, Airport>()
                .ForMember(a => a.latitude, b => b.MapFrom(c => c.AiportLatitude))
                .ForMember(a => a.longitude, b => b.MapFrom(c => c.AiportLongitude))
                .ForMember(a => a.name_pt, b => b.MapFrom(c => c.AirportName))
                .ForMember(a => a.name_es, b => b.MapFrom(c => c.AirportName))
                .ForMember(a => a.name_en, b => b.MapFrom(c => c.AirportName))
                .ForMember(a => a.city, b => b.MapFrom(c => c.AirportCity))
                .ForMember(a => a.tags, b => b.Ignore())
                .ForMember(a => a._class, b => b.Ignore())
                .ForMember(a => a.weight, b => b.Ignore())
                .ForMember(a => a.iata, b => b.MapFrom(c => c.AirportCode))
                .ReverseMap()
                .AfterMap((src, dst) => dst.AirportCountry = src.city.Split(',')[2].Trim())
            ;


            #endregion

            #region Domain To DTO

            CreateMap<Passenger, PassengerDTO>()
                .ForMember(a => a.GivenName, b => b.MapFrom(c => c.GivenName))
                .ForMember(a => a.Surname, b => b.MapFrom(c => c.Surname))
                .ForMember(a => a.Id, b => b.MapFrom(c => Guid.NewGuid().ToString()))
                .ForMember(a => a.Ptc, b => b.MapFrom(c => c.Ptc))
                .ForMember(a => a.DateOfBirth, b => b.MapFrom(c => c.DateOfBirth))
                .ForMember(a => a.Contacts, b => b.MapFrom(c => c.Contacts))
                .ForMember(a => a.Documents, b => b.MapFrom(c => c.Documents))
                ;

            CreateMap<Document, DocumentDTO>()
                .ForMember(a => a.DocumentNumber, b => b.MapFrom(c => c.Number))
                .ForMember(a => a.DocumentType, b => b.MapFrom(c => c.Type))
                ;

            CreateMap<Contact, ContactDTO>()
                .ForMember(a => a.EmailContacts, b => b.MapFrom(c => c.EmailContacts))
                .ForMember(a => a.PhoneContacts, b => b.MapFrom(c => c.PhoneContacts))
                ;

            CreateMap<Phone, PhoneDTO>()
                .ForMember(a => a.PhoneNumber, b => b.MapFrom(c => c.Number))
                .ForMember(a => a.PhoneType, b => b.MapFrom(c => c.Type))
                .ForMember(a => a.CountryCode, b => b.MapFrom(c => c.CountryCode))
                .ForMember(a => a.LocalCode, b => b.MapFrom(c => c.LocalCode))
                ;

            CreateMap<EmailContact, EmailContactDTO>()
                .ForMember(a => a.Email, b => b.MapFrom(c => c.Email))
                ;

            #endregion
        }
    }
}
