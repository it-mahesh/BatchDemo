using BatchDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDemo.DataAccess.Repository.IRepository
{
    public interface IJsonDocumentRepository : IRepository<JsonDocument>
    {
        void Update(JsonDocument obj);
    }
}
