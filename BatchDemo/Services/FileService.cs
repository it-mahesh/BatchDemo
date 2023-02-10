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
using System.Text;

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
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private string CreateHash(string filePath)
        {
            // var message = Encoding.UTF8.GetBytes(strData);
            using (var fileStream = new FileStream(filePath,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.ReadWrite))
            {
                using (var alg = SHA512.Create())
                {
                    var hash = alg.ComputeHash(fileStream);
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
            ICollection<Attributes> attributes = new List<Attributes>
            {
            new Attributes{ Key="read-only", Value="true" },
            new Attributes{ Key="archive", Value="false" }
            };
            DirectoryInfo di = new(folderPath);
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
                    Attributes = attributes,
                    Hash = CreateHash(fi.FullName)
                });
            }
            return files;
        }
    }
}
