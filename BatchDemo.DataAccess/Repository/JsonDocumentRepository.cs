using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDemo.DataAccess.Repository
{
    public class JsonDocumentRepository : Repository<JsonDocument>, IJsonDocumentRepository
    {
        private ApplicationDbContext _db;

        public JsonDocumentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(JsonDocument obj)
        {
            var objFromDb = _db.JsonDocuments?.FirstOrDefault(u => u.BatchId == obj.BatchId);
            if (objFromDb != null)
            {
                objFromDb.Document = obj.Document;
            }
        }
    }
}
