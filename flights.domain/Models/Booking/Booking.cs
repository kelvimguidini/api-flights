using flights.domain.Models.Availability;
using System;
using System.Collections.Generic;

namespace flights.domain.Models.Booking
{
    public class Booking
    {
        public string OrderId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string RecordLocator { get; set; }
        public List<Passenger> Passengers { get; set; }
        public string OrderStatus { get; set; }
        public List<string> Etickets { get; set; }
        public Offer OrderInfo { get; set; }
        public DataList DataList { get; set; }
        public DateTime? TimeLimit { get; set; }
    }
}