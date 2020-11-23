using System;
using System.Collections.Generic;
using System.Linq;
using FlexKidsScheduler;
using NLog;

namespace Reporter.GoogleCalendar
{
    public class CalendarReportScheduleChange : IReportScheduleChange
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IFlexKidsConfig flexKidsConfig;

        public CalendarReportScheduleChange(IDateTimeProvider dateTimeProvider, IFlexKidsConfig flexKidsConfig)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.flexKidsConfig = flexKidsConfig;
        }

        public bool HandleChange(IList<FlexKidsScheduler.Model.ScheduleDiff> schedule)
        {
            if (schedule == null || !schedule.Any())
            {
                Logger.Trace("HandleChange Google calender, schedule == null | count = 0");
                return true;
            }

            try
            {
                Logger.Trace("Create Google Calendar");
                var google = new GoogleCalendarScheduler(dateTimeProvider, flexKidsConfig);
                Logger.Trace("Make events");
                google.MakeEvents(schedule);
            }
            catch(Exception ex)
            {
                Logger.Error("Something went wrong using Google Calendar.", ex);
                return false;
            }
            Logger.Trace("Done Google calendar");
            return true;
        }
    }
}
