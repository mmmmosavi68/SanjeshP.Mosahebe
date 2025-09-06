using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace SanjeshP.Mosahebe.Common.MyAttribute
{
    public class PersianEnglishInputValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string input = value.ToString();

            // عبارت منظم برای مجاز کردن حروف فارسی، انگلیسی، اعداد و کاراکترهای خاص
            string pattern = @"^[a-zA-Z\u0600-\u06FF0-9@#$%^&*()_+=\-{}
                                \[\]
                                :;\""<>,.?/!\\s]*$";

            if (Regex.IsMatch(input, pattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "مقدار وارد شده نامعتبر است.");
        }

    }
}
