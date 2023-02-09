using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using BatchDemo.Services.Interface;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace BatchDemo.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class BatchUtility : IBatchUtility
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public BatchUtility(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        private JsonDocument GetJsonByBatchId(Guid? batchId)
        {
            JsonDocument jsonDocument = _unitOfWork.JsonDocument.GetFirstOrDefault(u => u.BatchId == batchId);
            return jsonDocument;
        }
        /// <summary>
        /// To be used in Batch controller
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public Batch DeserializeJsonDocument(Guid? batchId)
        {
            JsonDocument jsonDocument = GetJsonByBatchId(batchId);
            if (jsonDocument is null)
            {
                return new Batch();
            }
            else 
            {
                return JsonConvert.DeserializeObject<Batch>(jsonDocument.Document);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public BatchInfo BatchToBatchInfoConverter(Batch batch)
        {
            return new BatchInfo()
            {
                BatchId = batch.BatchId,
                Attributes = batch.Attributes,
                BusinessUnit = batch.BusinessUnit,
                ExpiryDate = batch.ExpiryDate,
                ACL = batch.ACL,
                Files = new List<Files>() { new Files() { Attributes = batch.Attributes }
                }
            };
        }
    }
}
