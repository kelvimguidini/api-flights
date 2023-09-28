using System;
using System.Collections.Generic;
using System.Text;

namespace flights.application.DTO
{
    public class AvailabilityRQDTO
    {
        public string DepartureCode { get; set; }
        public string ArrivalCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int CountADT { get; set; }
        public int CountCHD { get; set; }
        public int CountINF { get; set; }
        public int CountTotalPassangers { get; set; }

    }
}
