using System.Text.Json.Serialization;
namespace BatchDemo.Models
{
    public class Files
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public Guid? BatchId { get; set; }
        public string FileName { get; set; } = "string";
        public long FileSize { get; set; } = 0;
        public string MimeType { get; set; } = "string";
        public string Hash { get; set; } = "string";
        public ICollection<Attributes>? Attributes { get; set; }
    }
}
