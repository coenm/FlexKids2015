using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FakeItEasy;
using FlexKidsScheduler;

namespace Reporter.GoogleCalendar.Test
{
    class CalendarReportScheduleChangeTest
    {
        [Test]
        public void ConstructorTest()
        {
            // arrange
            var dateTimeProvider = A.Fake<IDateTimeProvider>();
            var flexKidsConfig = A.Fake<IFlexKidsConfig>();

            // act
            var sut = new CalendarReportScheduleChange(dateTimeProvider, flexKidsConfig);

            // assert
            Assert.That(sut, Is.Not.Null);
        }

        [Test]
        public void HandleChangeWithNullListShouldReturnTrueTest()
        {
            // arrange
            var dateTimeProvider = A.Fake<IDateTimeProvider>();
            var flexKidsConfig = A.Fake<IFlexKidsConfig>();
            var sut = new CalendarReportScheduleChange(dateTimeProvider, flexKidsConfig);

            // act
            var result = sut.HandleChange(null);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HandleChangeWithEmptyListShouldReturnTrueTest()
        {
            // arrange
            var dateTimeProvider = A.Fake<IDateTimeProvider>();
            var flexKidsConfig = A.Fake<IFlexKidsConfig>();
            var sut = new CalendarReportScheduleChange(dateTimeProvider, flexKidsConfig);
            var emptyList = new List<FlexKidsScheduler.Model.ScheduleDiff>();

            // act
            var result = sut.HandleChange(emptyList);

            // assert
            Assert.That(result, Is.True);
        }
    }
}
