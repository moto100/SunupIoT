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
            InitalizeConfig();
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// CreateWebHostBuilder.
        /// </summary>
        /// <param name="args">args.</param>
        /// <returns>IWebHostBuilder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
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
                    loggngBuilder.AddFilter(AppId, level);
                    loggngBuilder.AddLog4Net("ControlPanelWeb_log4net.config");
                })
                ////.UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
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

                    if (EnableHttps && !string.IsNullOrEmpty(Certificate) && !string.IsNullOrEmpty(CertificatePWD))
                    {
                        webBuilder.ConfigureKestrel((context, serverOptions) =>
                      {
                          serverOptions.ConfigureHttpsDefaults(configureOption =>
                          {
                              configureOption.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(Path.Combine(currentPath, Certificate), CertificatePWD);
                          });
                      });
                    }

                    webBuilder.UseStartup<Startup>();
                });

        private static void InitalizeConfig()
        {
            var builder = new ConfigurationBuilder()
            .AddJsonFile("ControlPanelWeb_appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();

            var defaultAppPath = configuration.GetValue<string>("appSettings:DefaultAppPath");
            BasePath = defaultAppPath;
            Urls = configuration.GetValue<string>("appSettings:ListenOn");

            EnableHttps = configuration.GetValue<bool>("HTTPS:Enable");
            Certificate = configuration.GetValue<string>("HTTPS:Certificate");

            CertificatePWD = configuration.GetValue<string>("HTTPS:CertificatePWD");

            ////HttpsPort = configuration.GetValue<int>("HTTPS:HttpsPort");

            ControlPanel.Config.DbConnectionString = configuration.GetConnectionString("DbConnection");
            ////var enableLogTrace = configuration.GetValue<bool>("log:EnableLogTrace");
            ////Logger.EnableLogTrace = enableLogTrace;
            ////var enableLogError = configuration.GetValue<bool>("log:EnableLogError");
            ////Logger.EnableLogError = enableLogError;
            ////var enableLogInfo = configuration.GetValue<bool>("log:EnableLogInfo");
            ////Logger.EnableLogInfo = enableLogInfo;
            ////var enableLogWarning = configuration.GetValue<bool>("log:EnableLogWarning");
            ////Logger.EnableLogWarning = enableLogWarning;
            ////var enableConsoleLog = configuration.GetValue<bool>("log:ConsoleListener:Enable");
            ////if (enableConsoleLog)
            ////{
            ////    IListener listener = new ConsoleListener();
            ////    listener.EnableLogTrace = configuration.GetValue<bool>("log:ConsoleListener:EnableLogTrace");
            ////    listener.EnableLogError = configuration.GetValue<bool>("log:ConsoleListener:EnableLogError");
            ////    listener.EnableLogInfo = configuration.GetValue<bool>("log:ConsoleListener:EnableLogInfo");
            ////    listener.EnableLogWarning = configuration.GetValue<bool>("log:ConsoleListener:EnableLogWarning");
            ////    ListenerManager.Instance.AddListener("ConsoleListener", listener);
            ////    listener.Start();
            ////}

            ////var enableFileLog = configuration.GetValue<bool>("log:FileListener:Enable");
            ////if (enableFileLog)
            ////{
            ////    IListener listener = new FileListener(defaultAppPath);
            ////    listener.EnableLogTrace = configuration.GetValue<bool>("log:FileListener:EnableLogTrace");
            ////    listener.EnableLogError = configuration.GetValue<bool>("log:FileListener:EnableLogError");
            ////    listener.EnableLogInfo = configuration.GetValue<bool>("log:FileListener:EnableLogInfo");
            ////    listener.EnableLogWarning = configuration.GetValue<bool>("log:FileListener:EnableLogWarning");
            ////    ListenerManager.Instance.AddListener("FileListener", listener);
            ////    listener.Start();
            ////}
        }
    }
}
