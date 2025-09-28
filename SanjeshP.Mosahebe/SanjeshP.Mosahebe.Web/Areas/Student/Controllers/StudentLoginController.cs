using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;
using SanjeshP.Mosahebe.Services.SmsSender.PayamSMS;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;
using System;
using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.Entities.User;
using Microsoft.AspNetCore.Http;
using SkiaSharp;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SanjeshP.Mosahebe.Entities.SiteSetting;
using SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SanjeshP.Mosahebe.Common;

namespace SanjeshP.Mosahebe.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    [Area("Student")]
    public class StudentLoginController : Controller
    {
        private readonly ILogger<StudentLoginController> _logger;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentOtpRepository _studentOtpRepository;
        private readonly IPayamSMS _payamSMS;
        private readonly IMemoryCache _memoryCache;
        private readonly SiteSettings _siteSetting;

        public StudentLoginController(
            ILogger<StudentLoginController> logger,
            IStudentRepository studentRepository,
            IStudentOtpRepository studentOtpRepository,
            IPayamSMS payamSMS,
            IMemoryCache memoryCache,
            IOptionsSnapshot<SiteSettings> siteSetting
            )
        {
            this._logger = logger;
            _studentRepository = studentRepository;
            _studentOtpRepository = studentOtpRepository;
            _payamSMS = payamSMS;
            _memoryCache = memoryCache;
            this._siteSetting = siteSetting.Value;
        }

        // ================= صفحه اصلی لاگین =================
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // صفحه‌ای که انتخاب روش ورود را نمایش می‌دهد
        }

        // ================= ورود با کدرهگیری =================
        [HttpGet]
        public IActionResult LoginCodeRahgiri()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginCodeRahgiri(LoginCodeRahgiriViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
            if (string.IsNullOrEmpty(sessionCaptcha) || model.CaptchaCode != sessionCaptcha)
            {
                ModelState.AddModelError("CaptchaCode", "کد امنیتی اشتباه است");
                return PartialView(model);
            }


            var student = await _studentRepository.GetStudentByNationalCodeCodeRahgiriAsync(model.NationalCode, model.CodeRahgiri, cancellationToken);
            //اگر ورودی معتبر نبود یا کاربر پیدا نشد → دوباره همون PartialView با خطا نمایش داده می‌شه.
            if (student == null)
            {
                ModelState.AddModelError("", "کد ملی یا کد رهگیری اشتباه است.");
                _logger.LogWarning("LoginCodeRahgiri failed for StudentId={StudentId}, NationalCode={NationalCode}, CodeRahgiri={CodeRahgiri}", student.Id, model.NationalCode, model.CodeRahgiri);
                return PartialView(model);
            }

            await SignInStudent(student);
            return Json(new { redirectToUrl = Url.Action("Index", "StudentHome", new { area = "Student" }) });

            //return RedirectToAction("Index", "StudentHome", new { area = "Student" });
        }

        // ================= ورود با OTP =================
        [HttpGet]
        public IActionResult LoginOTP()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginOTP(LoginOTPViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
            if (string.IsNullOrEmpty(sessionCaptcha) || model.CaptchaCode != sessionCaptcha)
            {
                ModelState.AddModelError("CaptchaCode", "کد امنیتی اشتباه است");
                return PartialView(model);
            }

            var student = await _studentRepository.GetStudentByNationalCodeAsync(model.NationalCode, cancellationToken);
            if (student == null)
            {
                ModelState.AddModelError("", "کد ملی یافت نشد.");
                _logger.LogWarning("LoginOTP failed for StudentId={StudentId}, NationalCode={NationalCode}", student.Id, model.NationalCode);
                return PartialView(model);
            }

            // شمارش درخواست‌های OTP در روز جاری
            // var todayStart = DateTime.UtcNow.Date;
            // 1. دریافت تمام OTPهای کاربر
            //var otps = await _studentOtpRepository.GetAllOtpBystudentIdAsync(student.Id, cancellationToken);
            // 2. شمارش درخواست‌های ۲۴ ساعت گذشته
            //var otpCountToday = otps.Count(o => o.CreatedDate >= DateTime.UtcNow.AddHours(-24));

            var otpCountToday = await _studentOtpRepository.CountOtpByStudentSinceAsync(student.Id, DateTime.UtcNow.AddHours(-24), cancellationToken);

            // 3. بررسی سقف ۱۰ بار
            if (otpCountToday >= _siteSetting.authenticationStudentSettings.MaxSmsPerDay)
            {
                ModelState.AddModelError("",
                    $"امروز {otpCountToday} بار درخواست ارسال کد کرده‌اید. بیش از ۱۰ بار امکان‌پذیر نیست.");
                return PartialView(model);
            }

            // ۲. غیرفعال کردن (IsDeleted = true) همهٔ کدهای قبلی
            await _studentOtpRepository.InvalidatePreviousOtpsAsync(student.Id, cancellationToken);

            // تولید OTP شش رقمی
            var otpCode = new Random().Next(100000, 999999).ToString();
            var otpEntity = new StudentOtp
            {
                Id = Guid.NewGuid(),
                StudentId = student.Id,
                Code = otpCode,
                HostIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                CreatedDate = DateTime.UtcNow,
                ExpiresAt = DateTime.Now.AddSeconds(120),
                SentTo = student.PhoneNumber,
                IsUsed = false
            };

            await _studentOtpRepository.AddAsync(otpEntity, cancellationToken);
            await _studentOtpRepository.SaveChangesAsync(cancellationToken);

            // ارسال SMS با PayamSMS
            var smsMessage = $"کد یکبار مصرف شما: {otpCode}";
            await _payamSMS.Send(student.PhoneNumber, smsMessage);

            ViewBag.NationalCode = student.NationalCode;
            return PartialView("StudentTwoFactorAuthentication", new StudentTwoFactorAuthenticationViewModel());
        }

        // ================= وارد کردن و تایید OTP =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentTwoFactorAuthentication(StudentTwoFactorAuthenticationViewModel model, string nationalCode, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.NationalCode = nationalCode;
                return PartialView(model);
            }

            var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
            if (string.IsNullOrEmpty(sessionCaptcha) || model.CaptchaCode != sessionCaptcha)
            {
                ModelState.AddModelError("CaptchaCode", "کد امنیتی اشتباه است");
                return PartialView(model);
            }

            var student = await _studentRepository.GetStudentByNationalCodeAsync(nationalCode, cancellationToken);
            if (student == null)
            {
                ModelState.AddModelError("", "داوطلب یافت نشد.");
                ViewBag.NationalCode = nationalCode;
                _logger.LogWarning("StudentTwoFactorAuthentication failed (student == null) for StudentId={StudentId}, Code={Code}", student.Id, model.TwoFactorCode);
                return PartialView(model);
            }

            var otp = await _studentOtpRepository.GetValidOtpAsync(student.Id, model.TwoFactorCode, cancellationToken);
            if (otp == null || otp.IsUsed || otp.ExpiresAt < DateTime.UtcNow)
            {
                ModelState.AddModelError("", "کد وارد شده صحیح نیست یا منقضی شده است.");
                ViewBag.NationalCode = nationalCode;
                _logger.LogWarning("OTP validation failed for StudentId={StudentId}, Code={Code}",student.Id, model.TwoFactorCode);
                return PartialView(model);
            }

            // علامت‌گذاری OTP به عنوان مصرف‌شده
            otp.IsUsed = true;
            await _studentOtpRepository.UpdateAsync(otp, cancellationToken);
            await _studentOtpRepository.SaveChangesAsync(cancellationToken);

            // ایجاد Claims و لاگین
            await SignInStudent(student);
            return Json(new { redirectToUrl = Url.Action("Index", "StudentHome", new { area = "Student" }) });
            //return RedirectToAction("Index", "StudentHome", new { area = "Student" });
        }

        // ================= متد کمکی SignIn =================
        private async Task SignInStudent(SanjeshP.Mosahebe.Entities.Student.Student student)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, student.FirstName + " " + student.LastName),
                new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                new Claim("NationalCode", student.NationalCode),
                new Claim(ClaimTypes.Role, "Student")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("StudentScheme", principal);
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        // ================= متد کمکی CaptchaCode =================
        #region CaptchaCode
        [HttpGet]
        [Route("student/captcha-image")]
        public IActionResult CaptchaImage_()
        {
            var captchaCode = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("CaptchaCode", captchaCode);

            int width = 120;
            int height = 50;

            // ایجاد surface با پشتیبانی از آلفا (شفاف)
            var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            // پس‌زمینه شفاف
            canvas.Clear(SKColors.Transparent);

            using var paint = new SKPaint
            {
                Color = SKColors.Black, // متن کاملا اپاسیتی
                TextSize = 28,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            };

            // موقعیت متن را تنظیم کنید تا در مرکز یا جای دلخواه قرار گیرد
            var bounds = new SKRect();
            paint.MeasureText(captchaCode, ref bounds);
            float x = 10;
            float y = height / 2f + bounds.Height / 2f;

            canvas.DrawText(captchaCode, x, y, paint);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            return File(data.ToArray(), "image/png");
        }


        private void IncrementAttempts(string key)
        {
            var attempts = _memoryCache.Get<int>(key);
            _memoryCache.Set(key, attempts + 1, TimeSpan.FromMinutes(5));
        }
        #endregion
    }
}
