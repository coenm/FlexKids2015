using System;
using System.Collections.Generic;
using System.Linq;
using FlexKidsScheduler;
using NLog;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Reporter.Email
{
    public class EmailReportScheduleChange : IReportScheduleChange
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFlexKidsConfig flexKidsConfig;
        private readonly IEmailService emailService;

        public EmailReportScheduleChange(IFlexKidsConfig flexKidsConfig, IEmailService emailService)
        {
            if (flexKidsConfig == null)
                throw new ArgumentNullException("flexKidsConfig");
            if (emailService == null)
                throw new ArgumentNullException("emailService");

            this.flexKidsConfig = flexKidsConfig;
            this.emailService = emailService;
        }

        public bool HandleChange(IList<FlexKidsScheduler.Model.ScheduleDiff> schedule)
        {
            if (schedule == null || !schedule.Any())
                return true;

            try
            {
                var orderedSchedule = schedule.OrderBy(x => x.Start).ThenBy(x => x.Status).ToArray();
                
                var subject = "Werkrooster voor week " + orderedSchedule[0].Schedule.Week.WeekNr;
                var schedulePlain = EmailContentBuilder.ScheduleToPlainTextString(orderedSchedule);
                var scheduleHtml = EmailContentBuilder.ScheduleToHtmlString(orderedSchedule);
                
                var mm = CreateMailMessage(subject, schedulePlain, scheduleHtml);
                emailService.Send(mm);
            }
            catch (Exception ex)
            {
                Logger.Error("Something went wrong sending an email with the schedule.", ex);
                return false;
            }
            return true;
        }

        private MailMessage CreateMailMessage(string subject, string schedulePlain, string scheduleHtml)
        {
            var from = new MailAddress(flexKidsConfig.EmailFrom, "FlexKids rooster");
            var toEmail1 = new MailAddress(flexKidsConfig.EmailTo1, flexKidsConfig.EmailToName1);
            var toEmail2 = new MailAddress(flexKidsConfig.EmailTo2, flexKidsConfig.EmailToName2);
            var mm = new MailMessage(from, toEmail1)
            {
                Subject = subject,
                Body = schedulePlain,
                BodyEncoding = Encoding.UTF8,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };
            mm.To.Add(toEmail2);

            var mimeType = new ContentType("text/html");
            var alternate = AlternateView.CreateAlternateViewFromString(scheduleHtml, mimeType);
            mm.AlternateViews.Add(alternate);

            return mm;
        }
    }
}