using System;
using System.Collections.Generic;
using FlexKidsScheduler.Model;
using NUnit.Framework;
using FakeItEasy;
using Repository;
using Repository.Model;

namespace FlexKidsScheduler.Test
{
    class SchedulerTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetChangesWithNoChangesReturnsEmptyListTest(bool isLoggedIn)
        {
            // arrange
            var flexKidsConnection = A.Fake<IFlexKidsConnection>();
            var parser = A.Fake<IKseParser>();
            A.CallTo(() => parser.GetIndexContent(A<string>._))
                .Returns(new IndexContent { Email = "a@b.nl", IsLoggedin = isLoggedIn, Weeks = new Dictionary<int, WeekItem>() });
            var scheduleRepository = A.Dummy<IScheduleRepository>();
            var hash = A.Dummy<IHash>();
            var sut = new Scheduler(flexKidsConnection, parser, scheduleRepository, hash);
            
            // act
            var result = sut.GetChanges();
            
            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            A.CallTo(() => flexKidsConnection.GetAvailableSchedulesPage()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => parser.GetIndexContent(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => flexKidsConnection.GetSchedulePage(A<int>._)).MustNotHaveHappened();
            A.CallTo(hash).MustNotHaveHappened();
            A.CallTo(scheduleRepository).MustNotHaveHappened();
        }


        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetChangesWithOneScheduleWichAlreadyExistsAndDidntChangeReturnsEmptyListTest(bool isLoggedIn)
        {
            // arrange
            var weeks = new Dictionary<int, WeekItem> {{0, new WeekItem(6, 2015)}};
            
            var flexKidsConnection = A.Fake<IFlexKidsConnection>();
            var parser = A.Fake<IKseParser>();
            var scheduleRepository = A.Dummy<IScheduleRepository>();
            var hash = A.Dummy<IHash>();
            var sut = new Scheduler(flexKidsConnection, parser, scheduleRepository, hash);

            A.CallTo(() => parser.GetIndexContent(A<string>._))
                .Returns(new IndexContent { Email = "a@b.nl", IsLoggedin = isLoggedIn, Weeks = weeks });
            A.CallTo(() => flexKidsConnection.GetSchedulePage(A<int>.That.IsEqualTo(0))).Returns("GetSchedulePage0");
            A.CallTo(() => hash.Hash(A<string>.That.IsEqualTo("GetSchedulePage0"))).Returns("hash0");
            A.CallTo(() => scheduleRepository.GetWeek(A<int>.That.IsEqualTo(2015), A<int>.That.IsEqualTo(6))).Returns(new Week { Hash = "hash0"});

            // act
            var result = sut.GetChanges();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            A.CallTo(() => flexKidsConnection.GetAvailableSchedulesPage()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => parser.GetIndexContent(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => flexKidsConnection.GetSchedulePage(A<int>.That.IsEqualTo(0))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => hash.Hash(A<string>.That.IsEqualTo("GetSchedulePage0"))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => scheduleRepository.GetWeek(A<int>.That.IsEqualTo(2015), A<int>.That.IsEqualTo(6))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetaChangesWithOneScheduleWichAlreadyExistsAndDidntChangeReturnsEmptyListTest(bool isLoggedIn)
        {
            // arrange
            var weeks = new Dictionary<int, WeekItem> { { 0, new WeekItem(6, 2015) } };
            var weekOld = new Week()
            {
                Hash = "hashOld",
                Id = 1,
                WeekNr = 6,
                Year = 2015
            };
            var weekNew = new Week()
            {
                Hash = "hashNew",
                Id = 1,
                WeekNr = 6,
                Year = 2015,
                
            };
            weekNew.Schedules = new List<Schedule>()
            {
                new Schedule 
                {
                    Id = 2, 
                    Location = "LocA", 
                    Week = weekNew, 
                    WeekId = weekNew.Id, 
                    StartDateTime = new DateTime(2015,01,02,12,13,14),     
                    EndDateTime= new DateTime(2015,01,02,17,13,14) 
                }
            }; 


            var flexKidsConnection = A.Fake<IFlexKidsConnection>();
            var parser = A.Fake<IKseParser>();
            var scheduleRepository = A.Dummy<IScheduleRepository>();
            var hash = A.Dummy<IHash>();
            var sut = new Scheduler(flexKidsConnection, parser, scheduleRepository, hash);

            A.CallTo(() => parser.GetIndexContent(A<string>._))
                .Returns(new IndexContent { Email = "a@b.nl", IsLoggedin = isLoggedIn, Weeks = weeks });
            A.CallTo(() => flexKidsConnection.GetSchedulePage(A<int>.That.IsEqualTo(0))).Returns("GetSchedulePage0");
            A.CallTo(() => hash.Hash(A<string>.That.IsEqualTo("GetSchedulePage0"))).Returns(weekNew.Hash);
            A.CallTo(() => scheduleRepository.GetWeek(A<int>.That.IsEqualTo(2015), A<int>.That.IsEqualTo(6))).Returns(weekOld);
            A.CallTo(() => scheduleRepository.Update(A<Week>._, A<Week>._)).Returns(weekNew);

            // act
            var result = sut.GetChanges();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            A.CallTo(() => flexKidsConnection.GetAvailableSchedulesPage()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => parser.GetIndexContent(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => flexKidsConnection.GetSchedulePage(A<int>.That.IsEqualTo(0))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => hash.Hash(A<string>.That.IsEqualTo("GetSchedulePage0"))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => scheduleRepository.GetWeek(A<int>.That.IsEqualTo(2015), A<int>.That.IsEqualTo(6))).MustHaveHappened(Repeated.Exactly.Once);

        }


    }
}
