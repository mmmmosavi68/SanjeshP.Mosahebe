using System;
using System.Globalization;

namespace SanjeshP.Mosahebe.Convertor
{
    public static class DateConvertor
    {
        //۱. تبدیل تاریخ میلادی با ساعت به شمسی
        public static string ToShamsiDateTime(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            return $"{pc.GetYear(value)}/{pc.GetMonth(value):00}/{pc.GetDayOfMonth(value):00} {value.Hour:00}:{value.Minute:00}:{value.Second:00}";
        }
        //۳. تبدیل تاریخ میلادی بدون ساعت به شمسی
        public static string ToShamsiDate(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            return $"{pc.GetYear(value)}/{pc.GetMonth(value):00}/{pc.GetDayOfMonth(value):00}";
        }

        //۲. تبدیل تاریخ شمسی با ساعت به میلادی
        public static DateTime FromShamsiDateTime(this string persianDateTime)
        {
            var parts = persianDateTime.Split(' ');
            if (parts.Length != 2) throw new FormatException("Invalid Persian date and time format.");

            var dateParts = parts[0].Split('/');
            var timeParts = parts[1].Split(':');
            if (dateParts.Length != 3 || timeParts.Length != 3) throw new FormatException("Invalid format.");

            var year = int.Parse(dateParts[0]);
            var month = int.Parse(dateParts[1]);
            var day = int.Parse(dateParts[2]);
            var hour = int.Parse(timeParts[0]);
            var minute = int.Parse(timeParts[1]);
            var second = int.Parse(timeParts[2]);

            PersianCalendar pc = new PersianCalendar();
            return pc.ToDateTime(year, month, day, hour, minute, second, 0);
        }
        //۴. تبدیل تاریخ شمسی بدون ساعت به میلادی
        public static DateTime FromShamsiDate(this string persianDate)
        {
            var parts = persianDate.Split('/');
            if (parts.Length != 3) throw new FormatException("Invalid Persian date format.");

            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var day = int.Parse(parts[2]);

            PersianCalendar pc = new PersianCalendar();
            return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
        }

    }
}
