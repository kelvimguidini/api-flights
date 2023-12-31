﻿using flights.crosscutting.Messages.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace flights.crosscutting.Messages.Models
{
    public class Notificator : INotificator
    {
        private List<Notification> _notifications;

        public Notificator()
        {
            _notifications = new List<Notification>();
        }

        public void Handle(Notification notification)
        {
            _notifications.Add(notification);
        }

        public List<Notification> GetNotifications()
        {
            return _notifications;
        }

        public bool HasNotification()
        {
            return _notifications.Any();
        }

        public void notify(string message)
        {
            Handle(new Notification(message));
        }
    }
}
