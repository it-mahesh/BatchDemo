using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BatchDemo.Models;
using System.Reflection.Emit;

namespace BatchDemo.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {

        }
        public DbSet<JsonDocument>? JsonDocuments { get; set; }
        public DbSet<Files>? Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            //  base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<JsonDocument>().HasKey(a => a.BatchId);
            modelBuilder.Entity<JsonDocument>().Property(a => a.Document).IsRequired();
        }

    }
}
