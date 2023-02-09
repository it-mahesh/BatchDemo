using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDemo.Models
{
    public class Files
    {
        public string FileName { get; set; } = "string";
        public long FileSize { get; set; } = 0;
        public string MimeType { get; set; } = "string";
        public string Hash { get; set; } = "string";
        public ICollection<Attributes>? Attributes { get; set; }
    }
}
