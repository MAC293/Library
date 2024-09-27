using System.ComponentModel.DataAnnotations;

namespace Library.CustomDataAnnotations
{
    public class SizeAttribute : ValidationAttribute
    {
        private const int MinFileSize = 10 * 1024; //10 KB in bytes
        private const int MaxFileSize = 1000000; //1 MB in bytes

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
                    return new ValidationResult($"Maximum allowed file size is {MaxFileSize / 1024} MB.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
