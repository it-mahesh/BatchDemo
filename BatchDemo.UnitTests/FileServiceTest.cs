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
using Newtonsoft.Json.Linq;

namespace BatchDemo.UnitTests
{
    [TestFixture]
    [Category("FileService")]
    public class FileServiceTest
    {
        private IFileService _fileService;

        [SetUp]
        public void SetUp()
        {
            _fileService = A.Fake<IFileService>();
        }
        
        [Test]
        public void GetBatchFiles_ReturnDummyFiles()
        {
            ICollection<Files> files = new List<Files> { new Files { FileName="dummy.txt", FileSize=100, Hash="xyz", MimeType="application/text" } };
            string folderpath = "d:";
            A.CallTo(() => _fileService.GetBatchFiles(A<string>.Ignored)).Returns(files);            
            var jsonResult = _fileService.GetBatchFiles(folderpath) as IList<Files>;

            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult[0].FileName, Is.EqualTo("dummy.txt"));
        }
        [Test]
        public void GetBatchFiles_ReturnActualFiles()
        {
            FileService fileService=new FileService();
            string folderpath = "d:";
            var jsonResult = fileService.GetBatchFiles(folderpath) as ICollection<Files>;
            Assert.That(jsonResult, Is.Not.Null);
        }
    }
}
