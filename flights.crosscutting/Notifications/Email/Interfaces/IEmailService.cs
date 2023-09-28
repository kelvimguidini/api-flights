using flights.crosscutting.Notifications.Email.Models;

namespace flights.crosscutting.Notifications.Email.Interfaces
{
    public interface IEmailService
    {
        bool SendEmailMessage(EmailMessage message);
    }
}
