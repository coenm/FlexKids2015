using System;
using System.IO;
using System.Linq;
using FlexKidsScheduler.Model;
using NUnit.Framework;

namespace FlexKidsParser.Test
{
    public class ScheduleParserTest
    {
        private const string ResourceDirectory = "resources";

        [Test]
        public void Schedule20141212Test()
        {
            // arrange
            const int year = 2014;

            var expectedItem0 = new ScheduleItem()
            {
                Start = new DateTime(year, 12, 08, 9, 0, 0),
                End = new DateTime(year, 12, 08, 13, 30, 0),
                Location = "Boventallig werken, mentor en inwerken"
            };

            var expectedItem1 = new ScheduleItem()
            {
                Start = new DateTime(year, 12, 08, 14, 0, 0),
                End = new DateTime(year, 12, 08, 17, 30, 0),
                Location = "Boventallig werken, mentor en inwerken"
            };

            var expectedItem2 = new ScheduleItem()
            {
                Start = new DateTime(year, 12, 11, 11, 0, 0),
                End = new DateTime(year, 12, 11, 12, 30, 0),
                Location = "Boventallig werken, mentor en inwerken"
            };

            // act
            var htmlContent = GetFileContent("20141212_rooster.txt");
            var contentParser = new ScheduleParser(htmlContent, year);
            var schedule = contentParser.GetScheduleFromContent();

            // assert
            Assert.That(schedule, Is.Not.Null);
            Assert.That(schedule.Count(), Is.EqualTo(3));
            AssertSchedule(schedule[0], expectedItem0);
            AssertSchedule(schedule[1], expectedItem1);
            AssertSchedule(schedule[2], expectedItem2);
        }

        [Test]
        public void Schedule20150209Test()
        {
            // arrange
            const int year = 2015;

            var expectedItem0 = new ScheduleItem()
            {
                Start = new DateTime(year, 02, 10, 8, 0, 0),
                End = new DateTime(year, 02, 10, 17, 30, 0),
                Location = "Flamingo's"
            };

            var expectedItem1 = new ScheduleItem()
            {
                Start = new DateTime(year, 02, 13, 9, 0, 0),
                End = new DateTime(year, 02, 13, 18, 30, 0),
                Location = "Panda's"
            };

            // act
            var htmlContent = GetFileContent("20150209_rooster.txt");
            var contentParser = new ScheduleParser(htmlContent, year);
            var schedule = contentParser.GetScheduleFromContent();

            // assert
            Assert.That(schedule, Is.Not.Null);
            Assert.That(schedule.Count(), Is.EqualTo(2));
            AssertSchedule(schedule[0], expectedItem0);
            AssertSchedule(schedule[1], expectedItem1);
        }

        [Test]
        public void Schedule201508Test()
        {
            // arrange
            const int year = 2015;

            var expectedItem0 = new ScheduleItem
            {
                Start = new DateTime(year, 02, 16, 8, 0, 0),
                End = new DateTime(year, 02, 16, 13, 15, 0),
                Location = "Zebra's"
            };

            var expectedItem1 = new ScheduleItem
            {
                Start = new DateTime(year, 02, 16, 13, 45, 0),
                End = new DateTime(year, 02, 16, 17, 30, 0),
                Location = "Krokodillen"
            };
           
            var expectedItem2 = new ScheduleItem
            {
                Start = new DateTime(year, 02, 17, 8, 0, 0),
                End = new DateTime(year, 02, 17, 17, 30, 0),
                Location = "Flamingo's"
            };

            var expectedItem3 = new ScheduleItem
            {
                Start = new DateTime(year, 02, 20, 9, 0, 0),
                End = new DateTime(year, 02, 20, 18, 30, 0),
                Location = "Panda's"
            };

            // act
            var htmlContent = GetFileContent("2015-08.html");
            var contentParser = new ScheduleParser(htmlContent, year);
            var schedule = contentParser.GetScheduleFromContent();

            // assert
            Assert.That(schedule, Is.Not.Null);
            Assert.That(schedule.Count(), Is.EqualTo(4));
            AssertSchedule(schedule[0], expectedItem0);
            AssertSchedule(schedule[1], expectedItem1);
            AssertSchedule(schedule[2], expectedItem2);
            AssertSchedule(schedule[3], expectedItem3);
        }

        private static void AssertSchedule(ScheduleItem itemA, ScheduleItem itemB)
        {
            Assert.That(itemA.End, Is.EqualTo(itemB.End));
            Assert.That(itemA.Start, Is.EqualTo(itemB.Start));
            Assert.That(itemA.Location, Is.EqualTo(itemB.Location));
        }

        private static string GetFileContent(string filename)
        {
            var file = Path.Combine(ResourceDirectory, filename);
            Assert.That(File.Exists(file), Is.True);
            return File.ReadAllText(file);
        }
    }
}