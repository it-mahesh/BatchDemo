using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDemo.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            JsonDocument = new JsonDocumentRepository(_db);
            Files = new FilesRepository(_db);
        }

        public IJsonDocumentRepository JsonDocument { get; private set; }
        public IFilesRepository Files { get; private set; }
        public void Save()
        {
            _db.SaveChanges();
        }
        public async Task SaveAsync()
        {
           await _db.SaveChangesAsync();
        }
    }
}
