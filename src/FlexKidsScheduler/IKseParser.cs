using System.Collections.Generic;
using FlexKidsScheduler.Model;

namespace FlexKidsScheduler
{
    public interface IKseParser
    {
        IndexContent GetIndexContent(string html);

        List<ScheduleItem> GetScheduleFromContent(string html, int year);
    }
}