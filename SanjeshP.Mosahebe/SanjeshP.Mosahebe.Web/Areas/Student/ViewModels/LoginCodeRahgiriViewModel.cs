using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Student.ViewModels
{
    public class LoginCodeRahgiriViewModel
    {
        [Required(ErrorMessage = "کد ملی الزامی است")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "کد ملی باید ۱۰ رقم باشد")]
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Required(ErrorMessage = "کد رهگیری الزامی است")]
        [StringLength(16, MinimumLength = 4, ErrorMessage = "طول کد رهگیری نامعتبر است")]
        [Display(Name = "کد رهگیری")]
        public string CodeRahgiri { get; set; }

        [Required(ErrorMessage = "کد امنیتی الزامی است")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "کد امنیتی باید ۶ رقم باشد")]
        [Display(Name = "کد امنیتی")]
        public string CaptchaCode { get; set; }
    }
}
