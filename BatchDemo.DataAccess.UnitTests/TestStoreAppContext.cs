using System;
using System.Data.Entity;
using BatchDemo.Models;

namespace BatchDemo.DataAccess.UnitTests
{
    public class TestApplicationDbContext : DbContext
    {
        public TestApplicationDbContext()
        {
            this.Batches = new TestBatchDbSet();
        }

        public DbSet<Batch> Batches { get; set; }

        public int SaveChanges()
        {
            return 0;
        }

        public void MarkAsModified(Batch item) { }
        public void Dispose() { }
    }
}
