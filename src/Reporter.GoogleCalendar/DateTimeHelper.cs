using System;
using System.Globalization;

namespace Reporter.GoogleCalendar
{
    static class DateTimeHelper
    {
        public static DateTime GetMondayForGivenWeek(int year, int weeknr)
        {
            //http://stackoverflow.com/questions/662379/calculate-date-from-week-number

            // assume 1 januari of year xx is week 1
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            var firstThursday = jan1.AddDays(daysOffset);

            var cal = CultureInfo.CurrentCulture.Calendar;
            var firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weeknr;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            var mondayOfRequestedWeek = result.AddDays(-3);

            return mondayOfRequestedWeek;
        }
    }
}