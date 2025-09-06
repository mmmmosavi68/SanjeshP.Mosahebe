using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels
{
    public class TwoFactorViewModel
    {
        [Required(ErrorMessage = "لطفاً کد کاربری را وارد کنید.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "لطفاً کد تایید را وارد کنید.")]
        public string VerificationCode { get; set; }
    }

}
