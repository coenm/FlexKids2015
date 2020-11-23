﻿using System.Collections.Generic;
using FlexKidsScheduler;
using FlexKidsScheduler.Model;

namespace FlexKidsParser
{
    public class FlexKidsHtmlParser : IKseParser
    {
        public IndexContent GetIndexContent(string html)
        {
            var parser  = new IndexParser(html);
            return parser.Parse();
        }

        public List<ScheduleItem> GetScheduleFromContent(string html, int year)
        {
            var parser = new ScheduleParser(html, year);
            return parser.GetScheduleFromContent();
        }
    }
}