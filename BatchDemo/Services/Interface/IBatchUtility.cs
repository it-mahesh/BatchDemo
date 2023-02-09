using BatchDemo.Models;

namespace BatchDemo.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBatchUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        // public JsonDocument GetJsonByBatchId(Guid? batchId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public Batch DeserializeJsonDocument(Guid? batchId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public BatchInfo BatchToBatchInfoConverter(Batch batch);
    }
}
