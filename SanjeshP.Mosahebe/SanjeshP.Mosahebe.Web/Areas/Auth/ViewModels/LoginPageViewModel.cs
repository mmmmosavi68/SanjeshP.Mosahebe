using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SanjeshP.Mosahebe.Entities.SiteSetting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels
{
    public class LoginPageViewModel : IValidatableObject
    {
        public LoginViewModel Login { get; set; } = new LoginViewModel();

        [ValidateNever]  // این باعث میشه اعتبارسنجی روی این پراپرتی اجرا نشه
        public LoginSettings Settings { get; set; } = new LoginSettings();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // فقط اعتبارسنجی روی Login انجام بده
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(Login, new ValidationContext(Login), results, true);
            return results;
        }
    }

}
