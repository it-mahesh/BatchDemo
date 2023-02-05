using BatchDemo.Models;

namespace BatchDemo.Models.UnitTests
{
    public class BatchTests
    {
        private Batch _batch;
        [SetUp]
        public void SetUp()
        {
            _batch = new Batch();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}