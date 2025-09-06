using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels
{
    public class VerifyOtpViewModel
    {
        [Display(Name = "کد اعتبار سنجی")]
        [Required(ErrorMessage = "وارد کردن کد الزامی است")]
        [MaxLength(6, ErrorMessage = "کد نباید بیش از ۶ رقم باشد")]
        public string Code { get; set; }
    }

}
