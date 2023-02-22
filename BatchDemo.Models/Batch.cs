// using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BatchDemo.Models
{
    /// <summary>
    /// Create a new batch to upload the files into and get files from 
    /// </summary>
    public class Batch
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public Guid? BatchId { get; set; }
     
        [Required(ErrorMessage = "BusinessUnit can't be empty.")]
        [JsonPropertyOrder(3)]
        public string? BusinessUnit { get; set; }
        [JsonPropertyOrder(4)]
        
        public AccessControlLayer? ACL { get; set; }
        [BatchAttributeValidation]
        [JsonPropertyOrder(5)]
        public ICollection<Attributes>? Attributes { get; set; }
        
        [JsonPropertyOrder(6)]
        public DateTime? ExpiryDate { get; set; }
    }
}
