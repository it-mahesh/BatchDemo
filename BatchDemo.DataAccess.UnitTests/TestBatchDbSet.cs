using System;
using System.Linq;
using BatchDemo.Models;

namespace BatchDemo.DataAccess.UnitTests
{
    class TestBatchDbSet : TestDbSet<Batch>
    {
        public override Batch Find(params object[] keyValues)
        {
            return this.SingleOrDefault(batch => batch.BatchId == (Guid)keyValues.Single());
        }
    }
}
