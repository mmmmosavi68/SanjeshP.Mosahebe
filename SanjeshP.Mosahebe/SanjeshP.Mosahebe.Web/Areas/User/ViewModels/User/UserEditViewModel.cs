using SanjeshP.Mosahebe.Common.MyAttribute;
using System;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.User.ViewModels.User
{
    public class UserEditViewModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "{0} را وارد نمایید.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "{0} باید بین 5 تا 50 کاراکتر باشد.")]
        [PersianEnglishInputValidator(ErrorMessage = "لطفاً فقط از حروف فارسی، انگلیسی، اعداد و کاراکترهای مجاز استفاده کنید.")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} را وارد نمایید.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "{0} باید بین 5 تا 50 کاراکتر باشد.")]
        [PersianEnglishInputValidator(ErrorMessage = "لطفاً فقط از حروف فارسی، انگلیسی، اعداد و کاراکترهای مجاز استفاده کنید.")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [MaxLength(10, ErrorMessage = "{0} حداکثر {1} کاراکتر میباشد.")]
        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Text)]
        [NationalCode("لطفا کد ملی را بدرستی وارد کنید")]
        public string NationalCode { get; set; }

        [MaxLength(11, ErrorMessage = "{0} حداکثر {1} کاراکتر میباشد.")]
        [Display(Name = "شماره همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "{0} را وارد نمایید.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "{0} باید بین 5 تا 50 کاراکتر باشد.")]
        [EnglishUsername(ErrorMessage = "نام کاربری فقط می‌تواند شامل حروف انگلیسی و اعداد باشد.")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} را وارد نمایید.")]
        [EmailAddress(ErrorMessage = "لطفاً یک {0} معتبر وارد نمایید.")]
        [Display(Name = "ایمیل")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "{0} را وارد نمایید.")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "{0} باید حداقل 8 کاراکتر باشد.")]
        [PasswordValidation(IsOptional = true, ErrorMessage = "کلمه عبور باید حداقل ۸ کاراکتر باشد و شامل یک حرف بزرگ، یک حرف کوچک، یک عدد و یک کاراکتر خاص باشد.")]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [Display(Name = "نوع کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int? RoleId { get; set; }

        [Display(Name = "وضعیت ( فعال/غیرفعال)")]
        public bool IsActive { get; set; }
    }

}
