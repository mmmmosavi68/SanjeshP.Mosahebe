using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "لطفاً کد کاربری را وارد کنید.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "لطفاً کد تایید را وارد کنید.")]
        public string VerificationCode { get; set; }

        [Required(ErrorMessage = "لطفاً رمز عبور جدید را وارد کنید.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "لطفاً تکرار رمز عبور جدید را وارد کنید.")]
        [Compare("NewPassword", ErrorMessage = "رمز عبور جدید و تکرار آن مطابقت ندارند.")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }

}
