using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace SanjeshP.Mosahebe.Common.MyAttribute
{
    public class EnglishUsernameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string input = value.ToString();

            // عبارت منظم برای مجاز کردن فقط حروف انگلیسی و اعداد
            string pattern = @"^[a-zA-Z0-9]*$";

            if (Regex.IsMatch(input, pattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "نام کاربری باید فقط شامل حروف انگلیسی و اعداد باشد.");
        }
    }

}
