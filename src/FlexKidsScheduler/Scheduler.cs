using System;
using System.Collections.Generic;
using System.Linq;
using FlexKidsScheduler.Model;
using Repository;
using Repository.Model;

namespace FlexKidsScheduler
{
    // A delegate type for hooking up change notifications.
    public delegate void ChangedEventHandler(object sender, ScheduleChangedArgs e);

    public class Scheduler : IDisposable
    {
        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event ChangedEventHandler ScheduleChanged;

        private readonly IFlexKidsConnection flexKidsConnection;
        private readonly IHash hash;
        private readonly IKseParser parser;
        private readonly IScheduleRepository repo;

        public Scheduler(IFlexKidsConnection flexKidsConnection, IKseParser parser, IScheduleRepository scheduleRepository, IHash hash)
        {
            this.flexKidsConnection = flexKidsConnection;
            this.hash = hash;
            this.parser = parser;
            repo = scheduleRepository;
        }
        
        private static IList<ScheduleDiff> GetDiffs(ICollection<Schedule> dbSchedules, ICollection<ScheduleItem> parsedSchedules, Week week)
        {
            var diffResult = new List<ScheduleDiff>(parsedSchedules.Count + dbSchedules.Count);

            foreach (var item in dbSchedules)
            {
                var diffResultItem = new ScheduleDiff
                {
                    Schedule = item,
                };

                var selectItem = parsedSchedules
                    .FirstOrDefault(x =>
                        x.Start == item.StartDateTime && x.End == item.EndDateTime && x.Location == item.Location);

                if (selectItem != null)
                {
                    diffResultItem.Status = ScheduleStatus.Unchanged;
                    parsedSchedules.Remove(selectItem);
                }
                else
                    diffResultItem.Status = ScheduleStatus.Removed;
               
                diffResult.Add(diffResultItem);
            }


            foreach (var parsedSchedule in parsedSchedules)
            {
                var schedule = new Schedule
                {
                    WeekId = week.Id,
                    Week = week,
                    Location = parsedSchedule.Location,
                    StartDateTime = parsedSchedule.Start,
                    EndDateTime = parsedSchedule.End
                };

                diffResult.Add(new ScheduleDiff
                {
                    Schedule = schedule,
                    Status = ScheduleStatus.Added
                });
            }

            return diffResult;
        }
        
        private Week GetCreateOrUpdateWeek(Week week, int year, int weekNr, string htmlHash)
        {
            if (week == null)
            {
                week = repo.Insert(new Week { Hash = htmlHash, Year = year, WeekNr = weekNr });
                if (week == null)
                    throw new Exception();
            }
            else
            {
                if (week.Hash == htmlHash)
                    return week;

                var newWeek = new Week
                {
                    Hash = htmlHash,
                    WeekNr = week.WeekNr,
                    Year = week.Year,
                    Id = week.Id
                };

                // week.Hash = htmlHash;
                var w = repo.Update(week, newWeek);
                if (w == null)
                    throw new Exception();

                return w;
            }

            return week;
        }

        public IEnumerable<ScheduleDiff> GetChanges()
        {
            var rooterFirstPage = flexKidsConnection.GetAvailableSchedulesPage();
            var indexContent = parser.GetIndexContent(rooterFirstPage);
            var somethingChanged = false;
            var weekAndHtml = new Dictionary<int, WeekAndHtml>(indexContent.Weeks.Count);

            foreach (var i in indexContent.Weeks)
            {
                var htmlSchedule = flexKidsConnection.GetSchedulePage(i.Key);
                var htmlHash = hash.Hash(htmlSchedule);
                var week = repo.GetWeek(i.Value.Year, i.Value.WeekNr);

                if (week == null || htmlHash != week.Hash)
                    somethingChanged = true;

                weekAndHtml.Add(i.Key, new WeekAndHtml
                {
                    Week = GetCreateOrUpdateWeek(week, i.Value.Year, i.Value.WeekNr, htmlHash),
                    Hash = htmlHash,
                    Html = htmlSchedule,
                    ScheduleChanged = (week == null || htmlHash != week.Hash)
                });
            }

            if(somethingChanged == false)
                return Enumerable.Empty<ScheduleDiff>();

            var diffsResult = new List<ScheduleDiff>();

            foreach (var item in weekAndHtml.Select(a => a.Value))
            {
                var dbSchedules = repo.GetSchedules(item.Week.Year, item.Week.WeekNr);
                IList<ScheduleDiff> diffResult;
                if (item.ScheduleChanged)
                {
                    var parsedSchedules = parser.GetScheduleFromContent(item.Html, item.Week.Year);
                    diffResult = GetDiffs(dbSchedules, parsedSchedules, item.Week);

                    var schedulesToDelete = diffResult
                        .Where(x => x.Status == ScheduleStatus.Removed)
                        .Select(x => x.Schedule);
                    repo.Delete(schedulesToDelete);

                    var schedulesToInsert = diffResult
                        .Where(x => x.Status == ScheduleStatus.Added)
                        .Select(x => x.Schedule);
                    foreach (var schedule in schedulesToInsert)
                    {
                        repo.Insert(schedule);
                    }

                    OnScheduleChanged(diffResult.OrderBy(x => x.Start).ThenBy(x => x.Status));
                }
                else
                {
                    diffResult = new List<ScheduleDiff>(dbSchedules.Count);
                    foreach (var dbSchedule in dbSchedules)
                    {
                        diffResult.Add(new ScheduleDiff
                        {
                            Schedule = dbSchedule,
                            Status = ScheduleStatus.Unchanged
                        });
                    }
                }
                diffsResult.AddRange(diffResult);
            }

            return diffsResult;
        }

        public void Dispose()
        {
            // should we dispose injected instances?
        }

        protected virtual void OnScheduleChanged(IOrderedEnumerable<ScheduleDiff> diffs)
        {
            var handler = ScheduleChanged;
            if(handler == null)
                return;

            ScheduleChangedArgs e = new ScheduleChangedArgs(diffs);
                handler(this, e);
        }
    }

    public class ScheduleChangedArgs : EventArgs
    {
        private readonly IOrderedEnumerable<ScheduleDiff> diff;

        public ScheduleChangedArgs(IOrderedEnumerable<ScheduleDiff> diff)
        {
            this.diff = diff;
        }

        public IList<ScheduleDiff> Diff
        {
            get { return diff.ToList(); }
        }
    }
}