using flights.crosscutting.Notifications.Email.Interfaces;
using flights.crosscutting.Notifications.Email.Models;
using System;
using System.Net;
using System.Net.Mail;

namespace flights.crosscutting.Notifications.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpConfiguration _config;

        public EmailService()
        {
            _config = new SmtpConfiguration();

            var gmailUserName = "lufemaa@lufemaa.com";
            var gmailPassword = "guxbib-gubPev-nanbu0";
            var gmailHost = "smtp.gmail.com";
            var gmailPort = 587;
            var gmailSsl = true;
            _config.Username = gmailUserName;
            _config.Password = gmailPassword;
            _config.Host = gmailHost;
            _config.Port = gmailPort;
            _config.Ssl = gmailSsl;
        }

        #region Send Email Message

        public bool SendEmailMessage(EmailMessage message)
        {
            var success = false;

            try
            {
                var smtp = new SmtpClient
                {
                    Host = _config.Host,
                    Port = _config.Port,
                    EnableSsl = _config.Ssl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_config.Username, _config.Password)
                };

                using var smtpMessage = new MailMessage(_config.Username, message.ToEmail)
                {
                    From = new MailAddress("lufemaa@lufemaa.com", "API de Aéreo"),
                    Subject = message.Subject,
                    Body = message.Body,
                    IsBodyHtml = message.IsHtml
                };
                smtp.Send(smtpMessage);

                success = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao enviar o e-mail: " + ex.Message);
            }

            return success;
        }

        #endregion
    }
}
