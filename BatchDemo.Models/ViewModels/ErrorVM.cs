using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace BatchDemo.Models.ViewModels
{
    public class ErrorVM
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Path { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        
    }
}
