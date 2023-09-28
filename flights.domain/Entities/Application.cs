using System;

namespace flights.domain.Entities
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
