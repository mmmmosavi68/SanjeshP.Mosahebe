using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace SanjeshP.Mosahebe.Common.MyAttribute
{
    public class PasswordValidationAttribute : ValidationAttribute
    {

        public bool IsOptional { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || IsOptional)
            {
                return ValidationResult.Success;
            }

            string input = value.ToString();

            // عبارت منظم برای اعتبارسنجی (در اینجا مثال برای کلمه عبور است)
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=!]).{8,}$";

            if (Regex.IsMatch(input, pattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "مقدار وارد شده نامعتبر است.");


        }
    }
}