using NUnit.Framework;

namespace FlexKids.Main.Test
{
    public class Sha1HashTest
    {
        [Test]
        public void TestSXy()
        {
            // arrange
            var sut = FlexKids.Main.Sha1Hash.Instance;
            
            // act
            var result = sut.Hash("this is a test");
            
            // assert
            Assert.That(result, Is.EqualTo("sksdjf"));
        }
    }
}
