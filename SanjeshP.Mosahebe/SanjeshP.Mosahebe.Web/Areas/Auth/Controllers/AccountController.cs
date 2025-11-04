using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipelines.Sockets.Unofficial.Arenas;
using SanjeshP.Mosahebe.Web.SharedViewModels;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Menus;
using SanjeshP.Mosahebe.Data.Contracts.SiteSetting;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.User;
using SanjeshP.Mosahebe.Security;
using SanjeshP.Mosahebe.Services.SmsSender.PayamSMS;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using SanjeshP.Mosahebe.Web.Areas.Auth.ViewModels;

namespace SanjeshP.Mosahebe.Web.Areas.Auth.Controllers
{
    [AllowAnonymous]
    [Area("Auth")]
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IViewUserMenubarRepository _viewUserMenubarRepository;
        private readonly IUserOtpRepository _userOtpRepository;
        private readonly IPayamSMS _payamSMS;
        private readonly ILoginSettingsRepository _loginSettingsRepository;
        private readonly SiteSettings _siteSetting;
        public AccountController(IMapper mapper
                                , IUserRepository userRepository
                                , ILogger<AccountController> logger
                                , IUserTokenRepository userTokenRepository
                                , IOptionsSnapshot<SiteSettings> siteSetting
                                , IMemoryCache memoryCache
                                , IViewUserMenubarRepository viewUserMenubarRepository
                                , IUserOtpRepository userOtpRepository
                                , IPayamSMS payamSMS
                                , ILoginSettingsRepository loginSettingsRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _logger = logger;
            _userTokenRepository = userTokenRepository;
            _memoryCache = memoryCache;
            _viewUserMenubarRepository = viewUserMenubarRepository;
            _userOtpRepository = userOtpRepository;
            _payamSMS = payamSMS;
            _loginSettingsRepository = loginSettingsRepository;
            _siteSetting = siteSetting.Value;
        }


        #region Login
        [HttpGet("/Login")]
        public async Task<IActionResult> Login(CancellationToken cancellationToken)
        {
            // اگر کاربر لاگین کرده باشد، انتقال به داشبورد
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Home" });
            }

            var loginSettings = await _loginSettingsRepository.GetActiveLoginSetting(cancellationToken);
            var viewModel = new LoginPageViewModel
            {
                Settings = loginSettings
            };

