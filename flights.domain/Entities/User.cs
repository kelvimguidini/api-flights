using System;

namespace flights.domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public bool Active { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int ApplicationId { get; set; }
        public virtual Application Application { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
