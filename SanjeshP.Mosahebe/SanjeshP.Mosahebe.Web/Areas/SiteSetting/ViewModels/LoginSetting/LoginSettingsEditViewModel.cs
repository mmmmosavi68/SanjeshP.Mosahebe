using SanjeshP.Mosahebe.Entities.SiteSetting;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.SiteSetting.ViewModels.LoginSetting
{
    public class LoginSettingsEditViewModel : BaseDto<LoginSettingsEditViewModel, LoginSettings, Guid>
    {
        public Guid LoginSettingId { get; set; }

        [Display(Name = "فعال بودن کپچا")]
        public bool IsCaptchaEnabled { get; set; }

        [Display(Name = "فعال بودن ورود با OTP")]
        public bool IsOtpLoginEnabled { get; set; }

        [Display(Name = "فعال بودن ورود با نام کاربری و رمز عبور")]
        public bool IsUsernamePasswordLoginEnabled { get; set; }

        [Display(Name = "فعال بودن ورود دومرحله‌ای")]
        public bool IsTwoFactorEnabled { get; set; }

        [Required(ErrorMessage = "وارد کردن مدت اعتبار کد OTP الزامی است")]
        [Range(30, 600, ErrorMessage = "مدت اعتبار کد OTP باید بین ۳۰ تا ۶۰۰ ثانیه باشد")]
        [Display(Name = "مدت اعتبار کد OTP (ثانیه)")]
        public int OtpExpirySeconds { get; set; }

        [Required(ErrorMessage = "وارد کردن حداکثر دفعات مجاز ورود اشتباه الزامی است")]
        [Range(1, 10, ErrorMessage = "حداکثر دفعات مجاز ورود اشتباه باید بین ۱ تا ۱۰ باشد")]
        [Display(Name = "حداکثر دفعات مجاز ورود اشتباه")]
        public int MaxFailedLoginAttempts { get; set; }
    }
}