            return View(viewModel);
        }

        [HttpPost("/Login")]
        public async Task<IActionResult> Login(LoginPageViewModel loginPageViewModel, CancellationToken cancellationToken)
        {
            var loginSettings = await _loginSettingsRepository.GetActiveLoginSetting(cancellationToken);
            loginPageViewModel.Settings = loginSettings;
            // کلید ذخیره تعداد تلاش: ترکیبی از IP و نام کاربری (اگر نام کاربری وجود داشته باشد)
            string userName = loginPageViewModel.Login?.UserName?.ToLower() ?? "unknown";
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"LoginAttempts_{ipAddress}_{userName}";

            var attempts = _memoryCache.Get<int>(key);

            // بررسی تعداد تلاش‌های ناموفق
            if (attempts >= loginSettings.MaxFailedLoginAttempts)
            {
                ModelState.AddModelError("", "تعداد تلاش‌های ناموفق بیش از حد مجاز است. لطفاً چند دقیقه دیگر صبر کنید.");
                _logger.LogWarning($"Login MaxFailedLoginAttempts: {key} تعداد تلاش‌های ناموفق بیش از حد مجاز.");
                return View(loginPageViewModel);
            }

            // اعتبارسنجی اولیه مدل
            if (!ModelState.IsValid)
            {
                IncrementAttempts(key);
                return View(loginPageViewModel);
            }

            // بررسی کپچا در صورت فعال بودن
            if (loginSettings.IsCaptchaEnabled)
            {
                var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
                if (string.IsNullOrEmpty(sessionCaptcha) || loginPageViewModel.Login.CaptchaCode != sessionCaptcha)
                {
                    ModelState.AddModelError("CaptchaCode", "کد امنیتی اشتباه است");
                    IncrementAttempts(key);
                    return View(loginPageViewModel);
                }
            }

            if (loginSettings.IsOtpLoginEnabled) // ✅ ورود غیر مستقیم با OTP
            {
                // اینجا باید لاجیک ورود OTP بذاری (مثلاً ارسال کد و بعد تایید)
                // چون این متد لاگین اولیه است، می‌تونی ارسال کد OTP رو انجام بدی یا ریدایرکت کنی به صفحه تایید OTP
                // اگر نام کاربری وجود نداشت افزایش تلاش بزن
                var user = await _userRepository.GetUserByUserNameAsync(userName, cancellationToken);
                if (user == null)
                {
                    ModelState.AddModelError("UserName", "کاربری با چنین مشخصاتی یافت نشد");
                    _logger.LogWarning($"Login failed: {userName} not found.");
                    IncrementAttempts(key);
                    return View(loginPageViewModel);
                }

                // ارسال OTP و ریدایرکت (مشابه کدی که خودت داری)
                // ...

                // بعد از موفقیت در ارسال OTP شمارنده رو حذف کن
                _memoryCache.Remove(key);

                // ادامه فرآیند مثل کد خودت
                return Redirect("VerifyOtp");
            }

            // اگر ورود با نام کاربری و رمز عبور فعال است
            if (loginSettings.IsUsernamePasswordLoginEnabled)
            {
                var user = await _userRepository.GetUserByUserNameAsync(userName, cancellationToken);
                if (user == null)
                {
                    ModelState.AddModelError("UserName", "کاربری با چنین مشخصاتی یافت نشد");
                    _logger.LogWarning($"Login failed: {userName} not found.");
                    IncrementAttempts(key);
                    return View(loginPageViewModel);
                }

                if (!PasswordHelper.VerifyPasswordBCrypt(loginPageViewModel.Login.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("Password", "نام کاربری یا رمز عبور اشتباه است");
                    _logger.LogWarning($"Login failed: password verification failed for user {user.UserName}.");
                    IncrementAttempts(key);
                    return View(loginPageViewModel);
                }

                // بررسی وضعیت حساب کاربری (حذف شده، فعال، منقضی)
                if (user.IsDeleted)
                {
                    ModelState.AddModelError("UserName", "حساب کاربری شما حذف شده است");
                    _logger.LogWarning($"Login failed: user {user.UserName} is deleted.");
                    return View(loginPageViewModel);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError("UserName", "حساب کاربری شما فعال نشده است");
                    _logger.LogWarning($"Login failed: user {user.UserName} is not active.");
                    return View(loginPageViewModel);
                }

                if (user.ExpireDate < DateTime.Now)
                {
                    ModelState.AddModelError("UserName", "حساب کاربری شما منقضی شده است");
                    _logger.LogWarning($"Login failed: user {user.UserName} account is expired.");
                    return View(loginPageViewModel);
                }

                // اگر ورود دو مرحله‌ای فعال است
                if (loginSettings.IsTwoFactorEnabled)
                {
                    // تولید و ارسال OTP (مثل کد فعلی خودت)
                    // بعد حذف شمارنده
                    _memoryCache.Remove(key);


                    // حذف توکن‌های قدیمی
                    await _userTokenRepository.MarkExpiredTokensAsDeletedAsync(user.Id, cancellationToken);

                    // تولید و ذخیره OTP
                    var otpCode = new Random().Next(100000, 999999).ToString();

                    var otp = new UserOtp
                    {
                        UserId = user.Id,
                        Code = otpCode,
                        ExpireAt = DateTime.Now.AddSeconds(120),
                        IsUsed = false,
                        SentTo = user.PhoneNumber,
                        Purpose = "Login",

                        CreatedBy = user.Id, // یا ادمین/سیستم اگر لاگین نیست
                        HostIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                    };

                    await _userOtpRepository.AddAsync(otp, cancellationToken);

                    // ارسال OTP
                    //_smsSender.SendAsync(user.PhoneNumber, $"کد تایید شما: {otpCode}");

                    DateTime Time = DateTime.Now;
                    PersianCalendar pc = new PersianCalendar();
                    int day, month, year, hour, min, sec;
                    day = pc.GetYear(Time);
                    month = pc.GetMonth(Time);
                    year = pc.GetDayOfMonth(Time);
                    string CreateCodeTime = DateTime.Now.ToString("HH:mm");
                    string ConvertPDT = day + "/" + month + "/" + year + " " + CreateCodeTime;

                    string smsContent = "محرمانه";
                    smsContent += "\r\n" + "رمز ورود به سامانه:";
                    smsContent += otp.Code;
                    smsContent += "\r\n" + ConvertPDT;
                    smsContent += "\r\n" + otp.ExpireAt + " ثانیه";

                    var result = await _payamSMS.Send(user.PhoneNumber, smsContent);
                    _logger.LogInformation($"OTP sent to user {user.UserName}: {otpCode}");

                    TempData["UserId"] = user.Id.ToString();
                    TempData.Keep("UserId");

                    return Redirect("VerifyOtp");
                }
                else if (!loginSettings.IsTwoFactorEnabled) // ✅ ورود مستقیم بدون OTP
                {
                    // موفقیت: حذف شمارنده تلاش‌ها
                    _memoryCache.Remove(key);

                    // حذف توکن‌های قبلی
                    await _userTokenRepository.MarkExpiredTokensAsDeletedAsync(user.Id, cancellationToken);

                    var token = new UserToken
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        SessionId = HttpContext.Session.Id,
                        UserAgent = Request.Headers["User-Agent"].ToString(),
                        IsDeleted = false,
                        CreatedDate = DateTime.Now,
                        ExpirationDate = DateTime.Now.AddMinutes(_siteSetting.authenticationSettings.ExpireTimeSpanInMinutes),
                        HostIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                    };

                    await _userTokenRepository.AddUserTokenAsync(token, cancellationToken);

                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserProfiles.Any() ? $"{user.UserProfiles.First().FirstName} {user.UserProfiles.First().LastName}" : "No Profile"),
                    new Claim("RoleID", user.UserRoles.Any() ? user.UserRoles.Last().Role.NormalizedRoleNameEn : "No Role"),
                    new Claim("RoleNameFa", user.UserRoles.Any() ? user.UserRoles.First().Role.RoleNameFa : "No Role"),
                    new Claim("Token", token.Id.ToString())
                };

                    // سطح دسترسی کاربر
                    var userPermissions = _viewUserMenubarRepository.GetUserAccessMenus(user.Id, cancellationToken);
                    var cacheKey = $"UserPermissions_{token.Id}";

                    var accessData = userPermissions.Select(x => new UserAccessViewModel
                    {
                        MenuTitle = x.MenuTitle ?? "Default Title",
                        PageCode = x.PageCode ?? 0,
                        SowMenu = x.ShowMenu ?? false,
                        CssClass = x.CssClass ?? "default-class",
                        Icon = x.Icon ?? "default-icon",
                        Area = x.Area ?? "default-area",
                        ControllerName = x.ControllerName ?? "default-controller",
                        ActionName = x.ActionName ?? "default-action",
                        Person_Checkecd = x.Person_Checkecd,
                        Group_Checkecd = x.Group_Checkecd,
                        Disabled = x.disabled,
                        IsParent = x.IsParent,
                        FirstName = user.UserProfiles.FirstOrDefault()?.FirstName ?? "Default FirstName",
                        LastName = user.UserProfiles.FirstOrDefault()?.LastName ?? "Default LastName",
                        RoleNameFa = user.UserRoles.FirstOrDefault()?.Role?.RoleNameFa ?? "Default Role"
                    }).ToList();

                    _memoryCache.Set(cacheKey, accessData, TimeSpan.FromMinutes(30));

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("UserScheme", principal);
                    //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    _logger.LogInformation($"Login succeeded (no OTP): user {user.UserName} authenticated.");
                    return Redirect("/Home/Home");
                }
            }

            // اگر هیچ نوع ورود فعال نبود
            ModelState.AddModelError("", "روش ورود فعال نیست");
            return View(loginPageViewModel);
        }
        #endregion

        #region Register
        [HttpGet("/Register")]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost("/Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            var user = _mapper.Map<SanjeshP.Mosahebe.Entities.User.User>(registerViewModel);
            user.PasswordHash = PasswordHelper.HashPasswordBCrypt(registerViewModel.Password);
            user.CreatedDate = DateTime.Now;
            user.IsActive = true;

            await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation($"User {registerViewModel.UserName} registered successfully.");

            return RedirectToAction("Login");
        }
        #endregion

        #region Logout
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        [Route("Auth/Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("UserScheme");
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login");
        }
        #endregion

        #region CaptchaCode
        [HttpGet]
        [Route("captcha-image")]
        public IActionResult CaptchaImage_()
        {
            var captchaCode = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("CaptchaCode", captchaCode);

            int width = 120;
            int height = 50;

            using var bitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 28,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            };

            canvas.DrawText(captchaCode, 10, 35, paint);

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return File(data.ToArray(), "image/png");
        }
        #endregion

        #region OTP
        [HttpGet]
        [Route("VerifyOtp")]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        [Route("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var code = model.Code;
            if (string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "لطفاً کد را وارد کنید.");
                return View();
            }

            if (!TempData.ContainsKey("UserId"))
            {
                _logger.LogWarning("VerifyOtp: TempData does not contain UserId.");
                return RedirectToAction("Login");
            }

            var userId = Guid.Parse(TempData["UserId"].ToString());

            // به صورت اختیاری، مقدار RememberMe را هم از TempData بخوانیم
            bool rememberMe = TempData.ContainsKey("RememberMe") && bool.TryParse(TempData["RememberMe"]?.ToString(), out var rm) && rm;

            var otp = await _userOtpRepository.GetValidOtpAsync(userId, code, cancellationToken);

            if (otp == null || otp.IsUsed == true || otp.ExpireAt <= DateTime.Now || otp.IsDeleted == true)
            {
                ModelState.AddModelError("", "کد وارد شده نامعتبر یا منقضی است.");
                return View();
            }

            // علامت‌گذاری OTP به عنوان استفاده‌شده
            otp.IsUsed = true;
            await _userOtpRepository.UpdateAsync(otp, cancellationToken);

            //var user = await _userRepository.GetByIdAsync(cancellationToken, userId);
            var user = await _userRepository.GetByGuidIdAsync(userId, cancellationToken);

            // ساخت توکن و ثبت اطلاعات
            var token = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                SessionId = HttpContext.Session.Id,
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddMinutes(_siteSetting.authenticationSettings.ExpireTimeSpanInMinutes),
                HostIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
            };

            await _userTokenRepository.AddUserTokenAsync(token, cancellationToken);

            // تعریف Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserProfiles.Any() ? $"{user.UserProfiles.First().FirstName} {user.UserProfiles.First().LastName}" : "No Profile"),
                new Claim("RoleID", user.UserRoles.Any() ? user.UserRoles.Last().Role.NormalizedRoleNameEn : "No Role"),
                new Claim("RoleNameFa", user.UserRoles.Any() ? user.UserRoles.First().Role.RoleNameFa : "No Role"),
                new Claim("Token", token.Id.ToString())
            };

            #region ذخیره سطح دسترسی کاربر در MemoryCache
            var userPermissions = _viewUserMenubarRepository.GetUserAccessMenus(user.Id, cancellationToken);
            var cacheKey = $"UserPermissions_{token.Id}";

            var accessData = userPermissions.Select(x => new UserAccessViewModel
            {
                MenuTitle = x.MenuTitle ?? "Default Title",
                PageCode = x.PageCode ?? 0,
                SowMenu = x.ShowMenu ?? false,
                CssClass = x.CssClass ?? "default-class",
                Icon = x.Icon ?? "default-icon",
                Area = x.Area ?? "default-area",
                ControllerName = x.ControllerName ?? "default-controller",
                ActionName = x.ActionName ?? "default-action",
                Person_Checkecd = x.Person_Checkecd,
                Group_Checkecd = x.Group_Checkecd,
                Disabled = x.disabled,
                IsParent = x.IsParent,
                FirstName = user.UserProfiles.FirstOrDefault()?.FirstName ?? "Default FirstName",
                LastName = user.UserProfiles.FirstOrDefault()?.LastName ?? "Default LastName",
                RoleNameFa = user.UserRoles.FirstOrDefault()?.Role?.RoleNameFa ?? "Default Role"
            }).ToList();

            _memoryCache.Set(cacheKey, accessData, TimeSpan.FromMinutes(30));
            #endregion

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe
            };

            await HttpContext.SignInAsync("UserScheme", principal, authProperties);
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            _logger.LogInformation($"Login succeeded: user {user.UserName} authenticated successfully.");
            return Redirect("/Home/Home");
        }

        #endregion

        private void IncrementAttempts(string key)
        {
            var attempts = _memoryCache.Get<int>(key);
            _memoryCache.Set(key, attempts + 1, TimeSpan.FromMinutes(5));
        }
    }
}