using Azure.Storage.Blobs;
using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using BatchDemo.Services;
using BatchDemo.Services.Interface;
using BatchDemo.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace BatchDemo.Controllers
{
    /// <summary>
    /// Batch controller
    /// </summary>
    [ApiController]
    [Route("/")]
    [Produces("application/json")]
    public class BatchController : ControllerBase
    {
        #region Private Properties
        private readonly ILogger<BatchController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IBatchUtility _batchUtility;
        private readonly IBatchBlobService _blobService;
        private string _path;
        private ModelStateDictionary? _modelStateDictionary;
        #endregion

        #region Private Methods
        private bool SaveBatchInFile(string jsonResult, Guid batchId)
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

            using var tw = new StreamWriter(_path, true);
            _logger.LogInformation("Writing contete in json file {batchId}", batchId);
            tw.WriteLine(jsonResult.ToString());
            tw.Close();
            return true;
        }
        [ExcludeFromCodeCoverage]
        private void DeleteFileIfExists(string path)
        {
            if (System.IO.File.Exists(path))
            {
                _logger.LogInformation("File deleted from the path {path} ", path);
                System.IO.File.Delete(_path);
            }

        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for injecting DI 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="configuration"></param>
        /// <param name="batchUtility"></param>
        /// <param name="blobService"></param>
        public BatchController(ILogger<BatchController> logger, IUnitOfWork unitOfWork, IConfiguration configuration
            , IBatchUtility batchUtility, IBatchBlobService blobService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _batchUtility = batchUtility;
            _path = Directory.GetCurrentDirectory() + _configuration.GetValue<string>("BatchesFolderPath");
            _blobService = blobService;
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
                JsonDocument jsonDocument = new()
                {
                    BatchId = BatchId,
                    Document = JsonResult
                };


                // Save batch details in database.
                _unitOfWork.JsonDocument.Add(jsonDocument);
                _unitOfWork.Save();

                // Save json file in directory.
                if (!SaveBatchInFile(JsonResult, BatchId))
                {
                    _logger.LogError("File could not save in directory for {batchId}", BatchId);
                    _modelStateDictionary = new();
                    _modelStateDictionary.AddModelError("FileDirectoryError", "File could not save in directory.");
                    return NotFound(new ValidationResultModel(_modelStateDictionary));
                }

                // Connect with azure storage and create container.
                BlobContainerClient? container = _blobService.CreateContainer(BatchId.ToString());
                if (container is null)
                {
                    _logger.LogError("Azure container could not create for {batchId}", BatchId);
                    _modelStateDictionary = new();
                    _modelStateDictionary.AddModelError("ConatinerNotCreated", "Azure container could not create.");
                    return NotFound(new ValidationResultModel(_modelStateDictionary));
                }
            // return StatusCode(StatusCodes.Status201Created);
            return CreatedAtAction("batch", new { batchId = BatchId });
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
            string folderPath = Directory.GetCurrentDirectory() + _configuration.GetValue<string>("BatchesFolderPath") + batchId.ToString();
            Batch batch;
            FileService fileService = new();
            batch = _batchUtility.DeserializeJsonDocument(batchId);
            if (batch is null)
            {
                _logger.LogError($"Batch {batchId} is null returned from DeserializeJsonDocument().");
                _modelStateDictionary = new();
                _modelStateDictionary.AddModelError("DeserializeJsonDocument ", "Batch details could not been serialized.");
                return BadRequest(new ValidationResultModel(_modelStateDictionary));
            }
            BatchInfo batchInfo = _batchUtility.BatchToBatchInfoConverter(batch);
            if (batchInfo is null)
            {
                _logger.LogError($"BatchInfo object for {batchId} is null returned from BatchToBatchInfoConverter().");
            }
            // Set static files details located at batchid folder.
            batchInfo!.Files = fileService.GetBatchFiles(folderPath);

            if (batchInfo.BatchId is null)
            {
                //return NotFound(StatusCodes.Status404NotFound);
                _logger.LogError($"BatchInfo.BatchId object for {batchId} is null returned from GetBatchFiles().");
                _modelStateDictionary = new();
                _modelStateDictionary.AddModelError("BatchId ", "BatchId not found.");
                //return new ValidationResultModel(_modelStateDictionary);
                return NotFound(new ValidationResultModel(_modelStateDictionary));
            }
            return Ok(batchInfo);
        }
        /// <summary>
        /// Add a file to the batch
        /// </summary>
        /// <param name="batchId">A Batch ID</param>
        /// <param name="fileName">FileName for the new file. Must be unique in the batch (but can be the same as another file in another batch). 
        /// File names don't include a path.</param>
        /// <param name="fileMimeType">Optional. The MIME content type of the file. The default type is application/octet-stream.</param>
        /// <param name="fileContentSize">The final size of the file in bytes.</param>
        /// <returns></returns>
        /// <remarks>
        /// Creates a file in the batch. To upload the content of the file, one or more uploadBlockOfFile requests will need to be made followed by a 
        /// 'putBlocksInFile' request to complete the file.
        /// </remarks>
        /// <response code="201">Created</response>
        /// <response code="400">Bad request - Could be a bad batch ID; a batch ID that doesn't exist; a bad file name.</response>
        /// <response code="401">Unauthorized - either you have not provided any credentials, or your credentials are not recognised.</response>
        /// <response code="403">Forbidden - you have been authorized, but you are not allowed to access this resource.</response>
        [HttpPost("batch/{batchId}/{fileName}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ValidateModel]
        public IActionResult Batch(Guid batchId, string fileName, [FromHeader(Name = "X-MIME-TYPE")] string fileMimeType
            , [FromHeader(Name = "X-Content-Size")] float fileContentSize
           )
        {
            // D53C237C-4383-4D44-8DF5-DD46B06E575B
            string folderPath = Directory.GetCurrentDirectory() + _configuration.GetValue<string>("BatchesFolderPath") + batchId.ToString();
            string filePath = folderPath + "\\" + fileName;

            Request.Headers.TryGetValue("X-MIME-TYPE", out var mimeType);
            Request.Headers.TryGetValue("X-Content-Size", out var contentSize);

            // Validate if BatchId not exists
            JsonDocument jsonDocument = _unitOfWork.JsonDocument.GetFirstOrDefault(u => u.BatchId == batchId);
            if (jsonDocument is null)
            {
                _logger.LogError($"{batchId} not found in database.");
                _modelStateDictionary = new();
                _modelStateDictionary.AddModelError("BatchId ", "BatchId doesn't exists.");
                return NotFound(new ValidationResultModel(_modelStateDictionary));
            }

            // validate if file doesn't exists in batch directory
            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogError($"{fileName} doesn't exist.");
                _modelStateDictionary = new();
                _modelStateDictionary.AddModelError("FileNotFound ", "File doesn't exist.");
                return NotFound(new ValidationResultModel(_modelStateDictionary));
            }

            // Store files details into database
            Files files = new()
            {
                BatchId = batchId,
                FileName = fileName,
                MimeType = mimeType!
                // ,Attributes = new List<Attributes>{ new Attributes { Key = "size", Value = "200mb" }, new Attributes { Key = "type", Value = "json" } }
            };

            // Save file details in database
            _unitOfWork.Files.Add(files);
            _unitOfWork.Save();

            // Check if file could not add at azure storage. 
            if (string.IsNullOrWhiteSpace(_blobService.PostFile(batchId.ToString(), filePath, mimeType!, contentSize!)))
            {
                _logger.LogError($"{fileName} couldn't upload at azure storage.");
                _modelStateDictionary = new();
                _modelStateDictionary.AddModelError("AzureStorage", "File couldn't upload at azure.");
                return NotFound(new ValidationResultModel(_modelStateDictionary));
            }
            return CreatedAtAction("batch", new { batchId = batchId });
        }
        //[HttpGet("index")]
        //public string index()
        //{

        //    return "Hello BatchDemo";
        //}
        #endregion
    }
}
