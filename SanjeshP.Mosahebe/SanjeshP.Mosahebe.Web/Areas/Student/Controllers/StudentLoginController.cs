using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;

namespace SanjeshP.Mosahebe.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    [Area("Student")]
    public class StudentLoginController : Controller
    {
        // مرحله ۱: نمایش انتخاب روش ورود
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // مرحله ۲-الف: نمایش فرم ورود با OTP
        [HttpGet]
        public IActionResult LoginOTP()
        {
            return PartialView(nameof(LoginOTP));
        }

        // مرحله ۲-الف (POST): دریافت کد ملی و ریدایرکت به 2FA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginOTP(LoginOTPViewModel vm)
        {
            if (!ModelState.IsValid)
                // PartialView مرحله اول را با vm برگردان
                return PartialView(nameof(LoginOTP), vm);

            // ذخیره یا ارسال OTP...

            // این‌جا PartialView مرحله‌ی دوم را برمی‌گردانیم
            return PartialView(nameof(StudentTwoFactorAuthentication), new StudentTwoFactorAuthenticationViewModel());
        }


        // مرحله ۲-ب: نمایش فرم ورود با کد رهگیری
        [HttpGet]
        public IActionResult LoginCodeRahgiri()
        {
            return PartialView(nameof(LoginCodeRahgiri));
        }


        // مرحله ۳: صفحه احراز هویت دومرحله‌ای
        [HttpGet]
        public IActionResult StudentTwoFactorAuthentication()
        {
            return PartialView(nameof(StudentTwoFactorAuthentication), new StudentTwoFactorAuthenticationViewModel());
        }

    }
}