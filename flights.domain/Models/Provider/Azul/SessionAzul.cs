using System;
using System.Collections.Generic;
using System.Text;

namespace flights.domain.Models.Provider.Azul
{
    public class SessionAzul
    {
        public string ChannelType { get; set; } //enum
        public string CultureCode { get; set; }
        public bool InTransaction { get; set; }
        public string LocationCode { get; set; }
        public int MessageVersion { get; set; }
        public string SecureToken { get; set; }
        public string SessionControl { get; set; } //enum
        public long SessionID { get; set; }
        public int SequenceNumber { get; set; }
    }
}
