using System.ComponentModel.DataAnnotations;

namespace Library.CustomDataAnnotations
{
    public class SizeAttribute : ValidationAttribute
    {
        private const int MinFileSize = 1 * 1024; //1KB
        private const int MaxFileSize = 1 * 1024 * 1024; //1MB in bytes

        protected override ValidationResult? IsValid(Object? value, ValidationContext validationContext)
        {
            var file = value as Byte[];

            if (file != null)
            {
                if (file.Length < MinFileSize)
                {
                    return new ValidationResult($"Minimum allowed file size is {MinFileSize / 1024} KB.");
                }

                if (file.Length > MaxFileSize)
                {
                    return new ValidationResult($"Maximum allowed file size is {MaxFileSize / (1024 * 1024)} MB.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
