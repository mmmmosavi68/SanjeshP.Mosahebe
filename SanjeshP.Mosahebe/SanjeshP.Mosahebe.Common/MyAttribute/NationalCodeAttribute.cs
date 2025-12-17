using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SanjeshP.Mosahebe.Common.MyAttribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NationalCodeAttribute : ValidationAttribute
    {
        public NationalCodeAttribute(string ErrorMessage)
              : base()
        {
            this.ErrorMessage = ErrorMessage;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (IsValidNationalCode(value.ToString()) == false) return false;
            return true;
        }

        public bool IsValidNationalCode1(string nationalcode)
        {
            if (string.IsNullOrEmpty(nationalcode)) return false;
            if (!new Regex(@"\d{10}").IsMatch(nationalcode)) return false;
            var array = nationalcode.ToCharArray();
            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(nationalcode)) return false;
            var j = 10;
            var sum = 0;
            for (var i = 0; i < array.Length - 1; i++)
            {
                sum += Int32.Parse(array[i].ToString(CultureInfo.InvariantCulture)) * j;
                j--;
            }
            var div = sum / 11;
            var r = div * 11;
            var diff = Math.Abs(sum - r);
            if (diff <= 2)
            {
                return diff == Int32.Parse(array[9].ToString(CultureInfo.InvariantCulture));
            }
            var temp = Math.Abs(diff - 11);
            return temp == Int32.Parse(array[9].ToString(CultureInfo.InvariantCulture));
        }

        public bool IsValidNationalCode(string nationalcode)
        {
            if (string.IsNullOrEmpty(nationalcode)) return false;

            // ۱. بررسی طول ۱۰ رقم و فقط عدد بودن
            if (!Regex.IsMatch(nationalcode, @"^\d{10}$")) return false;

            // ۲. بررسی تکراری نبودن همه ارقام (مثل 1111111111)
            if (Regex.IsMatch(nationalcode, @"^(\d)\1{9}$")) return false;

            var chars = nationalcode.ToCharArray();

            // ۳. محاسبه مجموع حاصلضرب ارقام در موقعیت آن‌ها
            var sum = 0;
            for (var i = 0; i < 9; i++)
            {
                sum += (chars[i] - '0') * (10 - i);
            }

            // ۴. به دست آوردن باقیمانده تقسیم بر ۱۱
            var remainder = sum % 11;

            // ۵. رقم کنترل (رقم دهم)
            var controlDigit = chars[9] - '0';

            // ۶. شرط نهایی طبق استاندارد ثبت احوال
            if (remainder < 2)
            {
                return controlDigit == remainder;
            }
            else
            {
                return controlDigit == (11 - remainder);
            }
        }
    }
}
