using System;
using NUnit.Framework;

namespace FlexKidsScheduler.Test
{
    public class DateTimeProviderTest
    {
        [Test]
        public void NowTest()
        {
            // arrange
            var expected = DateTime.Now;

            // act
            var sut = DateTimeProvider.Instance;
            var result = sut.Now;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(Math.Abs(result.Ticks - expected.Ticks), Is.LessThan(3)); // not always ok.
        }


        [Test]
        public void TodayTest()
        {
            // arrange
            var expected = DateTime.Today;

            // assume
            Assume.That(DateTime.Now.Hour * 60 + DateTime.Now.Minute, Is.LessThan(1440-1)); //1440 is exact 00:00 hour which can makes this unittest fail

            // act
            var sut = DateTimeProvider.Instance;
            var result = sut.Today;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(expected, Is.EqualTo(result));
        }
    }
}
