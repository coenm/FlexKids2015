using System;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using NLog;
using System.Configuration;

namespace FlexKidsScheduler
{
    public class EmailService : IEmailService
    {
        SmtpClient client = null;

        public EmailService(IFlexKidsConfig flexKidsConfig)
        {
            client = new SmtpClient
            {
                Port = flexKidsConfig.SmtpPort,
                Host = flexKidsConfig.SmtpHost,
                EnableSsl = false,
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(flexKidsConfig.SmtpUsername, flexKidsConfig.SmtpPassword)
            };
        }

        public void Send(MailMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            client.Send(message); // TODO exception handling
        }
    }
}
