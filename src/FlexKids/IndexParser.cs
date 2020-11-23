using System;
using System.Collections.Generic;
using System.Linq;
using FlexKidsParser.Helper;
using FlexKidsScheduler.Model;
using HtmlAgilityPack;
using NLog;

namespace FlexKidsParser
{
    internal class IndexParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly HtmlDocument document;

        public IndexParser(string content)
        {
            document = new HtmlDocument();
            document.LoadHtml(content);
        }

        public IndexContent Parse()
        {
            var  result = new IndexContent();
            result.Email = ExtractEmailFromContent();
            result.IsLoggedin = (result.Email != "");
            result.Weeks = ExtractWeeksFromContent();
            return result;
        }

        private Dictionary<int, WeekItem> ExtractWeeksFromContent()
        {
            var weekselections = document.DocumentNode.Descendants()
                .Where(x => (x.IsSelect() && x.IdEquals("week_selectie")))
                .ToList();
            if (weekselections.Count != 1)
            {
                var s = String.Format("Nr of weekselections is {0} but should be equal to 1.", weekselections.Count());
                Logger.Error(s);
                throw new ApplicationException(s);
            }
            var weekselection = weekselections.First();

            // select options
            var options = weekselection.ChildNodes.Where(x => x.IsOption()).ToList();

            var weeks = new Dictionary<int, WeekItem>();
            if (options.Any())
            {
                foreach (var option in options)
                {
                    if (option.Attributes == null || option.Attributes["value"] == null)
                    {
                        throw new Exception();
                    }

                    int nr;
                    if (!Int32.TryParse(option.Attributes["value"].Value, out nr))
                    {
                        throw new Exception();
                    }

                    if (option.NextSibling == null)
                    {
                        throw new Exception();
                    }

                    //Week 09 - 2015
                    var weekText = option.NextSibling.InnerText.Trim();
                    weekText = weekText.Replace("Week", "").Trim();
                    var split = weekText.Split('-');
                    if(split.Count() == 2)
                    {
                        string sWeek = split[0].Trim(); // 09
                        string sYear = split[1].Trim(); // 2015
                        int weekNr, year;

                        if(Int32.TryParse(sWeek, out weekNr) && Int32.TryParse(sYear, out year))
                        {
                            var w = new WeekItem(weekNr, year);
                            weeks.Add(nr, w);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }
            return weeks;
        }

        private string ExtractEmailFromContent()
        {
            var logins = document.DocumentNode.Descendants().Where(x => (x.IsDiv() && x.ClassContains("login"))).ToList();
            if (logins.Count != 1)
            {
                throw new Exception();
            }
            var login = logins.First();
            if (!login.HasChildNodes || login.ChildNodes.Count != 2)
            {
                throw new Exception();
            }

            // first is text we are interested in
            var text = login.ChildNodes.First();
            if (!text.IsJustText())
            {
                throw new Exception();
            }
            var email = text.InnerText.Replace("&nbsp;", "").Trim();
            return email;
        }
    }
}
