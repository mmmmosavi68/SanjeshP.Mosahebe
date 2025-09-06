using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.SiteSetting
{
    public class LoginSettings : BaseEntity
    {
        // ورود با کپچا فعال باشد؟
        public bool IsCaptchaEnabled { get; set; }

        // ورود فقط با OTP (بدون رمز)
        public bool IsOtpLoginEnabled { get; set; }

        // ورود با نام کاربری و رمز عبور
        public bool IsUsernamePasswordLoginEnabled { get; set; }

        // ورود دومرحله‌ای (مثلاً رمز + کد پیامک)
        public bool IsTwoFactorEnabled { get; set; }

        // مدت اعتبار OTP بر حسب ثانیه
        public int OtpExpirySeconds { get; set; }

        // حداکثر دفعات مجاز ورود اشتباه
        public int MaxFailedLoginAttempts { get; set; }

        [Display(Name = "مدت زمان انقضای نشست (دقیقه)")]
        [Range(1, 1440, ErrorMessage = "مدت زمان باید بین ۱ تا ۱۴۴۰ دقیقه (۲۴ ساعت) باشد")]
        public int SessionExpiryTimeMinutes { get; set; }
    }
}
