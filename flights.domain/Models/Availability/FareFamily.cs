
namespace flights.domain.Models.Availability
{
    public class FareFamily
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Owner { get; set; }
        public DescriptionInfo DescriptionInfo { get; set; }
        public string links { get; set; }
    }
}
