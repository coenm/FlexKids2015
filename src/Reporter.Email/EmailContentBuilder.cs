using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlexKidsScheduler.Model;
using System.Globalization;

namespace Reporter.Email
{
    public static class EmailContentBuilder //TODO Should be internal
    {
        public static String ScheduleToPlainTextString(ScheduleDiff[] schedule)
        {
            if (schedule == null || schedule.Length == 0)
                return String.Empty;

            var sb = new StringBuilder();
            foreach (var item in schedule)
            {
                sb.Append(StatusToString(item));
                sb.Append(" ");
                sb.Append(item.Schedule.StartDateTime.ToString("dd-MM HH:mm"));
                sb.Append("-");
                sb.Append(item.Schedule.EndDateTime.ToString("HH:mm"));
                sb.Append(" ");
                sb.Append(item.Schedule.Location);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        public static String ScheduleToHtmlString(ScheduleDiff[] schedule)
        {
            if (schedule == null || schedule.Length == 0)
                return String.Empty;

            var sb = new StringBuilder();
            sb.AppendLine(String.Format("<p>Hier is je rooster voor week {0}:</p>", schedule.First().Schedule.Week.WeekNr));

            sb.AppendLine("<table style='border: 1px solid black; border-collapse:collapse;'>");

            //header
            sb.AppendLine(String.Format("<tr style='{0}'>", StyleString("left")));
            sb.AppendLine(String.Format("<td style='{0}'></td>", StyleString("center")));
            sb.AppendLine(String.Format("<td colspan=2 style='{0}'><b>Dag</b></td>", StyleString("left")));
            sb.AppendLine(String.Format("<td colspan=3 style='{0}'><b>Tijd</b></td>", StyleString("left")));
            sb.AppendLine(String.Format("<td style='{0}'><b>Locatie</b></td>", StyleString("left")));
            sb.AppendLine("</tr>");


            foreach (var item in schedule)
            {
                sb.AppendLine(String.Format("<tr style='{0}'>", StyleString("left")));
                sb.AppendLine(String.Format("<td style='{0}'>{1}</td>", StyleString("center"), StatusToString(item)));
                sb.AppendLine(String.Format("<td style='{0}{1} border-right:hidden;'>{2}</td>", StyleString("left"), Linethrough(item.Status), item.Schedule.StartDateTime.ToString("ddd", CultureInfo.CreateSpecificCulture("nl-NL"))));
                sb.AppendLine(String.Format("<td style='{0}{1}'>{2}</td>", StyleString("left"), Linethrough(item.Status), item.Schedule.StartDateTime.ToString("dd-MM")));
                sb.AppendLine(String.Format("<td style='{0}{1} text-align: right; padding-right:0px;'>{2}</td>", StyleString("left"), Linethrough(item.Status), item.Schedule.StartDateTime.ToString("HH:mm")));
                sb.AppendLine(String.Format("<td style='{0} border-left: hidden; border-right: hidden;'>-</td>", StyleString("center")));
                sb.AppendLine(String.Format("<td style='{0}{1} padding-left:0px;'>{2}</td>", StyleString("left"), Linethrough(item.Status), item.Schedule.EndDateTime.ToString("HH:mm")));
                sb.AppendLine(String.Format("<td style='{0}{1}'>{2}</td>", StyleString("left"), Linethrough(item.Status), item.Schedule.Location));
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine("</p>");

            return sb.ToString();
        }


        private static string StyleString(string textAlign)
        {
            return String.Format("text-align:{0}; padding:0px 5px; border: 1px solid black;", textAlign);
        }

        private static string Linethrough(ScheduleStatus status)
        {
            if (status == ScheduleStatus.Removed)
                return "text-decoration: line-through;";
            return String.Empty;
        }
        
        private static string StatusToString(ScheduleDiff item)
        {
            switch (item.Status)
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
