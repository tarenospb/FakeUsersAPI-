using System.IO;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace FakeUsersAPI
{
    internal static class Program
    {
        public static void Main(string[] args)
        {

            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            var Host = WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(a => a
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .AddCommandLine(args)
                )
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog()
#if DEBUG
                .UseUrls("http://localhost:5577")
#else
                .UseUrls("http://0.0.0.0:5577")
#endif
                .UseStartup<Startup>()
                
                .Build();
            try
            {
                logger.Debug("init main");
                Host.Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }
    }
}
