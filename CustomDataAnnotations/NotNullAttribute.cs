using Serilog;
using System.ComponentModel.DataAnnotations;

namespace Library.CustomDataAnnotations
{
    public class NotNullAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var byteArray = value as byte[];

            //Log.Information("byteArray: {@byteArray}", byteArray);

            if (byteArray == null || byteArray.Length == 0)
            {
                return new ValidationResult("Book cover is required.");
            }

            return ValidationResult.Success;
        }
    }
}
