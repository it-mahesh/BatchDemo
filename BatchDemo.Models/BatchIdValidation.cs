using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDemo.Models
{
    public class BatchIdValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Guid guidResult;

            if (Guid.TryParse(value.ToString(), out guidResult))
            {
                return ValidationResult.Success;
            }
            else
            {
               return new ValidationResult("BatchId should be in valid GUID format.");
            }

        }
    }
}
