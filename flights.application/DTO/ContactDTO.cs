
using System.Collections.Generic;

namespace flights.domain.Models
{
    public class ContactDTO
    {
        public List<PhoneDTO> PhoneContacts { get; set; }
        public List<EmailContactDTO> EmailContacts { get; set; }
    }
}