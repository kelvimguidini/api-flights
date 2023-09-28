using System;


namespace flights.domain.Entities
{
    public class Provider
    {
        public int ProviderId { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public string UrlProduction { get; set; }
        public string UrlHomolog { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
