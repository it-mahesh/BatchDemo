using BatchDemo.Controllers;
using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using BatchDemo.Services.Interface;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        private IBatchBlobService _blobService;
        private IKeyVaultManager _keyVaultManager;

        [SetUp]
        public void SetUp()
        {
            _logger = A.Fake<ILogger<BatchController>>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _configuration = A.Fake<IConfiguration>();
            _batchUtility = A.Fake<IBatchUtility>();
            _blobService = A.Fake<IBatchBlobService>();
            _keyVaultManager = A.Fake<IKeyVaultManager>();
            _controller = new BatchController(_logger, _unitOfWork, _configuration, _batchUtility, _blobService);
        }

        [Test]
        public void PostBatch_WhenCreated_ReturnStatus201()
        {
            Batch batch = new();
            batch = DemoBatchData.LoadBatch();

            var result =
                _controller.Batch(batch) as CreatedAtActionResult;

            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));
            Assert.That(result.ActionName, Is.EqualTo("batch"));
            StringAssert.Contains(batch.BatchId.ToString(), result.Value != null ? result.Value.ToString() : string.Empty);
        }

        [TestCaseSource(nameof(ExistsGuid))]
        public void GetBatch_WhenFound_ReturnStatusOK(Guid batchId)
        {
            Batch batch = new();
            batch= DemoBatchData.LoadBatch();
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
            
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
        [TestCaseSource(nameof(NotExistsGuid))]
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

        
    }
}
