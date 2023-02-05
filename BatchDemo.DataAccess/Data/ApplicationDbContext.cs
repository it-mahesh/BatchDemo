using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BatchDemo.Models;
using System.Reflection.Emit;
using BatchDemo.DataAccess.Data;

namespace BatchDemo.DataAccess
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {

        }
        public DbSet<JsonDocument>? JsonDocument { get; set; }

        //public void MarkAsModified(JsonDocument item)
        //{
        //    Entry(item).State = EntityState.Modified;
        //}

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
        }

    }
}
