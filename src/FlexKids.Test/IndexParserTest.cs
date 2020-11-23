using System.Collections.Generic;
using System.IO;
using FlexKidsScheduler.Model;
using NUnit.Framework;

namespace FlexKidsParser.Test
{
    public class IndexParserTest
    {
        private const string ResourceDirectory = "resources";

        [Test]
        public void IndexPageTest()
        {
            // arrange
            const string expectedEmail = "fake.login@github.com";
            const bool expectedIsLoggedin = true;
            var expectedWeeks = new Dictionary<int, WeekItem>
            {
                {0, new WeekItem(7,2015)}, 
                {1, new WeekItem(8,2015)}, 
                {2, new WeekItem(9,2015)}, 
            };

            // act
            var htmlContent = GetFileContent("index.html");
            var indexParser = new IndexParser(htmlContent);
            var indexContent = indexParser.Parse();

            // assert
            Assert.That(indexContent, Is.Not.Null);
            Assert.That(indexContent.Email, Is.EqualTo(expectedEmail));
            Assert.That(indexContent.IsLoggedin, Is.EqualTo(expectedIsLoggedin));
            Assert.That(indexContent.Weeks.Count, Is.EqualTo(expectedWeeks.Count));

            foreach (var item in indexContent.Weeks)
            {
                Assert.That(expectedWeeks.ContainsKey(item.Key), Is.True);
                Assert.That(expectedWeeks[item.Key].Year, Is.EqualTo(item.Value.Year));
                Assert.That(expectedWeeks[item.Key].WeekNr, Is.EqualTo(item.Value.WeekNr));
            }
        }

        private static string GetFileContent(string filename)
        {
            var file = Path.Combine(ResourceDirectory, filename);
            Assert.That(File.Exists(file), Is.True);
            return File.ReadAllText(file);
        }
    }
}