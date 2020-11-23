using System;

namespace FlexKidsScheduler
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public static readonly IDateTimeProvider Instance = new DateTimeProvider();

        private DateTimeProvider() { }

        public DateTime Today
        {
            get { return DateTime.Today; }
        }

        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}
