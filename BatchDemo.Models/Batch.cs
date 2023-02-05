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
        [BatchIdValidation]
        public Guid BatchId { get; set; }
     
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

    // Batch myDeserializedClass = JsonConvert.DeserializeObject<Batch>(myJsonResponse);
    /*
                 //Jsontochsharp.com
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://Your Json Source");
            request.Method = "GET"; request.Accept = "application/json;odata=verbose"; request.ContentType = "application/json;odata=verbose";
            WebResponse response = request.GetResponse(); string jSON = null; using (response)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    try
                    {
                        jSON = reader.ReadToEnd();
                    }
                    catch (Exception ex) { }
                }
            }

            // using javascript serializer
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            RootObject articleData = new RootObject();
            articleData = serializer.Deserialize<RootObject>(jSON);
     
     */

    //public class BatchData
    //{
    //    public Batch DataOperations()
    //    {
    //        AccessControlLayer accessControlLayer = new AccessControlLayer();
    //        accessControlLayer.ReadGroups = new List<string> { "Group 1", "Group 2" };
    //        accessControlLayer.ReadUsers = new List<string> { "User 1", "User 2" };

    //        ICollection<Attributes> attributes = new List<Attributes>
    //       {
    //       new Attributes{ Key= "AttKey1", Value = "Attribute Key 1" },
    //       new Attributes{ Key= "AttKey2", Value = "Attribute Key 2" }
    //       };

    //        Batch batch = new Batch();
    //        batch.BusinessUnit = "Demo";
    //        batch.ExpiryDate = DateTime.Now;
    //        batch.ACL = accessControlLayer;
    //        batch.Attributes = attributes;

    //        return batch;
    //    }
    //}
}
