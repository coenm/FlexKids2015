using Repository.Model;

namespace FlexKidsScheduler.Model
{
    public class WeekAndHtml
    {
        public Week Week { get; set; }
        public string Html { get; set; }
        public string Hash { get; set; }
        public bool ScheduleChanged { get; set; }
    }
}