using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatchDemo.Models
{
    public class BatchInfo:Batch
    {
        //public Batch? Batch { get; set; }
        [JsonPropertyOrder(1)]
        public new Guid? BatchId { get; set; }
        [JsonPropertyOrder(2)]
        public string? Status { get; set; } = "InComplete";
        [JsonPropertyOrder(7)]
        public DateTime? BatchPublishedDate { get; set; } = DateTime.Today;
        [JsonPropertyOrder(8)]
        public ICollection<Files>? Files { get; set; }
    }
}
