using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using BatchDemo.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BatchDemo.Models;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel.Design.Serialization;
using BatchDemo.DataAccess.Repository;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using BatchDemo.Utility;
using BatchDemo.Utility.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.CodeAnalysis;
using BatchDemo.Services.Interface;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.AspNetCore.StaticFiles;
using BatchDemo.Services;
using Microsoft.Extensions.Configuration;

namespace BatchDemo.Controllers
{
    /// <summary>
    /// Batch controller
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class BatchController : ControllerBase
    {
        private readonly ILogger<BatchController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IBatchUtility _batchUtility;
        private string _path;

        /// <summary>
        /// Constructor for injecting DI 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="configuration"></param>
        /// <param name="batchUtility"></param>
        public BatchController(ILogger<BatchController> logger, IUnitOfWork unitOfWork, IConfiguration configuration, IBatchUtility batchUtility)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _batchUtility = batchUtility;
            _path = Directory.GetCurrentDirectory() + "\\Files\\Batches";
        }

        /// <summary>
        /// Create a new batch to upload files into
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        /// <response code="201">Created</response>
        /// <response code="400">Bad request - there are one or more errors in the specified parameters</response>
        /// <response code="401">Unauthorized - either you have not provided any credentials, or your credentials are not recognised.</response>
        /// <response code="403">Forbidden - you have been authorized, but you are not allowed to access this resource.</response>
        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ValidateModel]
        public IActionResult Batch(Batch batch)
        {
            Guid BatchId = Guid.NewGuid();
            batch.BatchId = BatchId;
            string JsonResult = JsonConvert.SerializeObject(batch);
            JsonDocument jsonDocument = new JsonDocument
            {
                BatchId = BatchId,
                Document = JsonResult
            };
            _unitOfWork.JsonDocument.Add(jsonDocument);
            _unitOfWork.Save();

            SaveBatchInFile(JsonResult, BatchId);
            // return StatusCode(StatusCodes.Status201Created);
            return CreatedAtAction("batch", new { batchId = BatchId });
        }
        
        private void SaveBatchInFile(string jsonResult, Guid batchId)
        {
            // Construct path with BatchId as directory name 
            _path += "\\" + batchId.ToString();

            if (!System.IO.Directory.Exists(_path))
            {
                System.IO.Directory.CreateDirectory(_path);
            }

            // Concatenating file name in path as batchid.
            _path += "\\" + batchId.ToString() + ".json";
            DeleteFileIfExists(_path);

            using (var tw = new StreamWriter(_path, true))
            {
                _logger.LogInformation("Writing contete in json file {0}", batchId);
                tw.WriteLine(jsonResult.ToString());
                tw.Close();
            }
        }
        [ExcludeFromCodeCoverage]
        private void DeleteFileIfExists(string path)
        {
            if (System.IO.File.Exists(path))
            {
                _logger.LogInformation("File deleted from the path {1} ", path);
                System.IO.File.Delete(_path);
            }

        }

        /// <summary>
        /// Get details of the batch including links to all the files in the batch.
        /// </summary>
        /// <param name="batchId">A Batch ID</param>
        /// <returns></returns>
        /// <remarks>
        /// This get will include full details of the batch, for example it's status, the files in the batch.
        /// </remarks>
        /// <response code="200">OK - Return dtails about the batch.</response>
        /// <response code="400">Bad request - could be an invalid batch ID format. Batch Ids should be a GUID. A valid GUID that does n't match a batch ID will return a 404</response>
        /// <response code="401">Unauthorized - either you have not provided any credentials, or your credentials are not recognised.</response>
        /// <response code="403">Forbidden - you have been authorized, but you are not allowed to access this resource.</response>
        /// <response code="404">Not Found - Could be that the batch ID doesn't exist.</response>
        /// <response code="410">Gone - the batch has been expired and is no longer available.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        [HttpGet("batch/{batchId}")]
        public IActionResult Batch(Guid batchId)
        {
            // Set path of folder for supplied batchId.
             //string folderPath = Directory.GetCurrentDirectory() + "\\Files\\Batches\\"+batchId.ToString();
            string folderPath = Directory.GetCurrentDirectory() + _configuration.GetValue <string>("BatchesFolderPath") + batchId.ToString();
            Batch batch = new Batch();
            FileService fileService = new FileService();
            batch = _batchUtility.DeserializeJsonDocument(batchId);
            BatchInfo batchInfo = new BatchInfo();
            batchInfo = _batchUtility.BatchToBatchInfoConverter(batch);
            // Set static files details located at batchid folder.
            batchInfo.Files= fileService.GetBatchFiles(folderPath);
            
            if (batchInfo.BatchId is null)
            {
                //return NotFound(StatusCodes.Status404NotFound);
                ModelStateDictionary modelState=new ModelStateDictionary();
                modelState.AddModelError("BatchId ", "BatchId not found.");
                //return new ValidationResultModel(modelState);
                return NotFound(new ValidationResultModel(modelState));
            }
            return Ok(batchInfo);
        }
    }

    
}
