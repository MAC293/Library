using System.ComponentModel.DataAnnotations;
using System;
using System.IO;
using System.Linq;
using Serilog;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Library.CustomDataAnnotations
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private static readonly String[] AllowedExtensions = { ".jpeg", ".jpg", ".png" };

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value is not String fileName)
            {
                return ValidationResult.Success;
            }

            var extension = Path.GetExtension(fileName);

            if (!AllowedExtensions.Contains(extension.ToLower()))
            {
                return new ValidationResult($"Please select a valid file extension: {String.Join(", ", AllowedExtensions)}");
            }

            return ValidationResult.Success;
        }
    }
}
