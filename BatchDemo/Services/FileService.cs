using BatchDemo.Services.Interface;
using BatchDemo.DataAccess;
//using BatchDemo.Models.Enum;
using BatchDemo.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using System.Configuration;
using System.Security.Cryptography;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics.CodeAnalysis;

namespace BatchDemo.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class FileService : IFileService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private string GetMimeTypeForFileExtension(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string? contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="cryptoService"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private string GetHashCode(string filePath, HashAlgorithm cryptoService)
        {
            // create or use the instance of the crypto service provider
            // this can be either MD5, SHA1, SHA256, SHA384 or SHA512
            using (cryptoService)
            {
                using (var fileStream = new FileStream(filePath,
                                                       FileMode.Open,
                                                       FileAccess.Read,
                                                       FileShare.ReadWrite))
                {
                    var hash = cryptoService.ComputeHash(fileStream);
                    var hashString = Convert.ToBase64String(hash);
                    return hashString.TrimEnd('=');
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        public ICollection<Files> GetBatchFiles(string folderPath)
        {
            ICollection<Files> files = new List<Files>();
            DirectoryInfo di = new DirectoryInfo(folderPath);
            FileInfo[] fileNames;
            try
            {
                fileNames = di.GetFiles("*.*");
            }
            catch (Exception)
            {
                return files;
            }            
            foreach (System.IO.FileInfo fi in fileNames)
            {
                files.Add(new Files
                {
                    FileName = fi.Name,
                    FileSize = fi.Length,
                    MimeType = GetMimeTypeForFileExtension(fi.FullName),
                    Hash = GetHashCode(fi.FullName, new MD5CryptoServiceProvider())
                });
            }
            return files;
        }
    }
}
