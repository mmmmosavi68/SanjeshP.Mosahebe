using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Student.ViewModels
{
    public class StudentTwoFactorAuthenticationViewModel
    {
        [Required(ErrorMessage = "کد دو مرحله‌ای الزامی است")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "کد باید ۶ رقم باشد")]
        [Display(Name = "کد دو مرحله‌ای")]
        public string TwoFactorCode { get; set; }
    }
}
