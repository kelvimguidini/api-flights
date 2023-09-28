using System;
using System.Collections.Generic;

namespace flights.domain.Models
{
    public class PassengerDTO
    {
        public int? Id { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        //public string Gender { get; set; }
        public string Ptc { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<DocumentDTO> Documents { get; set; }
        public List<ContactDTO> Contacts { get; set; }
        public List<ContactDTO> EmergencyContacts { get; set; }
        public string LoyaltPrograms { get; set; }
    }
}