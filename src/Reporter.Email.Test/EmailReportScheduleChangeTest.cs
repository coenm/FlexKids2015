using System;
using System.Collections.Generic;
using FakeItEasy;
using FlexKidsScheduler;
using FlexKidsScheduler.Model;
using NUnit.Framework;
using Reporter.Email;
using Repository.Model;

namespace FlexKids.Reporter.Email.Test
{
    class EmailReportScheduleChangeTest
    {
        private Week Week = new Week()
        {
            Id = 2,
            Hash = "sdfskdf83",
            Year = 2012,
            WeekNr = 23
        };

        private Schedule ScheduleA = new Schedule()
        {
            Id = 1,
            Location = "Jacob",
            StartDateTime = new DateTime(2012, 4, 8, 8, 5, 4),
            EndDateTime = new DateTime(2012, 4, 8, 17, 5, 4)
        };

        private Schedule ScheduleB = new Schedule()
        {
            Id = 3,
            Location = "New York",
            StartDateTime = new DateTime(2012, 1, 8, 10, 5, 4),
            EndDateTime = new DateTime(2012, 1, 8, 12, 5, 4)
        };

        private Schedule ScheduleC = new Schedule()
        {
            Id = 6,
            Location = "Madrid",
            StartDateTime = new DateTime(2012, 4, 8, 08, 30, 0),
            EndDateTime = new DateTime(2012, 4, 8, 22, 0, 0)
        };

        [SetUp]
        public void SetUp()
        {
            ScheduleA.Week = Week;
            ScheduleA.WeekId = Week.Id;

            ScheduleB.Week = Week;
            ScheduleB.WeekId = Week.Id;

            ScheduleC.Week = Week;
            ScheduleC.WeekId = Week.Id;
        }


        [Test]
        public void HandleChangeWithEmptyListTest()
        {
            // arrange
            var emailService = A.Fake<IEmailService>();
            var flexKidsConfig = A.Fake<IFlexKidsConfig>();
            var sut = new EmailReportScheduleChange(flexKidsConfig, emailService);

            // act
            var result = sut.HandleChange(null);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HandleChangeWithThreeItemsInListTest()
        {
            // arrange
            var emailService = A.Fake<IEmailService>();
            var flexKidsConfig = A.Fake<IFlexKidsConfig>();

            A.CallTo(() => flexKidsConfig.EmailTo2).Returns("een@hotmail.com");
            A.CallTo(() => flexKidsConfig.EmailTo1).Returns("twee@gmail.com");
            A.CallTo(() => flexKidsConfig.EmailFrom).Returns("from@gmail.com");

            var sut = new EmailReportScheduleChange(flexKidsConfig, emailService);

            var scheduleDiff = new List<ScheduleDiff>()
            {
                new ScheduleDiff 
                {
                    Schedule = ScheduleA,
                    Status = ScheduleStatus.Added
                },
                new ScheduleDiff 
                {
                    Schedule = ScheduleB,
                    Status = ScheduleStatus.Removed
                },
                new ScheduleDiff 
                {
                    Schedule = ScheduleC,
                    Status = ScheduleStatus.Unchanged
                }
            };

            // act
            var result = sut.HandleChange(scheduleDiff);

            // assert
            Assert.That(result, Is.True);
            A.CallTo(() => emailService.Send(A<System.Net.Mail.MailMessage>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}
