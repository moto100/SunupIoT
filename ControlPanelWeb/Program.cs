// <copyright file="Program.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ControlPanelWeb
{
    using System;
    using System.IO;
    using System.Net;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Sunup.Diagnostics;

    /// <summary>
    /// Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets a value indicating whether EnableHttps.
        /// </summary>
        public static bool EnableHttps { get; private set; }

        /// <summary>
        /// Gets Certificate.
        /// </summary>
        public static string Certificate { get; private set; }

        /// <summary>
        /// Gets CertificatePWD.
        /// </summary>
        public static string CertificatePWD { get; private set; }

        /// <summary>
        /// Gets LogLevel.
        /// </summary>
        public static string LogLevel { get; private set; }

        /////// <summary>
        /////// Gets HttpsPort.
        /////// </summary>
        ////public static int HttpsPort { get; private set; }

        /// <summary>
        /// Gets AppId.
        /// </summary>
        public static string AppId { get; private set; }

        /// <summary>
        /// Gets basePath.
        /// </summary>
        public static string BasePath { get; private set; }

        /// <summary>
        /// Gets DBConnectionString.
        /// </summary>
        public static string DBConnectionString { get; private set; }

        /// <summary>
        /// Gets urls.
        /// </summary>
        public static string Urls { get; private set; }

        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args"> args.</param>
        public static void Main(string[] args)
        {
            AppId = "Sunup.ControlPanel";
            LogLevel = "Warning";
            var host = CreateHostBuilder(args).Build();
            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            Logger.MSLogger = loggerFactory.CreateLogger("Sunup");
            host.Run();
        }

        /// <summary>
        /// CreateWebHostBuilder.
        /// </summary>
        /// <param name="args">args.</param>
        /// <returns>IWebHostBuilder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                ////.UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                      {
                          configurationBuilder.AddJsonFile("ControlPanelWeb_appsettings.json", optional: true, reloadOnChange: true);
                      })
                    .ConfigureLogging(loggngBuilder =>
                        {
                            loggngBuilder.ClearProviders();
                            var level = Microsoft.Extensions.Logging.LogLevel.Warning;
                            Enum.TryParse<Microsoft.Extensions.Logging.LogLevel>(LogLevel, true, out level);
                            loggngBuilder.AddFilter("Default", level);
                            loggngBuilder.AddFilter("Microsoft", level);
                            loggngBuilder.AddFilter("Microsoft.Hosting.Lifetime", level);
                            loggngBuilder.AddFilter("System", level);
                            loggngBuilder.AddLog4Net("ControlPanelWeb_log4net.config");
                        });
                    var currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
                    webBuilder.UseWebRoot(Path.Combine(currentPath, "wwwroot"));
                    if (!string.IsNullOrEmpty(Urls))
                    {
                        var urls = Urls.Split(",");
                        if (urls.Length > 0)
                        {
                            webBuilder.UseUrls(urls);
                            ////Logger.LogInfo($"[IO Server Host]Server will listen on {Urls}.");
                        }
                    }

                    webBuilder.UseStartup<Startup>();
                });
    }
}
