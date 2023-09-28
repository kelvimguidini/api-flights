using System;
using System.Collections.Generic;

namespace flights.domain.Models
{
    public class Passenger
    {
        public string Id { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Ptc { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<Document> Documents { get; set; }
        public string LoyaltPrograms { get; set; }
    }
}
