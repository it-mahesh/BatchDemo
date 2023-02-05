using Microsoft.AspNetCore.Mvc;
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
        private string _path = @"D:\OneDrive\OneDrive - Mastek Limited\0_Learning\Projects\BatchDemo\BatchDemo\Files\json\";

        /// <summary>
        /// Constructor for injecting DI 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="configuration"></param>
        public BatchController(ILogger<BatchController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
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
            _path += batchId.ToString();
            if (!System.IO.Directory.Exists(_path))
            {
                System.IO.Directory.CreateDirectory(_path);
            }

            // Concatenating file name in path as batchid.
            _path += "/" + batchId.ToString() + ".json";
            if (System.IO.File.Exists(_path))
            {
                _logger.LogInformation("File {0} deleted from the path {1} ",batchId,_path);
                System.IO.File.Delete(_path);
            }

            using (var tw = new StreamWriter(_path, true))
            {
                _logger.LogInformation("Writing contete in json file {0}", batchId);
                tw.WriteLine(jsonResult.ToString());
                tw.Close();
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
            //Guid guidResult;

            //if (!Guid.TryParse(batchId.ToString(), out guidResult))
            //{
            //    return BadRequest("BatchId should be in valid GUID format.");
            //}

            Batch batch = new Batch();
            //JsonDocument jsonDocument = _unitOfWork.JsonDocument.GetFirstOrDefault(u => u.BatchId == batchId);
            //batch = JsonConvert.DeserializeObject<Batch>(jsonDocument.Document ?? string.Empty);
            batch = LoadAndDeserializeBatch(batchId);

            BatchInfo batchInfo = new BatchInfo();
            batchInfo.BatchId = batch!.BatchId;
            batchInfo.Attributes = batch.Attributes;
            batchInfo.BusinessUnit = batch.BusinessUnit;
            batchInfo.ExpiryDate = batch.ExpiryDate;
            batchInfo.ACL= batch.ACL;
            batchInfo.Files = new List<Files>() { new Files() {Attributes=batch.Attributes } };

            return Ok(batchInfo);
        }
        //private JsonDocument Loadbatch(Guid batchId)
        //{
        //return _unitOfWork.JsonDocument.GetFirstOrDefault(u => u.BatchId == batchId);
        //}
        private Batch LoadAndDeserializeBatch(Guid batchId)
        {
            BatchDemo.Models.JsonDocument jsonDocument = _unitOfWork.JsonDocument.GetFirstOrDefault(u => u.BatchId == batchId);
            return JsonConvert.DeserializeObject<Batch>(jsonDocument.Document ?? string.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[HttpPost("LoadBatch")]
        //public IActionResult LoadBatch()
        //{
        //    Batch batch = new Batch();
        //    BatchData batchData = new BatchData();
        //    batch = batchData.DataOperations();

        //    string JsonResult = JsonConvert.SerializeObject(batch);
        //    using (var tw = new StreamWriter(_path, true))
        //    {
        //        tw.WriteLine(JsonResult.ToString());
        //        tw.Close();
        //    }


        //    return Ok();
        //}
    }
}
