using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace SanjeshP.Mosahebe.Common.MyAttribute
{
    public class OnlyNumberValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string input = value.ToString();

            // عبارت منظم برای مجاز کردن فقط حروف انگلیسی و اعداد
            string pattern = @"^[0-9]*$";

            if (Regex.IsMatch(input, pattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "فیلد عددی صرفا بایستی اعداد صحیح بین 0 تا 9 باشد.");
        }
    }
}
