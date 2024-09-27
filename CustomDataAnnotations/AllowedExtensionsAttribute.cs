using System.ComponentModel.DataAnnotations;
using System;
using System.IO;
using System.Linq;
using Serilog;

namespace Library.CustomDataAnnotations
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private static readonly string[] AllowedExtensions = { ".jpeg", ".jpg", ".png" };

        protected override ValidationResult? IsValid(Object value, ValidationContext validationContext)
        {
            var fileBytes = value as Byte[];
            Log.Information("fileBytes: {@fileBytes}", fileBytes);

            if (fileBytes != null)
            {
                var fileName = validationContext.DisplayName;
                Log.Information("fileName: {@fileName}", fileName);

                if (!String.IsNullOrEmpty(fileName))
                {
                    var extension = Path.GetExtension(fileName);
                    Log.Information("extension: {@extension}", extension);

                    if (!AllowedExtensions.Contains(extension.ToLower()))
                    {
                        return new ValidationResult("Select the allowed file extensions: .jpg, .jpeg, .png");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
