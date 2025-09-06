using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Common;
using System;
using System.IO;
using System.Text.Json;

namespace SanjeshP.Mosahebe.WebFramework.Configuration
{
    /// <summary>
    /// کلاس تولید فایل env.js برای سمت کلاینت در زمان اجرا.
    /// این فایل شامل تنظیمات محیطی مثل BaseUrl، ApiPrefix و نوع Environment است.
    /// </summary>
    public class EnvJsGenerator
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger<EnvJsGenerator> _logger;

        /// <summary>
        /// سازنده کلاس؛ وابستگی‌ها از DI تزریق می‌شوند.
        /// </summary>
        /// <param name="env">اطلاعات محیط وب مانند مسیر wwwroot</param>
        /// <param name="config">تنظیمات برنامه از appsettings.json</param>
        /// <param name="logger">لاگر برای ثبت اتفاقات و خطاها</param>
        public EnvJsGenerator(IWebHostEnvironment env, IConfiguration config, ILogger<EnvJsGenerator> logger)
        {
            _env = env;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// متد اصلی برای ساخت فایل env.js در مسیر wwwroot
        /// از مقادیر تنظیم‌شده در ClientConfig استفاده می‌کند
        /// </summary>
        public void Generate()
        {
            try
            {
                // خواندن تنظیمات بخش ClientConfig از فایل appsettings
                var clientConfig = _config.GetSection("ClientConfig").Get<ClientConfig>();

                // بررسی مقدار baseUrl و هشدار در صورت خالی بودن
                //if (string.IsNullOrWhiteSpace(clientConfig.BaseUrl))
                //{
                //    _logger.LogWarning("ClientConfig.BaseUrl خالی یا نامعتبر است!");
                //}

                // تنظیمات سریال‌سازی JSON با فرمت camelCase و خوانا
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                // تبدیل تنظیمات به JSON و ساخت متن JavaScript
                var jsonContent = JsonSerializer.Serialize(clientConfig, jsonOptions);
                var jsContent = $"window.__env = {jsonContent};";

                // مسیر فایل env.js در wwwroot
                var filePath = Path.Combine(_env.WebRootPath, "env.js");

                // نوشتن محتوای فایل و ثبت موفقیت در لاگر
                File.WriteAllText(filePath, jsContent);
                _logger.LogInformation($"فایل env.js با موفقیت در مسیر '{filePath}' ایجاد شد.");
            }
            catch (Exception ex)
            {
                // ثبت خطا در صورت بروز مشکل در ساخت فایل
                _logger.LogError(ex, "خطا در ایجاد فایل env.js");
            }
        }
    }
}
