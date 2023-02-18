using BatchDemo.Models;
//using BatchDemo.Models.Enum;
namespace BatchDemo.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Read file details located at batchid folder.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public ICollection<Files> GetBatchFiles(string folderPath);

    }
}
