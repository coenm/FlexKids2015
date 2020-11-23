using System.Collections.Specialized;
using FakeItEasy;
using NUnit.Framework;

namespace FlexKidsConnection.Test
{
    public class FlexKidsCookieWebClientTest
    {
        private FlexKidsCookieConfig config;

        [SetUp]
        public void Setup()
        {
            config = new FlexKidsCookieConfig("https://abc.local/", "user", "pass");
        }

        [Test]
        public void GetSchedulePageTest()
        {
            // arrange
            var web = A.Fake<IWeb>();
            var sut = new FlexKidsCookieWebClient(web, config);

            // act
            sut.GetSchedulePage(1);

            // assert
            A.CallTo(() => web.PostValues(A<string>._, A<NameValueCollection>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => web.DownloadPageAsString(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void GetSchedulePageTwiceTest()
        {
            // arrange
            var web = A.Fake<IWeb>();
            var sut = new FlexKidsCookieWebClient(web, config);

            // act
            sut.GetSchedulePage(1);
            sut.GetSchedulePage(2);

            // assert
            A.CallTo(() => web.PostValues(A<string>._, A<NameValueCollection>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => web.DownloadPageAsString(A<string>._)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void GetAvailableSchedulesPageTest()
        {
            // arrange
            var response = "sdfdsf34IUHDSf834";
            var web = A.Fake<IWeb>();
            A.CallTo(() => web.DownloadPageAsString(A<string>._)).Returns(response);
            var sut = new FlexKidsCookieWebClient(web, config);

            // act
            var result = sut.GetAvailableSchedulesPage();

            // assert
            A.CallTo(() => web.PostValues(A<string>._, A<NameValueCollection>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => web.DownloadPageAsString(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            Assert.That(result, Is.EqualTo(response));
        }

        [Test]
        public void DisposeTest()
        {
            // arrange
            var web = A.Fake<IWeb>();
            var sut = new FlexKidsCookieWebClient(web, config);

            // act
            sut.Dispose();

            // assert
            A.CallTo(() => web.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
