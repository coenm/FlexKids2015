using System;
using FlexKidsScheduler.Model;
using NUnit.Framework;
using Repository.Model;
using sut = Reporter.Email.EmailContentBuilder;


namespace FlexKids.Reporter.Email.Test
{
    public class EmailContentBuilderTest
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
        public void ScheduleToPlainTextStringWithEmptyListReturnsEmptyStringTest()
        {
            // arrange
            var scheduleDiff = new ScheduleDiff[] { };

            // act
            var result = sut.ScheduleToPlainTextString(scheduleDiff);

            // assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ScheduleToPlainTextStringWithThreeItemsInListReturnsFormattedStringTest()
        {
            // arrange
            var scheduleDiff = new ScheduleDiff[] 
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
            var result = sut.ScheduleToPlainTextString(scheduleDiff);

            // assert
            string expected = "";
            expected += "+ 08-04 08:05-17:05 Jacob" + Environment.NewLine;
            expected += "- 08-01 10:05-12:05 New York" + Environment.NewLine;
            expected += "= 08-04 08:30-22:00 Madrid" + Environment.NewLine;
            Assert.That(result, Is.EqualTo(expected));
        }
        
        [Test]
        public void ScheduleToHtmlStringWithEmptyListReturnsEmptyStringTest()
        {
            // arrange
            var scheduleDiff = new ScheduleDiff[] { };

            // act
            var result = sut.ScheduleToHtmlString(scheduleDiff);

            // assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ScheduleToHtmlStringWithThreeItemsInListReturnsFormattedStringTest()
        {
            // arrange
            var scheduleDiff = new ScheduleDiff[] 
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
            var result = sut.ScheduleToHtmlString(scheduleDiff);

            // assert
            var expected = @"
<p>Hier is je rooster voor week 23:</p>
<table style='border: 1px solid black; border-collapse:collapse;'>
<tr style='text-align:left; padding:0px 5px; border: 1px solid black;'>
<td style='text-align:center; padding:0px 5px; border: 1px solid black;'></td>
<td colspan=2 style='text-align:left; padding:0px 5px; border: 1px solid black;'><b>Dag</b></td>
<td colspan=3 style='text-align:left; padding:0px 5px; border: 1px solid black;'><b>Tijd</b></td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black;'><b>Locatie</b></td>
</tr>
<tr style='text-align:left; padding:0px 5px; border: 1px solid black;'>
<td style='text-align:center; padding:0px 5px; border: 1px solid black;'>+</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black; border-right:hidden;'>zo</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black;'>08-04</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black; text-align: right; padding-right:0px;'>08:05</td>
<td style='text-align:center; padding:0px 5px; border: 1px solid black; border-left: hidden; border-right: hidden;'>-</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black; padding-left:0px;'>17:05</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black;'>Jacob</td>
</tr>
<tr style='text-align:left; padding:0px 5px; border: 1px solid black;'>
<td style='text-align:center; padding:0px 5px; border: 1px solid black;'>-</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black;text-decoration: line-through; border-right:hidden;'>zo</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black;text-decoration: line-through;'>08-01</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black;text-decoration: line-through; text-align: right; padding-right:0px;'>10:05</td>
<td style='text-align:center; padding:0px 5px; border: 1px solid black; border-left: hidden; border-right: hidden;'>-</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black;text-decoration: line-through; padding-left:0px;'>12:05</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black;text-decoration: line-through;'>New York</td>
</tr>
<tr style='text-align:left; padding:0px 5px; border: 1px solid black;'>
<td style='text-align:center; padding:0px 5px; border: 1px solid black;'>=</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black; border-right:hidden;'>zo</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black;'>08-04</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black; text-align: right; padding-right:0px;'>08:30</td>
<td style='text-align:center; padding:0px 5px; border: 1px solid black; border-left: hidden; border-right: hidden;'>-</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black; padding-left:0px;'>22:00</td>
<td style='text-align:left; padding:0px 5px; border: 1px solid black;'>Madrid</td>
</tr>
</table>
</p>";
            Assert.That(result.Trim(), Is.EqualTo(expected.Trim()));
        }
    }
}
