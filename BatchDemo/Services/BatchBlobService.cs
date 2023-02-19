using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
//using BatchDemo.Models.Enum;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BatchDemo.Services.Interface;
using System.Diagnostics.CodeAnalysis;

namespace BatchDemo.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class BatchBlobService : IBatchBlobService
    {
        private readonly IConfiguration _configuration;
        private readonly IKeyVaultManager _keyVaultManager;
        // private static string? _azureStorageConnection;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="keyVaultManager"></param>
        public BatchBlobService(IConfiguration configuration, IKeyVaultManager keyVaultManager)
        {
            _configuration = configuration;
            _keyVaultManager = keyVaultManager;
            // Get storage account connection from local config file.
            // _azureStorageConnection = _configuration.GetSection("AzureSettings:StorageAccount").Value;
        }
        /// <summary>
        /// Create a container
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public BlobContainerClient? CreateContainer(string containerName)
        {
            // Create the container
            string storageConnectionKV = _keyVaultManager.GetStorageConnectionFromAzureVault();
            //BlobServiceClient blobServiceClient = new BlobServiceClient(_azureStorageConnection);
            BlobServiceClient blobServiceClient = new(storageConnectionKV);

            // My Issue - Facing difficulty to unit test below line. Also can't create interface to invert the dependency.
            // CreateContainer_ReturnsValue() tried.
            //BlobContainerClient container = blobServiceClient.CreateBlobContainer(containerName);
            return blobServiceClient.CreateBlobContainer(containerName);
        }
        /// <summary>
        /// Delete a container
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="storageConnectionKV"></param>
        /// <returns></returns>
        public bool DeleteContainer(string containerName,string storageConnectionKV)
        {
            BlobServiceClient blobServiceClient = new(storageConnectionKV);
            blobServiceClient.DeleteBlobContainer(containerName);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="filePath"></param>
        /// <param name="mimeType"></param>
        /// <param name="contentSize"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public string PostFile(string containerName, string filePath, string mimeType,string contentSize)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);

                BlobHttpHeaders blobHttpHeaders = new()
                {
                    ContentType = mimeType
                };
                
                var fileUrl = "";
                string storageConnectionKV = _keyVaultManager.GetStorageConnectionFromAzureVault();
                BlobContainerClient containerClient = new(storageConnectionKV, containerName);
                //BlobContainerClient containerClient = new BlobContainerClient(_azureStorageConnection, containerName);
                //ListBlobsFlatListing(containerClient,100);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                using FileStream fileStream = File.OpenRead(filePath); 
                    blobClient.Upload(fileStream, blobHttpHeaders);
                //await blobClient.SetHttpHeadersAsync(blobHttpHeaders);
                //fileStream.Close();
                fileUrl = blobClient.Uri.AbsoluteUri;
                return fileUrl; 
            }
            catch (Exception)
            {
                throw;
            }
        }
        [ExcludeFromCodeCoverage]
        private static async Task ListBlobsFlatListing(BlobContainerClient blobContainerClient, int? segmentSize)
        {
            try
            {
                // Call the listing operation and return pages of the specified size.
                var resultSegment = blobContainerClient.GetBlobsAsync()
                    .AsPages(default, segmentSize);

                // Enumerate the blobs returned for each page.
                await foreach (Page<BlobItem> blobPage in resultSegment)
                {
                    foreach (BlobItem blobItem in blobPage.Values)
                    {
                        Console.WriteLine("Blob name: {0}", blobItem.Name);
                    }
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
