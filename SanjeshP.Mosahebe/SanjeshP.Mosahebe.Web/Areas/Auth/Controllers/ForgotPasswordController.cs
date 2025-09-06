using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Web.SharedViewModels;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using System;
using System.Threading;
using System.Threading.Tasks;
using SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels;

namespace SanjeshP.Mosahebe.Web.Areas.Auth.Controllers
{
    [AllowAnonymous]
    [Area("Auth")]
    public class ForgotPasswordController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly ILogger<ForgotPasswordController> _logger;

        public ForgotPasswordController(IUserRepository userRepository, IUserTokenRepository userTokenRepository, ILogger<ForgotPasswordController> logger)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userRepository.GetUserByEmailAsync(model.EmailOrPhoneNumber, cancellationToken);
            if (user == null)
            {
                ModelState.AddModelError("", "کاربری با این مشخصات یافت نشد.");
                _logger.LogWarning("Forgot password failed for email: " + model.EmailOrPhoneNumber);
                return View(model);
            }

            //var verificationCode = TokenGenerator.GenerateVerificationCode();
            //await _userTokenRepository.AddTokenAsync(new UserToken
            //{
            //    UserId = user.Id,
            //    Token = verificationCode,
            //    ExpirationDate = DateTime.Now.AddMinutes(10),
            //    IsDeleted = false,
            //}, cancellationToken);

            //_logger.LogInformation($"Verification code sent to {user.EmailAddress}.");
            //ViewBag.Message = "کد تایید به ایمیل یا شماره تماس شما ارسال شد.";
            return RedirectToAction("ConfirmCode", "ForgotPassword");
        }

        [HttpGet]
        public IActionResult ConfirmCode()
        {
            return View(new TwoFactorViewModel());
        }
    }
}