using NUnit.Framework;

namespace Reporter.GoogleCalendar.Test
{
    public class DateTimeHelperTest
    {
        [Test]
        [TestCase(2015, 2, 2015, 1, 5)]
        [TestCase(2015,  1, 2014, 12, 29)]
        [TestCase(2014, 53, 2014, 12, 29)]
        [TestCase(2014, 52, 2014, 12, 22)]
        public void GetMondayForGivenWeekTest(int requestedYear, int requestedWeekNr, int resultYear, int resultMonth, int resultDay)
        {
            //arrange

            //act
            var result = DateTimeHelper.GetMondayForGivenWeek(requestedYear, requestedWeekNr);

            //assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Year, Is.EqualTo(resultYear));
            Assert.That(result.Month, Is.EqualTo(resultMonth));
            Assert.That(result.Day, Is.EqualTo(resultDay));
        }
    }
}
