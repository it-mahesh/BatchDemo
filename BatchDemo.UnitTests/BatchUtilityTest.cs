using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using BatchDemo.Services;
using BatchDemo.Services.Interface;
using FakeItEasy;
using System.Linq.Expressions;

namespace BatchDemo.UnitTests
{
    [TestFixture]
    [Category("Batch")]
    public class BatchUtilityTest
    {
        private IUnitOfWork _unitOfWork;
        private IBatchUtility _batchUtility;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _batchUtility = A.Fake<IBatchUtility>();
        }

        /*
        [Test]
        public void GetJsonByBatchId_ReturnData()
        {
            BatchUtility batchUtility = new BatchUtility(_unitOfWork);

            _batchUtility = A.Fake<IBatchUtility>();
            Batch batch = new Batch();
            batch = DemoBatchData.LoadBatch();
            
            BatchDemo.Models.JsonDocument jsonDocument = new BatchDemo.Models.JsonDocument() { BatchId = batch.BatchId, Document = null };
            A.CallTo(() => _batchUtility.GetJsonByBatchId(A<Guid>.Ignored)).Returns(jsonDocument);
            var jsonResult = _batchUtility.GetJsonByBatchId(batch?.BatchId);
            //var jsonResult2= batchUtility.GetJsonByBatchId(batch?.BatchId);
            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult.BatchId, Is.EqualTo(batch?.BatchId));
        }
        */
        [Test]
        public void BatchToBatchInfoConverter_ReturnData()
        {
            BatchUtility batchUtility = new(_unitOfWork);

            _batchUtility = A.Fake<IBatchUtility>();
            Batch batch = new();
            batch = DemoBatchData.LoadBatch();

            BatchInfo batchInfo = new (){ BatchId=batch.BatchId,BusinessUnit=batch.BusinessUnit };
            A.CallTo(() => _batchUtility.BatchToBatchInfoConverter(A<Batch>.Ignored)).Returns(batchInfo);
            var jsonResult = _batchUtility.BatchToBatchInfoConverter(batch);
            var jsonResult2 = batchUtility.BatchToBatchInfoConverter(batch);
            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult.BatchId, Is.EqualTo(batch.BatchId));
        }
        [Test]
        public void DeserializeJsonDocument_BatchIdFound()
        {
            BatchUtility batchUtility = new(_unitOfWork);

            _batchUtility = A.Fake<IBatchUtility>();
            Batch batch = new();
            batch = DemoBatchData.LoadBatch();

            A.CallTo(() => _batchUtility.DeserializeJsonDocument(A<Guid>.Ignored)).Returns(batch);
            var jsonResult = _batchUtility.DeserializeJsonDocument(batch.BatchId);
            var jsonResult2 = batchUtility.DeserializeJsonDocument(batch.BatchId);
            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult.BatchId, Is.EqualTo(batch.BatchId));
        }
        [Test]
        public void DeserializeJsonDocument_BatchIdNotFound()
        {
            BatchUtility batchUtility = new(_unitOfWork);
            Guid nonExistanceBatchId = new("D53C237C-4383-4D44-8DF5-DD46B06E575A");
            A.CallTo(() => _unitOfWork.JsonDocument.GetFirstOrDefault(A<Expression<Func<BatchDemo.Models.JsonDocument, bool> >>.Ignored,A<string>.Ignored,A<bool>.Ignored)).Returns(null!);

            var jsonResult = batchUtility.DeserializeJsonDocument(nonExistanceBatchId);
            Assert.That(jsonResult.BatchId, Is.Null);
        }
    }
}
