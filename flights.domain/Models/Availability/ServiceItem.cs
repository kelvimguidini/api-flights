
namespace flights.domain.Models.Availability
{
    public class ServiceItem
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public decimal ServiceItemAmount { get; set; }
    }
}
