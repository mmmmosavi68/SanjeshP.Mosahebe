using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels;

namespace SanjeshP.Mosahebe.Web.Areas.Auth.Controllers
{
    [AllowAnonymous]
    [Area("Auth")]
    public class TwoFactorAuthenticationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly ILogger<TwoFactorAuthenticationController> _logger;

        public TwoFactorAuthenticationController(IUserRepository userRepository, IUserTokenRepository userTokenRepository, ILogger<TwoFactorAuthenticationController> logger)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult TwoFactorAuthentication()
        {
            return View(new TwoFactorViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorAuthentication(TwoFactorViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //    var token = await _userTokenRepository.GetTokenByCodeAsync(model.VerificationCode, cancellationToken);
            //    if (token == null || token.ExpirationDate < DateTime.Now || token.IsDeleted)
            //    {
            //        ModelState.AddModelError("", "کد تایید نامعتبر یا منقضی شده است.");
            //        _logger.LogWarning("Two-factor authentication failed: invalid or expired token.");
            //        return View(model);
            //    }

            //    var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
            //    if (user == null)
            //    {
            //        ModelState.AddModelError("", "کاربر یافت نشد.");
            //        return View(model);
            //    }

            //    // عملیات ورود کاربر
            //    var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //    new Claim(ClaimTypes.Name, user.UserName),
            //};

            //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //    var principal = new ClaimsPrincipal(identity);
            //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            //    _logger.LogInformation($"Two-factor authentication succeeded for user {user.UserName}.");
            return RedirectToAction("Index", "Home");
        }
    }
}