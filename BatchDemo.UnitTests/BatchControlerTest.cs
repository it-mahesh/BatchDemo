using Azure.Core;
using Azure.Storage.Blobs;
using BatchDemo.Controllers;
using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using BatchDemo.Services;
using BatchDemo.Services.Interface;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace BatchDemo.UnitTests
{
    [TestFixture]
    public class BatchControlerTest
    {
        private BatchController _controller;
        private ILogger<BatchController>? _logger;
        private IUnitOfWork? _unitOfWork;
        private IConfiguration? _configuration;
        private IBatchUtility _batchUtility;
        private IBatchBlobService _blobService;

        [SetUp]
        public void SetUp()
        {
            _logger = A.Fake<ILogger<BatchController>>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _configuration = A.Fake<IConfiguration>();
            _batchUtility = A.Fake<IBatchUtility>();
            _blobService = A.Fake<IBatchBlobService>();
            // SUT
            _controller = new BatchController(_logger, _unitOfWork, _configuration, _batchUtility, _blobService);
        }

        [Test]
        [Category("PostBatch")]
        public void PostBatch_WhenCreated_ReturnStatus201()
        {
            Batch batch;
            batch = DemoBatchData.LoadBatch();

            A.CallTo(() => _batchUtility.SaveBatchInFile(A<string>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(true);
            var result =
                _controller.Batch(batch) as CreatedAtActionResult;
            
            // Assert.IsInstanceOf<CreatedAtActionResult>(result);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));
            Assert.That(result.ActionName, Is.EqualTo("batch"));
            StringAssert.Contains(batch.BatchId.ToString(), result.Value != null ? result.Value.ToString() : string.Empty);
        }
        [Test]
        [Category("PostBatch")]
        public void PostFile_WhenContainerNotCreated_ReturnBadRequest()
        {
            Batch batch; 
            batch = DemoBatchData.LoadBatch();
            
            A.CallTo(() => _batchUtility.SaveBatchInFile(A<string>.Ignored, A<Guid>.Ignored,A<string>.Ignored)).Returns(true);
            A.CallTo(() => _blobService.CreateContainer(A<string>.Ignored)).Returns(null);

            var result =
                _controller.Batch(batch) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }
        [Test]
        [Category("PostBatch")]
        public void PostFile_SaveBatchInFile_ReturnError()
        {
            Batch batch;
            batch = DemoBatchData.LoadBatch();
            JsonDocument jsonDocument = new JsonDocument { BatchId = batch.BatchId };

            A.CallTo(() => _batchUtility.SaveBatchInFile(A<string>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(false);

            var result = _controller.Batch(batch) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));

        }
        [Test]
        [Category("PostBatch")]
        public void PostFile_CreateContainer_ReturnError()
        {
            Batch batch;
            batch = DemoBatchData.LoadBatch();
            JsonDocument jsonDocument = new JsonDocument { BatchId = batch.BatchId };

            BlobContainerClient? container = null;
            A.CallTo(() => _blobService.CreateContainer(A<string>.Ignored)).Returns(container);

            var result = _controller.Batch(batch) as NotFoundObjectResult;
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }
        [TestCaseSource(nameof(ExistsGuid))]
        [Category("GetBatch")]
        public void GetBatch_WhenFound_ReturnStatusOK(Guid batchId)
        {
            Batch batch = new();
            batch = DemoBatchData.LoadBatch();
            A.CallTo(() => _batchUtility.DeserializeJsonDocument(A<Guid>.Ignored)).Returns(batch);

            BatchInfo batchInfo = new()
            {
                BatchId = batchId,
                Attributes = batch.Attributes,
                BusinessUnit = batch.BusinessUnit,
                ExpiryDate = batch.ExpiryDate,
                ACL = batch.ACL,
                Files = new List<Files>() { new Files() { Attributes = batch.Attributes }
                }
            };

            A.CallTo(() => _batchUtility.BatchToBatchInfoConverter(A<Batch>.Ignored)).Returns(batchInfo);

            var result = _controller.Batch(batchId) as OkObjectResult;

            // Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
        [TestCaseSource(nameof(NotExistsGuid))]
        [Category("GetBatch")]
        public void GetBatch_WhenNotFound_ReturnNotFound(Guid batchId)
        {
            IFileService _fileService = A.Fake<IFileService>();
            ICollection<Files> files = new List<Files> { new Files { FileName = "dummy.txt", FileSize = 100, Hash = "xyz", MimeType = "application/text" } };
            A.CallTo(() => _fileService.GetBatchFiles(A<string>.Ignored)).Returns(files);

            var result = _controller.Batch(batchId) as NotFoundObjectResult;
            Assert.That(result?.StatusCode, Is.EqualTo(404));
        }
        //static Guid?[] NullAndEmptyGuid = new Guid?[] { null, Guid.Empty, new Guid("D53C237C-4383-4D44-8DF5-DD46B06E575B") };
        static readonly Guid?[] ExistsGuid = new Guid?[] { new Guid("D53C237C-4383-4D44-8DF5-DD46B06E575B") };
        static readonly Guid?[] NotExistsGuid = new Guid?[] { new Guid("D53C237C-4383-4D44-8DF5-DD46B06E575A") };

        [Test]
        [Category("GetBatch")]
        public void GetBatch_WhenDeserializeFailed_ReturnBadRequest()
        {
            Batch batch = new();
            batch = DemoBatchData.LoadBatch();
            
            BatchInfo batchInfo = new()
            {
                BatchId = batch.BatchId,
                Attributes = batch.Attributes,
                BusinessUnit = batch.BusinessUnit,
                ExpiryDate = batch.ExpiryDate,
                ACL = batch.ACL,
                Files = new List<Files>() { new Files() { Attributes = batch.Attributes }
                }
            };
            A.CallTo(() => _batchUtility.BatchToBatchInfoConverter(A<Batch>.Ignored)).Returns(batchInfo);

            A.CallTo(() => _batchUtility.DeserializeJsonDocument(A<Guid>.Ignored)).Returns(null);
            Guid batchId = new("D53C237C-4383-4D44-8DF5-DD46B06E575A");
            var result = _controller.Batch(batchId);

            Assert.That(result, Is.Not.Null);
        }
        [Test]
        [Category("GetBatch")]
        public void GetBatch_WhenBatchInfoConverterFailed_ReturnBadRequest()
        {
            Batch batch = new();
            A.CallTo(() => _batchUtility.DeserializeJsonDocument(A<Guid>.Ignored)).Returns(batch);
            A.CallTo(() => _batchUtility.BatchToBatchInfoConverter(A<Batch>.Ignored)).Returns(null);
            
            Guid batchId = new("D53C237C-4383-4D44-8DF5-DD46B06E575A");
            var result = _controller.Batch(batchId);

            Assert.That(result, Is.Not.Null);
        }
        [Test]
        [Category("PostFiles")]
        public void BatchAddFiles_WhenBatchIdNotFound_ReturnBadRequest()
        {
            Guid batchId = new("D53C237C-4383-4D44-8DF5-DD46B06E575A");
            // Incorrect file name
            string fileName = batchId.ToString() + ".txt";
            var aFakeRepository = A.Fake<IRepository<JsonDocument>>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-MIME-TYPE"] = "test-header";
            httpContext.Request.Headers["X-Content-Size"] = "test-header";

            _controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };

            JsonDocument? jsonDocument = null; //new(){ BatchId = batchId };

            A.CallTo(() => _unitOfWork!.JsonDocument.GetFirstOrDefault(A<Expression<Func<JsonDocument, bool>>>.Ignored, A<string>.Ignored, A<bool>.Ignored))
                .Returns(jsonDocument);

            var result = _controller.Batch(batchId, fileName, "application", 0f) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
        }
        [Test]
        [Category("PostFiles")]
        public void BatchAddFiles_WhenFileNotFound_ReturnBadRequest()
        {
            Guid batchId = new("D53C237C-4383-4D44-8DF5-DD46B06E575A");
            // Incorrect file name
            string fileName = batchId.ToString() + ".txt";

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-MIME-TYPE"] = "test-header";
            httpContext.Request.Headers["X-Content-Size"] = "test-header";

            _controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };


            var result = _controller.Batch(batchId, fileName, "application", 0f) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
        }
        [Test]
        [Category("PostFiles")]
        public void BatchAddFiles_WhenFileAdded_ReturnOk()
        {
            Guid batchId = new("d53c237c-4383-4d44-8df5-dd46b06e575a");
            // Correct file name
            string fileName = batchId.ToString() + ".json";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-MIME-TYPE"] = "test-header";
            httpContext.Request.Headers["X-Content-Size"] = "test-header";

            _controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            A.CallTo(() => _batchUtility.IsBatchFileExist(A<string>.Ignored)).Returns(true);
            A.CallTo(() => _blobService.PostFile(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns("ReturnSomeString");

            var result = _controller.Batch(batchId, fileName, "application", 0f) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(201));
        }
        [Test]
        [Category("PostFiles")]
        public void BatchAddFiles_FileNotAdded_ReturnBadRequest()
        {
            Guid batchId = new("d53c237c-4383-4d44-8df5-dd46b06e575a");
            // Correct file name
            string fileName = batchId.ToString() + ".json";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-MIME-TYPE"] = "test-header";
            httpContext.Request.Headers["X-Content-Size"] = "test-header";

            _controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            A.CallTo(() => _batchUtility.IsBatchFileExist(A<string>.Ignored)).Returns(true);
            A.CallTo(() => _blobService.PostFile(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(string.Empty);

            var result = _controller.Batch(batchId, fileName, "application", 0f) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
        }
        [Test]
        [Category("PostFiles")]
        public void BatchAddFiles_WhenFileNotExist_ReturnBadRequest()
        {
            Guid batchId = new("00c5900b-0f97-47d2-8d60-6abf29656ce3");
            // In Correct file name
            string fileName = batchId.ToString() + ".json";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-MIME-TYPE"] = "test-header";
            httpContext.Request.Headers["X-Content-Size"] = "test-header";

            _controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };

            var result = _controller.Batch(batchId, fileName, "application", 0f) as NotFoundObjectResult; 

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
        }
    }
}
