using System.Collections.Generic;

namespace FlexKidsScheduler.Model
{
    public class IndexContent
    {
        public bool IsLoggedin { get; set; }
        public string Email { get; set; }
        public Dictionary<int, WeekItem> Weeks { get; set; }
    }
}