using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [MaxLength(50, ErrorMessage = "نام کاربری نباید بیش از 50 کاراکتر باشد")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        [ValidateNever]  // این باعث میشه اعتبارسنجی روی این پراپرتی اجرا نشه
        [Display(Name = "کد امنیتی")]
        public string CaptchaCode { get; set; }


    }

}
