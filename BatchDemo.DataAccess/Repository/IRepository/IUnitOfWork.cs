using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDemo.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IJsonDocumentRepository JsonDocument { get; }
        void Save();
        Task SaveAsync();
    }
}
