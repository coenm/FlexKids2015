using System;
using System.Net.Mail;

namespace FlexKidsScheduler
{
    public interface IEmailService
    {
        void Send(MailMessage message);
    }
}
