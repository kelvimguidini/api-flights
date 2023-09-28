using System;

namespace flights.domain.Entities
{
    public class Agency
    {
        public int AgencyId { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
