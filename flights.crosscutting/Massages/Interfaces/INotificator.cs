using flights.crosscutting.Messages.Models;
using System.Collections.Generic;

namespace flights.crosscutting.Messages.Interfaces
{
    public interface INotificator
    {

        public void Handle(Notification notification);

        public List<Notification> GetNotifications();

        public bool HasNotification();

        public void notify(string message);
    }
}
