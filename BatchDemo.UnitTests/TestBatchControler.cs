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

namespace BatchDemo.UnitTests
{
    [TestFixture]
    [Category("Batch")]
    public class TestBatchControler
    {
        private BatchController _controller;
        private ILogger<BatchController>? _logger;
        private IUnitOfWork? _unitOfWork;
        private IConfiguration? _configuration;
        [SetUp]
        public void SetUp()
        {
            _logger = A.Fake<ILogger<BatchController>>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _configuration = A.Fake<IConfiguration>();
            _controller = new BatchController(_logger, _unitOfWork, _configuration);
        }

        [Test,Order(1)]
        public void PostBatch_ShouldReturnStatusCreated201()
        {
            Batch batch = new Batch();
            DemoBatchData demoBatchData= new DemoBatchData();
            batch = demoBatchData.LoadBatch();
            var result =
                _controller.Batch(batch) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));
            Assert.That(result.ActionName, Is.EqualTo("batch"));
            StringAssert.Contains(batch.BatchId.ToString(), result.Value != null ? result.Value.ToString() : string.Empty);
        }
        //Batch GetDemoBatchId()
        //{
        //    return new Batch() { BatchId = new Guid("19330cdc-cb38-4989-abbe-acde7359a24b") };
        //}
        //Guid GetDemoBatchId()
        //{
        //    return new Guid("19330cdc-cb38-4989-abbe-acde7359a24b");
        //}
        [Test]
        public void GettBatch_ShouldReturnStatusOK()
        {
            Batch batch = new Batch();
            DemoBatchData demoBatchData = new DemoBatchData();
            batch = demoBatchData.LoadBatch();
            //batch = GetDemoBatchId();
            //batch =A.Fake<Batch>();
            // A.CallTo()
            
            var OkResult =
                _controller.Batch(batch.BatchId) as OkObjectResult;

            Assert.That(OkResult, Is.Not.Null);
            Assert.That(OkResult.StatusCode, Is.EqualTo(200));
            Assert.That(OkResult.Value, Is.EqualTo("batch"));

        }
    }
}
