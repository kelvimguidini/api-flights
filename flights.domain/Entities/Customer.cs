using System;

namespace flights.domain.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Responsible { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
