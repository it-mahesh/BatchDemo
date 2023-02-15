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
using Azure;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs.Models;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Azure.Security.KeyVault.Secrets;

namespace BatchDemo.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class BatchBlobService : IBatchBlobService
    {
        private readonly IConfiguration _configuration;
        // private static string? _azureStorageConnection;

        private string GetAzureStorageKeyVaultConnection()
        {
            string? tenantId = _configuration.GetSection("KeyVaultConfig:TenantId").Value;
            string? clientId = _configuration.GetSection("KeyVaultConfig:ClientId").Value;
            string? clientSecret = _configuration.GetSection("KeyVaultConfig:ClientSecretId").Value;
            
            ClientSecretCredential clientSecretCredential=new(tenantId, clientId, clientSecret);

            string? keyVaultUrl = _configuration.GetSection("KeyVaultConfig:KVUrl").Value;
            string? secretName = _configuration.GetSection("KeyVaultConfig:SecretName").Value; 
            SecretClient secretClient = new(new Uri(keyVaultUrl!),clientSecretCredential);

            var secret = secretClient.GetSecret(secretName);

            return secret.Value.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public BatchBlobService(IConfiguration configuration)
        {
           _configuration = configuration;
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
            string storageConnectionKV = GetAzureStorageKeyVaultConnection();
            //BlobServiceClient blobServiceClient = new BlobServiceClient(_azureStorageConnection);
            BlobServiceClient blobServiceClient = new(storageConnectionKV);

            BlobContainerClient container = blobServiceClient.CreateBlobContainer(containerName);

                if (container.Exists())
                {
                    return container;
                }            
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="filePath"></param>
        /// <param name="mimeType"></param>
        /// <param name="contentSize"></param>
        /// <returns></returns>
        public async Task<string> PostFileAsync(string containerName, string filePath, string mimeType,string contentSize)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);

                BlobHttpHeaders blobHttpHeaders = new()
                {
                    ContentType = mimeType
                };
                
                var fileUrl = "";
                string storageConnectionKV = GetAzureStorageKeyVaultConnection();
                BlobContainerClient containerClient = new(storageConnectionKV, containerName);
                //BlobContainerClient containerClient = new BlobContainerClient(_azureStorageConnection, containerName);
                //ListBlobsFlatListing(containerClient,100);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                using FileStream fileStream = File.OpenRead(filePath); 
                    await blobClient.UploadAsync(fileStream, blobHttpHeaders);
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
