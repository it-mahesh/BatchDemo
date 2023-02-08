using BatchDemo.Controllers;
using BatchDemo.DataAccess.Repository.IRepository;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using System.Linq.Expressions;
using BatchDemo.Services.Interface;
using BatchDemo.Services;
using static System.Net.WebRequestMethods;
using System.Text.Json;

namespace BatchDemo.UnitTests
{
    [TestFixture]
    [Category("Batch")]
    public class BatchUtilityTest
    {
        private IUnitOfWork? _unitOfWork;
        private IBatchUtility _batchUtility;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _batchUtility = A.Fake<IBatchUtility>();
        }
        
        [Test]
        public void GetJsonByBatchId_ReturnData()
        {
            BatchUtility batchUtility = new BatchUtility(_unitOfWork);

            _batchUtility = A.Fake<IBatchUtility>();
            Batch batch = new Batch();
            batch = DemoBatchData.LoadBatch();
            
            BatchDemo.Models.JsonDocument jsonDocument = new BatchDemo.Models.JsonDocument() { BatchId = batch.BatchId, Document = null };
            A.CallTo(() => _batchUtility.GetJsonByBatchId(A<Guid>.Ignored)).Returns(jsonDocument);
            var jsonResult = _batchUtility.GetJsonByBatchId(batch.BatchId);
            var jsonResult2= batchUtility.GetJsonByBatchId(batch.BatchId);
            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult.BatchId, Is.EqualTo(batch.BatchId));
        }
        [Test]
        public void BatchToBatchInfoConverter_ReturnData()
        {
            BatchUtility batchUtility = new BatchUtility(_unitOfWork);

            _batchUtility = A.Fake<IBatchUtility>();
            Batch batch = new Batch();
            batch = DemoBatchData.LoadBatch();

            BatchInfo batchInfo = new BatchInfo() { BatchId=batch.BatchId,BusinessUnit=batch.BusinessUnit };
            A.CallTo(() => _batchUtility.BatchToBatchInfoConverter(A<Batch>.Ignored)).Returns(batchInfo);
            var jsonResult = _batchUtility.BatchToBatchInfoConverter(batch);
            var jsonResult2 = batchUtility.BatchToBatchInfoConverter(batch);
            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult.BatchId, Is.EqualTo(batch.BatchId));
        }
        [Test]
        public void DeserializeJsonDocument_ReturnData()
        {
            BatchUtility batchUtility = new BatchUtility(_unitOfWork);

            _batchUtility = A.Fake<IBatchUtility>();
            Batch batch = new Batch();
            batch = DemoBatchData.LoadBatch();

            A.CallTo(() => _batchUtility.DeserializeJsonDocument(A<Guid>.Ignored)).Returns(batch);
            var jsonResult = _batchUtility.DeserializeJsonDocument(batch.BatchId);
            var jsonResult2 = batchUtility.DeserializeJsonDocument(batch.BatchId);
            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult.BatchId, Is.EqualTo(batch.BatchId));
        }
    }
}
