using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using BatchDemo.Services;
using BatchDemo.Services.Interface;
using BatchDemo.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

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
        private readonly IBatchBlobService _blobService;
        private string _path;
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
            _path = Directory.GetCurrentDirectory() + "\\Files\\Batches";
            _blobService = blobService;
            //// D53C237C-4383-4D44-8DF5-DD46B06E575B
            ////var kvUrl = _configuration.GetSection("KeyVaultConfig:KVUrl").Value;  
            ////var secretClient = new SecretClient(new Uri(kvUrl), new DefaultAzureCredential());
            ////var sqlConnString = secretClient.GetSecret("dbConnection");
            //
            //string? tenantId = _configuration.GetSection("KeyVaultConfig:TenantId").Value;
            //string? clientId = _configuration.GetSection("KeyVaultConfig:ClientId").Value;
            //string? clientSecret = _configuration.GetSection("KeyVaultConfig:ClientSecretId").Value;
            //ClientSecretCredential clientSecretCredential = new(tenantId, clientId, clientSecret);
            //string? keyVaultUrl = _configuration.GetSection("KeyVaultConfig:KVUrl").Value;
            //string? secretName = _configuration.GetSection("KeyVaultConfig:DbConnSecretName").Value;

            //SecretClient secretClient = new(new Uri(keyVaultUrl!), clientSecretCredential);

            //var secret = secretClient.GetSecret(secretName);
            //var val = secret.Value.Value;
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
            _unitOfWork.JsonDocument.Add(jsonDocument);
            _unitOfWork.Save();

            SaveBatchInFile(JsonResult, BatchId);

            _blobService.CreateContainer(BatchId.ToString());

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

            using var tw = new StreamWriter(_path, true);
            _logger.LogInformation("Writing contete in json file {batchId}", batchId);
            tw.WriteLine(jsonResult.ToString());
            tw.Close();
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
            string folderPath = Directory.GetCurrentDirectory() + _configuration.GetValue<string>("BatchesFolderPath") + batchId.ToString();
            Batch batch;
            FileService fileService = new();
            batch = _batchUtility.DeserializeJsonDocument(batchId);
            BatchInfo batchInfo = _batchUtility.BatchToBatchInfoConverter(batch);
            // Set static files details located at batchid folder.
            batchInfo.Files = fileService.GetBatchFiles(folderPath);

            if (batchInfo.BatchId is null)
            {
                //return NotFound(StatusCodes.Status404NotFound);
                ModelStateDictionary modelState = new();
                modelState.AddModelError("BatchId ", "BatchId not found.");
                //return new ValidationResultModel(modelState);
                return NotFound(new ValidationResultModel(modelState));
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
                ModelStateDictionary modelState = new();
                modelState.AddModelError("BatchId ", "BatchId doesn't exists.");
                return NotFound(new ValidationResultModel(modelState));
            }

            // validate if file doesn't exists in batch directory
            if (!System.IO.File.Exists(filePath))
            {
                ModelStateDictionary modelState = new();
                modelState.AddModelError("FileNotFound ", "File doesn't exists.");
                return NotFound(new ValidationResultModel(modelState));
            }

            // Store files details into database
            Files files = new()
            {
                BatchId = batchId,
                FileName = fileName,
                MimeType = mimeType!
            // ,Attributes = new List<Attributes>{ new Attributes { Key = "size", Value = "200mb" }, new Attributes { Key = "type", Value = "json" } }
            };

            _unitOfWork.Files.Add(files);
            _unitOfWork.Save();

            _blobService.PostFileAsync(batchId.ToString(), filePath, mimeType!, contentSize!);

            return CreatedAtAction("batch", new { batchId = batchId });
        }
    }


}
