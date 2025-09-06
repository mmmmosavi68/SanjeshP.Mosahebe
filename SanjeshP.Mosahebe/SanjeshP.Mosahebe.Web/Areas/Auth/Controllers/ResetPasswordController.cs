using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Security;
using System;
using System.Threading;
using System.Threading.Tasks;
using SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels;


namespace SanjeshP.Mosahebe.Web.Areas.Auth.Controllers
{
    [AllowAnonymous]
    [Area("Auth")]
    public class ResetPasswordController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly ILogger<ResetPasswordController> _logger;

        public ResetPasswordController(IUserRepository userRepository, IUserTokenRepository userTokenRepository, ILogger<ResetPasswordController> logger)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View(new ResetPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //var token = await _userTokenRepository.GetTokenByCodeAsync(model.VerificationCode, cancellationToken);
            //if (token == null || token.ExpirationDate < DateTime.Now || token.IsDeleted)
            //{
            //    ModelState.AddModelError("", "کد تایید نامعتبر یا منقضی شده است.");
            //    _logger.LogWarning("Reset password failed: invalid or expired token.");
            //    return View(model);
            //}

            //var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
            //if (user == null)
            //{
            //    ModelState.AddModelError("", "کاربر یافت نشد.");
            //    return View(model);
            //}

            //user.PasswordHash = PasswordHelper.HashPasswordBCrypt(model.NewPassword);
            //user.UpdatedDate = DateTime.Now;
            //await _userRepository.UpdateAsync(user, cancellationToken);

            //_logger.LogInformation($"Password reset successfully for user {user.UserName}.");
            return RedirectToAction("Login", "Account");
        }
    }
}