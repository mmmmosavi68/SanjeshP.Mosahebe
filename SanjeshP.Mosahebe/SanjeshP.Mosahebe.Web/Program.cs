using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Web;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //UsingCodeConfiguration();
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                logger.Debug("init main");
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Flush();
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging(options => options.ClearProviders())
                .UseNLog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void UsingCodeConfiguration()
        {
            var config = new LoggingConfiguration();

            config.AddSentry(options =>
            {
                options.Layout = "${message}";
                options.BreadcrumbLayout = "${logger}: ${message}";
                options.MinimumBreadcrumbLevel = NLog.LogLevel.Debug;
                options.MinimumEventLevel = NLog.LogLevel.Error;
                options.AttachStacktrace = true;
                options.SendDefaultPii = true;
                options.IncludeEventDataOnBreadcrumbs = true;
                options.ShutdownTimeoutSeconds = 5;
                options.AddTag("logger", "${logger}");
                options.HttpProxy = new WebProxy("http://127.0.0.1:8118", true) { UseDefaultCredentials = true };
            });

            config.AddTarget(new DebuggerTarget("Debugger"));
            config.AddTarget(new ColoredConsoleTarget("Console"));

            config.AddRuleForAllLevels("Console");
            config.AddRuleForAllLevels("Debugger");

            LogManager.Configuration = config;
        }
    }
}
