﻿
namespace flights.crosscutting.Notifications.Email.Models
{
    public class EmailMessage
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
    }
}
