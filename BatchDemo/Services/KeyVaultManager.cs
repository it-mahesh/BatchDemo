using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BatchDemo.Services.Interface;
using System.Diagnostics.CodeAnalysis;

namespace BatchDemo.Services
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly IConfiguration _configuration;
        string? tenantId;
        string? clientId;
        string? clientSecret;
        string? keyVaultUrl;
        string? secretName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public KeyVaultManager(IConfiguration configuration)
        {
            _configuration = configuration;

            tenantId = _configuration.GetSection("KeyVaultConfig:TenantId").Value;
            clientId = _configuration.GetSection("KeyVaultConfig:ClientId").Value;
            clientSecret = _configuration.GetSection("KeyVaultConfig:ClientSecretId").Value;
            keyVaultUrl = _configuration.GetSection("KeyVaultConfig:KVUrl").Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDbConnectionFromAzureVault()
        {
            ClientSecretCredential clientSecretCredential = new(tenantId, clientId, clientSecret);
            SecretClient secretClient = new(new Uri(keyVaultUrl!), clientSecretCredential);
            secretName = _configuration.GetSection("KeyVaultConfig:DbConnSecretName").Value;
            var secret = secretClient.GetSecret(secretName);
            return secret.Value.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetStorageConnectionFromAzureVault()
        {
            ClientSecretCredential clientSecretCredential = new(tenantId, clientId, clientSecret);
            SecretClient secretClient = new(new Uri(keyVaultUrl!), clientSecretCredential);
            secretName = _configuration.GetSection("KeyVaultConfig:StorageSecretName").Value;
            var secret = secretClient.GetSecret(secretName);
            return secret.Value.Value;
        }

    }
}
