using System.ComponentModel.DataAnnotations;

namespace Library.CustomDataAnnotations
{
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotNullAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var byteArray = value as byte[];

            if (byteArray == null || byteArray.Length == 0)
            {
                return new ValidationResult("Book cover is required.");
            }

            return ValidationResult.Success;
        }
    }
}
