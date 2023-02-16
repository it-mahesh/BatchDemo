//using BatchDemo.Models.Enum;
namespace BatchDemo.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IKeyVaultManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDbConnectionFromAzureVault();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetStorageConnectionFromAzureVault();
    }
}
