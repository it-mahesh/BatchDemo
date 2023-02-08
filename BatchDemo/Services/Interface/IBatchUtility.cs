using BatchDemo.Models;

namespace BatchDemo.Services.Interface
{
    public interface IBatchUtility
    {
        public JsonDocument GetJsonByBatchId(Guid batchId);
        public Batch DeserializeJsonDocument(Guid batchId);
        public BatchInfo BatchToBatchInfoConverter(Batch batch);
    }
}
