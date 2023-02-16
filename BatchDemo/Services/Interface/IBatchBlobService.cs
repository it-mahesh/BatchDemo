using Azure.Storage.Blobs;
//using BatchDemo.Models.Enum;
namespace BatchDemo.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBatchBlobService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
       public BlobContainerClient? CreateContainer(string containerName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="filePath"></param>
        /// <param name="mimeType"></param>
        /// <param name="contentSize"></param>
        /// <returns></returns>
        public Task<string> PostFileAsync(string containerName, string filePath, string mimeType, string contentSize);

    }
}
