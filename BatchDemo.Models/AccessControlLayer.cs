using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDemo.Models
{
    public class AccessControlLayer
    {
        public ICollection<string>? ReadUsers { get; set; }
        public ICollection<string>? ReadGroups { get; set; }
    }
}
