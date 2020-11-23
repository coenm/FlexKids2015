using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlexKidsScheduler
{
    public interface IFlexKidsConfig
    {
        String EmailFrom { get; }
        String EmailTo2 { get; }
        String EmailToName2 { get; }
        String EmailTo1 { get; }
        String EmailToName1 { get; }

        String SmtpHost { get; }
        int SmtpPort { get; }
        String SmtpUsername { get; }
        String SmtpPassword { get; }

        String GoogleCalendarAccount { get; }
        String GoogleCalendarId { get; }
        String GoogleCalendarKeyFile { get; }
    }
}
