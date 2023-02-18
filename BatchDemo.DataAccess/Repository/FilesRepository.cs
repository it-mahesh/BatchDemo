using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDemo.DataAccess.Repository
{
    public class FilesRepository : Repository<Files>, IFilesRepository
    {
        private ApplicationDbContext _db;

        public FilesRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Files obj)
        {
            var objFromDb = _db.Files?.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.FileName = obj.FileName;
                objFromDb.MimeType = obj.MimeType;
            }
        }
    }
}
