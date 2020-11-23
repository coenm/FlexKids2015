using System;
using Repository.Model;

namespace FlexKidsScheduler.Model
{
    public struct ScheduleDiff
    {
        public ScheduleStatus Status { get; set; }
        public DateTime Start 
        {
            get { return Schedule.StartDateTime; }
        }
        public Schedule Schedule { get; set; }
    }
}