
namespace flights.domain.Models.Availability
{
    public class DescriptionInfo
    {
        public string BaggageId { get; set; }
        public Refund Refund { get; set; }
        public Exchange Exchange { get; set; }
        public bool AllowsSeatReservation { get; set; }
    }
}