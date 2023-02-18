using BatchDemo.Models;
namespace BatchDemo.UnitTests
{
    public class DemoBatchData
    {
        public static Batch LoadBatch()
        {
            AccessControlLayer accessControlLayer = new AccessControlLayer();
            accessControlLayer.ReadGroups = new List<string> { "Group 1", "Group 2" };
            accessControlLayer.ReadUsers = new List<string> { "User 1", "User 2" };

            ICollection<Attributes> attributes = new List<Attributes>
           {
           new Attributes{ Key= "size", Value = "200mb" },
           new Attributes{ Key= "type", Value = "json" }
           };

            Batch batch = new Batch();
            batch.BusinessUnit = "Mastek";
            batch.ExpiryDate = DateTime.Now;
            batch.ACL = accessControlLayer;
            batch.Attributes = attributes;
            batch.BatchId = new Guid("D53C237C-4383-4D44-8DF5-DD46B06E575B");
            return batch;
        }
        public static Guid GetDummyBatchId()
        {
          return  new Guid("D53C237C-4383-4D44-8DF5-DD46B06E575B");
        }
    }
}
