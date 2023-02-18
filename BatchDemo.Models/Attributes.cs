using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace BatchDemo.Models
{
    public class Attributes
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public int Id { get; set; }
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
