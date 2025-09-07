using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Student.ViewModels
{
    public class LoginOTPViewModel
    {
        [Required(ErrorMessage = "کد ملی الزامی است")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "کد ملی باید ۱۰ رقم باشد")]
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

    }
}
