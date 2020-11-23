using System.Collections.Generic;
using FlexKidsScheduler.Model;

namespace FlexKidsScheduler
{
    public interface IReportScheduleChange
    {
        bool HandleChange(IList<ScheduleDiff> schedule);
    }
}
