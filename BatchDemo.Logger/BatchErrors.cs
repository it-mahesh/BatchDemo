namespace BatchDemo.Logger
{
    public class BatchErrors
    {
        public string CorrelationId { get; }
        
        public IList<BatchError> Errors { get; }
        public BatchErrors(Exception exception)
        {
            CorrelationId = Guid.NewGuid().ToString();
            Errors = new List<BatchError>
            {
              new BatchError(exception.Source, exception.Message)
            };
        }

    }
    public class BatchError
    {
        public string? Source { get; set; }
        public string? Description { get; set; }
        public BatchError(string? source, string description)
        {
            Source = source != string.Empty ? source : null;
            Description = description;
        }
    }
}
