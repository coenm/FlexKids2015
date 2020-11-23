using System;
using System.Collections.Generic;
using System.Linq;
using FlexKidsScheduler;
using FlexKidsScheduler.Model;
using NLog;

namespace Reporter.Nlog
{
    public class ConsoleReportScheduleChange : IReportScheduleChange
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ConsoleReportScheduleChange()
        {
        }

        public bool HandleChange(IList<ScheduleDiff> schedule)
        {
            if(schedule == null)
            {
                Logger.Error("Schedule cannot be null");
                throw new ArgumentNullException("schedule");
            }

            if (! Logger.IsInfoEnabled)
                return false;

            foreach (var item in schedule.OrderBy(x => x.Start).ThenBy(x => x.Status))
            {
                var s = ScheduleItemToString(item);
                Logger.Info(s);
            }
            return true;
        }

        private static string ScheduleItemToString(ScheduleDiff item)
        {
            var s = ScheduleStatusToString(item);
            s += " ";
            s += item.Schedule.StartDateTime.ToString("dd-MM HH:mm");
            s += "-";
            s += item.Schedule.EndDateTime.ToString("HH:mm");
            s += " ";
            s += item.Schedule.Location;
            return s;
        }

        private static string ScheduleStatusToString(ScheduleDiff item)
        {
            switch(item.Status)
            {
                case ScheduleStatus.Added:
                    return "+";
                case ScheduleStatus.Removed:
                    return "-";
                case ScheduleStatus.Unchanged:
                    return "=";
                default:
                    return String.Empty;
            }
        }
    }
}