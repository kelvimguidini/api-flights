using System.Collections.Generic;

namespace flights.domain.Models
{
    public class Contact
    {
        public List<Phone> PhoneContacts { get; set; }
        public List<EmailContact> EmailContacts { get; set; }
    }
}
