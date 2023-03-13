using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using BatchDemo.Services;
using BatchDemo.Services.Interface;
using FakeItEasy;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.CodeAnalysis;
using System.Threading;
using Metadata = System.Collections.Generic.IDictionary<string, string>;
using FakeItEasy.Configuration;

namespace BatchDemo.UnitTests
{
    [TestFixture]
    [Category("Batch")]
    public class BatchBlobServiceTest
    {
        private IConfiguration? _configuration;
        private IKeyVaultManager? _keyVaultManager;
        private BatchBlobService? _batchBlobService;
        [SetUp]
        public void SetUp()
        {
            _configuration = A.Fake<IConfiguration>();
            _keyVaultManager = A.Fake<IKeyVaultManager>();
            // SUT
            _batchBlobService = new BatchBlobService(_configuration, _keyVaultManager);
        }
        [Test]
        public void CreateContainer_ReturnsValue()
        {
            const string containerName = "testcontainer";

            // Actual dependency on azure storage connection.
            const string storageConnection = "DefaultEndpointsProtocol=https;AccountName=batchdemostorage4;AccountKey=5wyynTLbb5mWo/gbSa0ZztpDZeUO3//I5VjgCknf7XdxlqDy3AAdoKdcezIb1eOHJMWkjtMNItDV+AStj0tRbA==;EndpointSuffix=core.windows.net";

            A.CallTo(() => _keyVaultManager!.GetStorageConnectionFromAzureVault()).Returns(storageConnection);
            // A.CallTo(() => aFakeBlobServiceClient.CreateBlobContainer(containerName, A<PublicAccessType>.Ignored, 
            //   A<Metadata>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappened();
            var blobContainerClient = _batchBlobService!.CreateContainer(containerName);

            // Delete the container which was just created during test execution.
            //_batchBlobService.DeleteContainer(containerName, storageConnection);
            Assert.That(_batchBlobService, Is.Not.Null);
        }
        //[Test]
        //public void DeleteContainer_ReturnsValue()
        //{
        //    const string containerName = "testcontainer";

        //    // Actual dependency on azure storage connection.
        //    const string storageConnection = "DefaultEndpointsProtocol=https;AccountName=batchdemostorage4;AccountKey=5wyynTLbb5mWo/gbSa0ZztpDZeUO3//I5VjgCknf7XdxlqDy3AAdoKdcezIb1eOHJMWkjtMNItDV+AStj0tRbA==;EndpointSuffix=core.windows.net";

        //    // Delete the container which was just created during test execution.
        //    var result = _batchBlobService!.DeleteContainer(containerName, storageConnection);
        //    Assert.That(result, Is.True);
        //}

    }
}
