using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDemo.Models
{
    public class ValidationError
    {
        public string? Source { get; set; }
        public string? Description { get; set; }
        public ValidationError(string source, string description)
        {
            Source = source != string.Empty ? source : null;
            Description = description;
        }
    }
    public class ValidationResultModel
    {
        public string CorrelationId { get; }
        public IList<ValidationError> Errors { get; }

        public ValidationResultModel(ModelStateDictionary modelState)
        {
            //_correlationIdGenerator = new CorrelationIdGenerator();
            CorrelationId = Guid.NewGuid().ToString(); 
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key]!.Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                    .ToList();
        }
    }
    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary modelState)
            : base(new ValidationResultModel(modelState))
        {
            // StatusCode = StatusCodes.Status422UnprocessableEntity;
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
