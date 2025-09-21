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

namespace SanjeshP.Mosahebe.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    [Area("Student")]
    public class StudentLoginController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentOtpRepository _studentOtpRepository;
        private readonly IPayamSMS _payamSMS;

        public StudentLoginController(
            IStudentRepository studentRepository,
            IStudentOtpRepository studentOtpRepository,
            IPayamSMS payamSMS)
        {
            _studentRepository = studentRepository;
            _studentOtpRepository = studentOtpRepository;
            _payamSMS = payamSMS;
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
        public async Task<IActionResult> LoginCodeRahgiri(LoginCodeRahgiriViewModel model,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var student = await _studentRepository.GetStudentByNationalCodeCodeRahgiriAsync(model.NationalCode, model.CodeRahgiri, cancellationToken);
            //اگر ورودی معتبر نبود یا کاربر پیدا نشد → دوباره همون PartialView با خطا نمایش داده می‌شه.
            if (student == null)
            {
                ModelState.AddModelError("", "کد ملی یا کد رهگیری اشتباه است.");
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

            var student = await _studentRepository.GetStudentByNationalCodeAsync(model.NationalCode, cancellationToken);

            if (student == null)
            {
                ModelState.AddModelError("", "کد ملی یافت نشد.");
                return PartialView(model);
            }

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
        public async Task<IActionResult> StudentTwoFactorAuthentication(StudentTwoFactorAuthenticationViewModel model,string nationalCode,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.NationalCode = nationalCode;
                return PartialView(model);
            }

            var student = await _studentRepository.GetStudentByNationalCodeAsync(nationalCode, cancellationToken);

            if (student == null)
            {
                ModelState.AddModelError("", "داوطلب یافت نشد.");
                ViewBag.NationalCode = nationalCode;
                return PartialView(model);
            }

            var otp = await _studentOtpRepository.GetValidOtpAsync(student.Id, model.TwoFactorCode, cancellationToken);

            if (otp == null || otp.IsUsed)
            {
                ModelState.AddModelError("", "کد وارد شده صحیح نیست یا منقضی شده است.");
                ViewBag.NationalCode = nationalCode;
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
    }
}
