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
        [StringLength(6, MinimumLength = 6, ErrorMessage = "کد رهگیری باید ۶ کاراکتر باشد")]
        [Display(Name = "کد رهگیری")]
        public string CodeRahgiri { get; set; }
    }
}
