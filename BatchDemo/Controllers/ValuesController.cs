using BatchDemo.DataAccess.Repository;
using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Configuration;

namespace BatchDemo.Controllers
{
    [Route("batch")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private string _path = @"D:\OneDrive\OneDrive - Mastek Limited\0_Learning\Projects\BatchDemo\BatchDemo\Files\json\Batch.json";

        public ValuesController(ILogger<ValuesController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        [HttpPost(Name = "batch")]
        public IActionResult Batch(Batch batch)
        {
            //BatchData batchData= new BatchData();
            //batch = batchData.DataOperations();

            string JsonResult = JsonConvert.SerializeObject(batch);

            if (System.IO.File.Exists(_path))
            {
                System.IO.File.Delete(_path);
            }
            using (var tw = new StreamWriter(_path, true))
            {
                tw.WriteLine(JsonResult.ToString());
                tw.Close();
            }
            return Ok(201);
        }
        [HttpPost("LoadBatch")]
        public IActionResult LoadBatch()
        {
            Batch batch = new Batch();
            BatchData batchData = new BatchData();
            batch = batchData.DataOperations();

            string JsonResult = JsonConvert.SerializeObject(batch);
            using (var tw = new StreamWriter(_path, true))
            {
                tw.WriteLine(JsonResult.ToString());
                tw.Close();
            }
            return Ok(201);
        }
        [HttpGet("batch/{batchId}")]
        public IActionResult Batch(Guid batchId)
        {
            return Ok(_unitOfWork.Employee.GetAll());
        }
    }
}