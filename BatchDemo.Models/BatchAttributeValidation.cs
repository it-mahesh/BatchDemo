using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDemo.Models
{
    public class BatchAttributeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            //var batch = (Batch)validationContext.ObjectInstance;
            List<string> lsterrors = new();
            IList<Attributes> attributes; 
            if (value==null)
            {
                attributes = new List<Attributes>();
            }
            else
            {
                attributes = (List<Attributes>)value;
            }
            
            // For attributes if present on request should have both key and value if not it should respond bad request.
            foreach (var attribute in attributes)
            {
                if (string.IsNullOrWhiteSpace(attribute.Key) || string.IsNullOrWhiteSpace(attribute.Value))
                {
                    lsterrors.Add("Attributes should have both key and value present.");
                    //new ValidationResult("Attributes should have both key and value present.");
                }
            }
            if (lsterrors.Count > 0)
            {
                ValidationResult validationResult = new("");
                foreach (var error in lsterrors)
                {
                    validationResult.ErrorMessage +=" "+ error;
                }
                return validationResult;
            }
            else
            {
                return ValidationResult.Success!;
            }
        }
    }
}
