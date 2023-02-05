using Microsoft.EntityFrameworkCore;
using BatchDemo.Models;

namespace BatchDemo.DataAccess.Data
{
   public interface IApplicationDbContext:IDisposable
    {
        DbSet<JsonDocument> JsonDocument { get; }
        int SaveChanges();
       // void MarkAsModified(JsonDocument item);
    }
}
