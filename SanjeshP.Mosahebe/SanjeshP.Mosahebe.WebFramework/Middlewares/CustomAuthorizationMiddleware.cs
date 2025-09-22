using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Menus;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.WebFramework.Middlewares
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;

        public CustomAuthorizationMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            // ۱. اگر متد یا کنترلر با [AllowAnonymous] علامت‌گذاری شده، عبور بده
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            // ۲. تشخیص area از Route data
            var area = context.GetRouteValue("area")?.ToString();

            if (string.Equals(area, "Student", System.StringComparison.OrdinalIgnoreCase))
            {
                // ۳. برای دانشجو از StudentScheme احراز هویت کن
                var studentAuth = await context.AuthenticateAsync("StudentScheme");
                if (!studentAuth.Succeeded)
                {
                    // اگر لاگین نبود، چالش دانشجو (ریدایرکت به /Student/Login)
                    await context.SignOutAsync("StudentScheme");
                    context.Response.Redirect("/Student/StudentLogin/Login");
                    //await context.ChallengeAsync("StudentScheme");
                    return;
                }

                // ۴. مجوز ساده: نقش باید Student باشد
                if (!context.User.IsInRole("Student"))
                {
                    await context.SignOutAsync("StudentScheme");
                    context.Response.Redirect("/Student/StudentLogin/Login");
                    return;
                }

                // ادامه‌ی درخواست
                await _next(context);
                return;
            }
            else
            {
                // ۵. برای کاربران عادی از UserScheme احراز هویت کن
                var userAuth = await context.AuthenticateAsync("UserScheme");
                if (!userAuth.Succeeded)
                {
                    // چالش برای کاربر عادی (ریدایرکت به /Login)
                    await context.ChallengeAsync("UserScheme");
                    return;
                }

                // ۶. در اینجا ValidatePrincipal کوکی قبلاً توکن و وضعیت کاربر را چک کرده
                //    بنابراین صرفاً اجازه‌ی ادامه بده

                // ۷. اگر نیاز به بررسی دسترسی بر اساس cached permissions باشد:
                //    فرض کنید claim توکن با نام "Token" در کوکی ثبت شده
                var token = context.User.FindFirst("Token")?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    await context.SignOutAsync("UserScheme");
                    context.Response.Redirect("/Login");
                    return;
                }

                // ۸. بررسی سطح دسترسی از MemoryCache
                var cacheKey = $"UserPermissions_{token}";
                if (_memoryCache.TryGetValue(cacheKey, out List<UserAccessViewModel> permissions))
                {
                    var areaName = context.GetRouteValue("area")?.ToString();
                    var controller = context.GetRouteValue("controller")?.ToString();
                    var action = context.GetRouteValue("action")?.ToString();

                    var hasAccess = permissions.Any(m =>
                        m.Area == areaName &&
                        m.ControllerName == controller &&
                        m.ActionName == action &&
                        (m.Person_Checkecd || m.Group_Checkecd)
                    );

                    if (!hasAccess)
                    {
                        // AJAX ⇒ 403، وگرنه صفحه‌ی AccessDenied
                        if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            context.Response.Clear();
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            await context.Response.WriteAsync("شما مجوز دسترسی به این بخش را ندارید.");
                        }
                        else
                        {
                            context.Response.Redirect("/SharedError/AccessDenied");
                        }
                        return;
                    }
                }

                // ادامه‌ی درخواست
                await _next(context);
                return;
            }
            //// بررسی ورود کاربر
            //      var user = context.User;
            //    var tokenClaim = user.FindFirst("token")?.Value;
            //    if (user?.Identity?.IsAuthenticated != true || string.IsNullOrEmpty(tokenClaim))
            //    {
            //        await context.SignOutAsync(); // خروج کاربر در صورت عدم احراز هویت
            //        context.Response.Redirect("/Login");
            //        return;
            //    }

            //    if (Guid.TryParse(tokenClaim, out Guid token))
            //    {
            //        var tokenRepository = context.RequestServices.GetRequiredService<IUserTokenRepository>();
            //        var haToken = await tokenRepository.GetUserTokenByIdAsync(token, context.RequestAborted);

            //        if (haToken == null || haToken.IsDeleted)
            //        {
            //            await context.SignOutAsync(); // خروج کاربر در صورت توکن نامعتبر
            //            context.Response.Redirect("/Login");
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        // اگر توکن معتبر نبود
            //        await context.SignOutAsync();
            //        context.Response.Redirect("/Login");
            //        return;
            //    }

            //    // بررسی سطح دسترسی کاربر
            //    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //    if (Guid.TryParse(userIdClaim, out Guid userId))
            //    {
            //        //var area = context.GetRouteValue("area")?.ToString();
            //        var actionName = context.GetRouteValue("action")?.ToString();
            //        var controllerName = context.GetRouteValue("controller")?.ToString();

            //        #region بررسی سطح دسترسی مستقیم از بانک اطلاعاتی 
            //        //var userMenubarRepository = context.RequestServices.GetRequiredService<IViewUserMenubarRepository>();
            //        //var result = userMenubarRepository.GetUserAccessMenus(userId, context.RequestAborted);
            //        //var hasPermission = result.Any(vum =>
            //        //    vum.ControllerName == controllerName &&
            //        //    vum.ActionName == actionName &&
            //        //    (vum.Person_Checkecd || vum.Group_Checkecd));
            //        //if (!hasPermission)
            //        //{
            //        //    if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //        //    {
            //        //        // درخواست AJAX برای PartialView
            //        //        context.Response.Clear();
            //        //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //        //        await context.Response.WriteAsync("شما مجوز دسترسی به این بخش را ندارید.");
            //        //    }
            //        //    else
            //        //    {
            //        //        // هدایت به صفحه مناسب برای View
            //        //        context.Response.Redirect("/SharedError/AccessDenied");
            //        //    }
            //        //    return;
            //        //}
            //        #endregion

            //        #region بررسی سطح دسترسی به وسیله  MemoryCache 
            //        var cacheKey = $"UserPermissions_{tokenClaim}";
            //        if (_memoryCache.TryGetValue(cacheKey, out List<UserAccessViewModel> cachedData))
            //        {
            //            var hasPermission = cachedData.Any(vum =>
            //                vum.ControllerName == controllerName &&
            //                vum.ActionName == actionName &&
            //                vum.Area == area &&
            //                (vum.Person_Checkecd || vum.Group_Checkecd));

            //            if (!hasPermission)
            //            {
            //                if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //                {
            //                    // درخواست AJAX برای PartialView
            //                    context.Response.Clear();
            //                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //                    await context.Response.WriteAsync("شما مجوز دسترسی به این بخش را ندارید.");
            //                }
            //                else
            //                {
            //                    // هدایت به صفحه مناسب برای View
            //                    context.Response.Redirect("/SharedError/AccessDenied");
            //                }
            //                return;
            //            }
            //        }
            //        #endregion
            //    }
            //    else
            //    {
            //        await context.SignOutAsync();
            //        context.Response.Redirect("/Login");
            //        return;
            //    }

            //    // ادامه درخواست به Middleware بعدی
            //    await _next(context);
        }
    }

}
