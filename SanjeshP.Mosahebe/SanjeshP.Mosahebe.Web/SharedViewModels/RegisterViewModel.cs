using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.SharedViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "لطفاً نام کاربری را وارد کنید.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "لطفاً ایمیل را وارد کنید.")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "لطفاً رمز عبور را وارد کنید.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "لطفاً تکرار رمز عبور را وارد کنید.")]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن مطابقت ندارند.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Phone(ErrorMessage = "شماره تلفن معتبر نیست.")]
        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "لطفاً کد ملی را وارد کنید.")]
        public string NationalCode { get; set; }
    }

}
