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
using NuGet.ContentModel;

namespace BatchDemo.UnitTests
{
    [TestFixture]
    [Category("Batch")]
    public class BatchControlerTest
    {
        private BatchController _controller;
        private ILogger<BatchController>? _logger;
        private IUnitOfWork? _unitOfWork;
        private IConfiguration? _configuration;
        private IBatchUtility _batchUtility;

        [SetUp]
        public void SetUp()
        {
            _logger = A.Fake<ILogger<BatchController>>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _configuration = A.Fake<IConfiguration>();
            _batchUtility = A.Fake<IBatchUtility>();
            _controller = new BatchController(_logger, _unitOfWork, _configuration, _batchUtility);
        }

        [Test]
        public void PostBatch_WhenCreated_ReturnStatus201()
        {
            Batch batch = new Batch();
            batch = DemoBatchData.LoadBatch();
            var result =
                _controller.Batch(batch) as CreatedAtActionResult;

            // A.CallTo(() => _controller.DeleteFileIfExists(A<string>.Ignored)).MustHaveHappened();
            // A.CallTo(() => _logger.LogInformation(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened();
            A.CallTo(() => _logger.LogInformation("File deleted from the path {1} ", A<string>.Ignored)).MustHaveHappened();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));
            Assert.That(result.ActionName, Is.EqualTo("batch"));
            StringAssert.Contains(batch.BatchId.ToString(), result.Value != null ? result.Value.ToString() : string.Empty);
        }
        /*
        [Test]
        public void GettBatch_WhenFound_ReturnStatusOK()
        {
            Batch batch = new Batch();
            DemoBatchData demoBatchData = new DemoBatchData();
            batch = demoBatchData.LoadBatch();

            BatchInfo batchInfo = new BatchInfo() 
            {
                BatchId = batch.BatchId,
                Attributes = batch.Attributes,
                BusinessUnit = batch.BusinessUnit,
                ExpiryDate = batch.ExpiryDate,
                ACL = batch.ACL,
                Files = new List<Files>() { new Files() { Attributes = batch.Attributes }
                }
            };
            ////_controller = A.Fake<BatchController>();
            ////var convertFromBatch = A.Fake<IConvertFromBatch>();
            // test result as well along with status code
            // A.CallTo(() => _controller.ConvertFromBatch(batch)).Returns<BatchInfo>(batchInfo);
            ////A.CallTo(() => convertFromBatch.ConvertFromBatch(batch)).Returns<BatchInfo>(batchInfo);

            ////BatchInfo result = convertFromBatch.ConvertFromBatch(batch);
            ////Assert.That(_controller, Is.Not.Null);


            var OkResult =
                _controller.Batch(batch.BatchId) as OkObjectResult;
                //_controller.Batch(batchInfo.BatchId) as OkObjectResult;

            //var Result =
            //_controller.Batch(batch.BatchId);
            //batchInfo.BatchId = batch!.BatchId;
            //batchInfo.Attributes = batch.Attributes;
            //batchInfo.BusinessUnit = batch.BusinessUnit;
            //batchInfo.ExpiryDate = batch.ExpiryDate;
            //batchInfo.ACL = batch.ACL;
            //batchInfo.Files = new List<Files>() { new Files() { Attributes = batch.Attributes } };

            //Assert.That(Result,Is.AnyOf(batchInfo.Files));

            //MethodInfo methodInfo = typeof(BatchController)
            //    .GetMethod("ConvertFromBatch",
            //                BindingFlags.NonPublic | BindingFlags.Instance);
            //object[] parameters = { "God" };

            //object result = methodInfo.Invoke(_controller, parameters);

            //Assert.True((bool)result);


            //Assert.That(OkResult, Is.Not.Null);
            //Assert.That(OkResult.StatusCode, Is.EqualTo(200));
            //Assert.That(OkResult.Value, Is.EqualTo("batch"));

        }
        */
        //[Test]
        //public void BatchFound()
        //{
        //    Batch batch = new Batch();
        //    DemoBatchData demoBatchData = new DemoBatchData();
        //    batch = demoBatchData.LoadBatch();

        //    BatchInfo batchInfo = new BatchInfo()
        //    {
        //        BatchId = batch.BatchId,
        //        Attributes = batch.Attributes,
        //        BusinessUnit = batch.BusinessUnit,
        //        ExpiryDate = batch.ExpiryDate,
        //        ACL = batch.ACL,
        //        Files = new List<Files>() { new Files() { Attributes = batch.Attributes }
        //        }
        //    };
        //    var OkResult =
        //        _controller.Batch(batch.BatchId) as OkObjectResult;

        //    A.CallTo(() => _batchUtility.DeserializeJsonDocument(A<Guid>.Ignored)).Returns<Batch>(batch);
        //    A.CallTo(() => _batchUtility.BatchToBatchInfoConverter(A<Batch>.Ignored)).Returns<BatchInfo>(batchInfo);

        //    //Assert.That(OkResult, Is.Not.Null);
        //    //Assert.That(OkResult.StatusCode, Is.EqualTo(200));
        //    //Assert.That(OkResult.Value, Is.EqualTo("batch"));

        //}
        //[Test]
        //public void GetBatch_WhenFound_ReturnStatusOK()
        //{
        //    Batch batch = new Batch();
        //    DemoBatchData demoBatchData = new DemoBatchData();
        //    batch = demoBatchData.LoadBatch();

        //    BatchInfo batchInfo = new BatchInfo()
        //    {
        //        BatchId = batch.BatchId,
        //        Attributes = batch.Attributes,
        //        BusinessUnit = batch.BusinessUnit,
        //        ExpiryDate = batch.ExpiryDate,
        //        ACL = batch.ACL,
        //        Files = new List<Files>() { new Files() { Attributes = batch.Attributes }
        //        }
        //    };

        // var OkResult =  _controller.Batch(batch.BatchId) as OkObjectResult;
        //}
        
        [Test]
        public void GettBatch_WhenFound_ReturnStatusOK()
        {
            var result = _controller.Batch(DemoBatchData.GetDummyBatchId());
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }
}
