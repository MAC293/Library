﻿using System.ComponentModel.DataAnnotations;
using System;
using System.IO;
using System.Linq;

namespace Library.CustomDataAnnotations
{
    public class AllowedExtensionsAttribute: ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(Object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);

                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult($"This image file extension is not allowed!");
                }
            }
            return ValidationResult.Success;
        }


    }
}
