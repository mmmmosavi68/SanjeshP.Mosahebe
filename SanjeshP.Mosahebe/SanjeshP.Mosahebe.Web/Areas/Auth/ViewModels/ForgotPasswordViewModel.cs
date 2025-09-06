using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "لطفاً ایمیل یا شماره تلفن را وارد کنید.")]
        public string EmailOrPhoneNumber { get; set; }
    }

}
