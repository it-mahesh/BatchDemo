using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using BatchDemo.Services;
using BatchDemo.Services.Interface;
using FakeItEasy;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;

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
            _batchBlobService = new BatchBlobService(_configuration, _keyVaultManager);
        }

        [Test]
        public void PostFileAsync_ReturnsValue()
        {


            //A.CallTo(() => _batchBlobService!.GetDbConnectionFromAzureVault()).Returns("found");
           var returnedResult = _batchBlobService?.PostFileAsync("", "", "", "");
            Assert.That(returnedResult, Is.Not.Null);
        }

    }
}
