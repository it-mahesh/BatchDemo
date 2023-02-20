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
        public Batch DeserializeJsonDocument(Guid? batchId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public BatchInfo BatchToBatchInfoConverter(Batch batch);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonResult"></param>
        /// <param name="batchId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SaveBatchInFile(string jsonResult, Guid batchId, string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool DirectoryCreated(string path);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsBatchFileExist(string filePath);
    }
}
