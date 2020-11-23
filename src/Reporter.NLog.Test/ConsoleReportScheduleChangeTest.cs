using System;
using System.Collections.Generic;
using FlexKidsScheduler.Model;
using NUnit.Framework;
using Reporter.Nlog;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace Reporter.NLog.Test
{
    public class ConsoleReportScheduleChangeTest
    {
        private MemoryTarget logger;

        [SetUp]
        public void SetUp()
        {
            InitializeLogger(LogLevel.Info);
        }

        [TearDown]
        public void TearDown()
        {
            if (logger != null)
                logger.Dispose();
        }

        [Test]
        public void ConstructorTest()
        {
            // arrange
            // act
            var sut = new ConsoleReportScheduleChange();

            // assert
            Assert.That(sut, Is.Not.Null);
        }

        [Test]
        public void HandleChangeWithNullListShouldThrowExceptionTest()
        {
            // arrange
            var sut = new ConsoleReportScheduleChange();

            // act
            // assert
            Assert.That(() => sut.HandleChange(null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void HandleChangeWithEmptyListShouldReturnTrueTest()
        {
            // arrange
            var sut = new ConsoleReportScheduleChange();
            var list = new List<ScheduleDiff>();

            // act
            var result = sut.HandleChange(list);
            
            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HandleChangeWithInfoLoggingDisabledShouldReturnFalseTest()
        {
            // arrange
            InitializeLogger(LogLevel.Fatal); // info is disabled
            var sut = new ConsoleReportScheduleChange();
            var list = new List<ScheduleDiff>();

            // act
            var result = sut.HandleChange(list);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void HandleChangeWithNonemptyListShouldReturnTrueAndWriteLoggingTest()
        {
            // arrange
            var sut = new ConsoleReportScheduleChange();
            
            var week = new Repository.Model.Week()
            {
                Id = 1,
                Hash = "hash 1",
                Year = 2015,
                WeekNr = 12
            };

            var list = new List<ScheduleDiff>()
            {
                new ScheduleDiff() 
                {
                    Status = ScheduleStatus.Added,
                    Schedule = new Repository.Model.Schedule() 
                    {
                        Id = 1, WeekId = week.Id, Week = week,
                        StartDateTime = new DateTime(2015,2,22, 10,00,00),
                        EndDateTime = new DateTime(2015,2,22, 15,30,00),
                        Location = "The Club"
                    },
                },
                new ScheduleDiff() 
                {
                    Status = ScheduleStatus.Removed,
                    Schedule = new Repository.Model.Schedule() 
                    {
                        Id = 1, WeekId = week.Id, Week = week,
                        StartDateTime = new DateTime(2015,2,22, 9,00,00),
                        EndDateTime = new DateTime(2015,2,22, 17,30,00),
                        Location = "Bongo beach"
                    },
                },
                new ScheduleDiff() 
                {
                    Status = ScheduleStatus.Unchanged,
                    Schedule = new Repository.Model.Schedule() 
                    {
                        Id = 1, WeekId = week.Id, Week = week,
                        StartDateTime = new DateTime(2015,2,20, 9,00,00),
                        EndDateTime = new DateTime(2015,2,20, 17,30,00),
                        Location = "The Plazza"
                    },
                }
            };

            // act
            var result = sut.HandleChange(list);

            // assert
            Assert.That(result, Is.True);
            Assert.That(logger.Logs, Is.Not.Null);
            Assert.That(logger.Logs.Count, Is.EqualTo(3));
            Assert.That(logger.Logs[0], Is.EqualTo("= 20-02 09:00-17:30 The Plazza"));
            Assert.That(logger.Logs[1], Is.EqualTo("- 22-02 09:00-17:30 Bongo beach"));
            Assert.That(logger.Logs[2], Is.EqualTo("+ 22-02 10:00-15:30 The Club"));
        }

        private void InitializeLogger(LogLevel minLevel)
        {
            if (logger != null)
                logger.Dispose();

            logger = new MemoryTarget { Layout = "${message}" };
            SimpleConfigurator.ConfigureForTargetLogging(logger, minLevel);
        }
    }
}
