using Serilog;
using System.ComponentModel.DataAnnotations;

namespace Library.CustomDataAnnotations
{
    public class NotNullAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            var byteArray = value as Byte[];

            if (byteArray == null)
            {
                return new ValidationResult("Book cover is required.");
            }

            return ValidationResult.Success;
        }
    }
}
