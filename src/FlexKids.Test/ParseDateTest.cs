using System;
using FlexKidsParser.Helper;
using NUnit.Framework;
using sut = FlexKidsParser.Helper.ParseDate;

namespace FlexKidsParser.Test
{   
    public class ParseDateTest
    {
        [Test]
        [TestCase("09:00",9,0)]
        [TestCase("23:11",23,11)]
        [TestCase("00:59",0,59)]
        public void AddStringTimeToDateTest(string input, int hour, int min)
        {
            // arrange
            var d = new DateTime(214, 1, 2, 0, 0, 0);

            // act
            var result = sut.AddStringTimeToDate(d, input);

            // assert
            Assert.That(result.Year, Is.EqualTo(d.Year));
            Assert.That(result.Month, Is.EqualTo(d.Month));
            Assert.That(result.Day, Is.EqualTo(d.Day));

            Assert.That(result.Hour, Is.EqualTo(hour));
            Assert.That(result.Minute, Is.EqualTo(min));
        }

        [Test]
        [TestCase("1100")]
        public void AddStringTimeToDateFail1Test(string input)
        {
            // arrange
            var d = new DateTime(214, 1, 2, 0, 0, 0);

            // act
            // assert
            Assert.That(() => sut.AddStringTimeToDate(d, input), Throws.Exception.TypeOf<FormatException>());
        }  

        [Test]
        [TestCase("-1:00", "Hours not in range")]
        [TestCase("24:00", "Hours not in range")]
        [TestCase("24:00", "Hours not in range")]
        [TestCase("ab:00", "No hours found")]
        [TestCase("a1:00", "No hours found")]
        [TestCase("1a:00", "No hours found")]
        [TestCase(" :00", "No hours found")]
        [TestCase("1:-1", "Minutes not in range")]
        [TestCase("1:60", "Minutes not in range")]
        [TestCase("1:160", "Minutes not in range")]
        [TestCase("1:aa", "No minutes found")]
        [TestCase("1:a1", "No minutes found")]
        [TestCase("1:a1", "No minutes found")]
        [TestCase("1:", "No minutes found")]
        public void AddStringTimeToDateFail2Test(string input, string expectedMsg)
        {
            // arrange
            var d = new DateTime(214, 1, 2, 0, 0, 0);

            // act
            // assert
            Assert.That(() => sut.AddStringTimeToDate(d, input), Throws.Exception.With.Message.EqualTo(expectedMsg));
        }   

        
        [Test]
        [TestCase("09:00-13:30", 9, 0, 13,30)]
        [TestCase("14:00-17:30", 14, 0, 17,30)]
        public void CreateStartEndDateTimeTupleTest(string input, int startHour, int startMin, int endHour, int endMin)
        {
            // arrange
            var d = new DateTime(2014, 1, 2, 16, 17, 18);

            // act
            var result = sut.CreateStartEndDateTimeTuple(d, input);

            // assert
            Assert.That(result.Item1, Is.EqualTo(new DateTime(2014, 1, 2, startHour, startMin, 0)));
            Assert.That(result.Item2, Is.EqualTo(new DateTime(2014, 1, 2, endHour, endMin, 0)));
        }        
      

        [Test]
        [TestCase("din 3-feb.", 2015, 2, 3)]
        [TestCase("maa 8-dec.", 2014, 12, 8)]
        [TestCase("din 9-dec.", 2014, 12, 9)]
        [TestCase("woe 10-dec.", 2014, 12, 10)]
        [TestCase("don 11-dec.", 2014, 12, 11)]
        [TestCase("vri 12-dec.", 2014, 12, 12)]
        [TestCase("vri 6-mrt.", 2015, 3, 06)]
        [TestCase("woe 29-apr.", 2015, 4, 29 )]
        [TestCase("vri 12-mei.", 2015, 5, 12)]
        public void StringToDateTimeTest(string input, int year, int month, int day)
        {
            // arrange
            // act
            var result = sut.StringToDateTime(input, year);

            // assert
            Assert.That(result.Year, Is.EqualTo(year));
            Assert.That(result.Month, Is.EqualTo(month));
            Assert.That(result.Day, Is.EqualTo(day));
        }

        [Test]
        [TestCase("din 3 feb.")]
        [TestCase("din3feb.")]
        [TestCase("din-3-feb.")]
        [TestCase(" din 3-feb. ")]
        [TestCase("")]
        public void ParseDateFailure1Test(string input)
        {
            // arrange
            int year = 2015;

            // act
            // assert
            Assert.That(() => sut.StringToDateTime(input, year), Throws.Exception.TypeOf<FormatException>());
        }

     
        [Test]
        [TestCase("alpha.",   "alpha")]
        [TestCase("alpha",    "alpha")]
        [TestCase("alpha...", "alpha..")]
        [TestCase("alpha. ",  "alpha. ")]
        [TestCase("",         "")]
        public void TestRemoveLastCharIfDot(string test, string expectedResult)
        {
            //arrange
            
            //act
            var result = sut.RemoveLastCharIfDot(test);

            //assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
