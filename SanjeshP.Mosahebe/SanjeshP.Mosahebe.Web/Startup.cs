using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.WebFramework.Configuration;
using SanjeshP.Mosahebe.WebFramework.Middlewares;
using System;
using WebFramework.Configuration;
using WebFramework.CustomMapping;
using WebFramework.Middlewares;

namespace SanjeshP.Mosahebe.Web
{
    public class Startup
    {
        private readonly SiteSettings _siteSetting;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));
            services.Configure<ClientConfig>(Configuration.GetSection(nameof(ClientConfig)));
            services.AddSingleton<EnvJsGenerator>();
            services.InitializeAutoMapper();
            services.AddDbContext(Configuration);
            services.AddCustomIdentity(_siteSetting.IdentitySettings);
            services.AddMinimalMvc();
            services.AddElmahCore(Configuration, _siteSetting);
            //services.AddJwtAuthentication(_siteSetting.JwtSettings);
            services.AddControllers();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddCustomAuthentication(_siteSetting.authenticationSettings, _siteSetting.authenticationStudentSettings, Configuration);

            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "localhost:6379"; // آدرس Redis Server
            //    options.InstanceName = "MyAppCache_";
            //});



            //services.AddSingleton<IFieldModelRepository, FieldModelRepository>();
            //services.AddScoped<IFieldService, FieldService>();
            #region Comment
            //services.AddCustomElmah(Configuration, _siteSetting);
            //services.AddCustomApiVersioning();
            //services.AddSwagger();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SanjeshP.RDC", Version = "v1" });
            //    c.SwaggerDoc("v2", new OpenApiInfo { Title = "SanjeshP.RDC", Version = "v2" });
            //});
            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(
            //        builder =>
            //        {
            //            builder.WithOrigins("http://localhost:10053", "http://localhost:4200")
            //                                .AllowAnyHeader()
            //                                .AllowAnyMethod();
            //        });
            //});
            #endregion

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //Register Services to Autofac ContainerBuilder
            builder.AddServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            // برای زمانی که میخواهیم پابلیش بگیریم
            //app.UsePathBase("/UserPortal");

            app.UseStaticFiles();                    // 1️⃣ فایل‌های استاتیک
            app.InitializeDatabase();               // 2️⃣ مقداردهی اولیه
            app.UseCustomExceptionHandler();        // 3️⃣ هندل Exception‌ها
            app.UseHsts(env);                       // 4️⃣ امنیت
            app.UseHttpsRedirection();              // 5️⃣ ریدایرکت به HTTPS

            app.UseRouting();                       // 6️⃣ مسیردهی

            app.GenerateClientEnvJs();                  // 🔟 تولید خودکار فایل env.js برای کلاینت

            app.UseSession();                       // 7️⃣ سشن بعد از مسیر

            app.UseAuthentication();                // 8️⃣ احراز هویت
            app.UseMiddleware<CustomAuthorizationMiddleware>();
            app.UseAuthorization();                 // 9️⃣ مجوزها

            app.UseEndpoints(endpoints =>
            {
                // Auth area
                endpoints.MapAreaControllerRoute(
                    name: "auth",
                    areaName: "Auth",
                    pattern: "Auth/{controller=Home}/{action=Index}/{id?}");
                // DocumentVerification area
                endpoints.MapAreaControllerRoute(
                    name: "documentVerification",
                    areaName: "DocumentVerification",
                    pattern: "DV/{controller=Home}/{action=Index}/{id?}");
                // Exam area
                endpoints.MapAreaControllerRoute(
                    name: "exam",
                    areaName: "Exam",
                    pattern: "Exam/{controller=Home}/{action=Index}/{id?}");
                // Group area
                endpoints.MapAreaControllerRoute(
                    name: "group",
                    areaName: "Group",
                    pattern: "Group/{controller=Home}/{action=Index}/{id?}");
                // Home area
                endpoints.MapAreaControllerRoute(
                    name: "home",
                    areaName: "Home",
                    pattern: "Home/{controller=Home}/{action=Index}/{id?}");
                // Interview area
                endpoints.MapAreaControllerRoute(
                    name: "interview",
                    areaName: "Interview",
                    pattern: "Interview/{controller=Home}/{action=Index}/{id?}");
                // Menu area
                endpoints.MapAreaControllerRoute(
                    name: "menu",
                    areaName: "Menu",
                    pattern: "Menu/{controller=Home}/{action=Index}/{id?}");
                // Report area
                endpoints.MapAreaControllerRoute(
                    name: "report",
                    areaName: "Report",
                    pattern: "Report/{controller=Home}/{action=Index}/{id?}");
                // SiteSetting area
                endpoints.MapAreaControllerRoute(
                    name: "siteSetting",
                    areaName: "SiteSetting",
                    pattern: "SiteSetting/{controller=Home}/{action=Index}/{id?}");
                // Student area
                endpoints.MapAreaControllerRoute(
                    name: "student",
                    areaName: "Student",
                    pattern: "Student/{controller=Home}/{action=Index}/{id?}");

                // StudentForm area
                endpoints.MapAreaControllerRoute(
                    name: "studentForm",
                    areaName: "StudentForm",
                    pattern: "StudentForm/{controller=Home}/{action=Index}/{id?}");

                // User area
                endpoints.MapAreaControllerRoute(
                    name: "user",
                    areaName: "User",
                    pattern: "User/{controller=Home}/{action=Index}/{id?}");

                // مسیر مخصوص ورود بدون مشخص بودن controller/action (فقط root)
                endpoints.MapControllerRoute(
                    name: "root",
                    pattern: "",
                    defaults: new { action = "Login" });
                // مسیر پیش‌فرض برای باقی درخواست‌ها
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{action=Login}/{id?}");
            });
        }
    }
}

