using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDemo.Models;
namespace BatchDemo.UnitTests
{
    public class DemoBatchData
    {
        public Batch LoadBatch()
        {
            AccessControlLayer accessControlLayer = new AccessControlLayer();
            accessControlLayer.ReadGroups = new List<string> { "Group 1", "Group 2" };
            accessControlLayer.ReadUsers = new List<string> { "User 1", "User 2" };

            ICollection<Attributes> attributes = new List<Attributes>
           {
           new Attributes{ Key= "AttKey1", Value = "Attribute Key 1" },
           new Attributes{ Key= "AttKey2", Value = "Attribute Key 2" }
           };

            Batch batch = new Batch();
            batch.BusinessUnit = "Demo";
            batch.ExpiryDate = DateTime.Now;
            batch.ACL = accessControlLayer;
            batch.Attributes = attributes;
            batch.BatchId = new Guid("19330cdc-cb38-4989-abbe-acde7359a24b");
            return batch;
        }
    }
}
