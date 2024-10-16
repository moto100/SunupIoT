// <copyright file="Program.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServerHost
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
            AppId = "00000000-0000-0000-0000-000000000000";
            LogLevel = "Warning";
            if (args.Length > 0)
            {
                ProcessArgs(args);
            }

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
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.ConfigureAppConfiguration(configurationBuilder =>
                    {
                        configurationBuilder.AddJsonFile("IOServerHost_appsettings.json", optional: true, reloadOnChange: true);
                    })
                    .ConfigureLogging(loggngBuilder =>
                    {
                        loggngBuilder.ClearProviders();
                        var level = Microsoft.Extensions.Logging.LogLevel.Warning;
                        Enum.TryParse(LogLevel, true, out level);
                        loggngBuilder.AddFilter("Default", level);
                        loggngBuilder.AddFilter("Microsoft", level);
                        loggngBuilder.AddFilter("Microsoft.Hosting.Lifetime", level);
                        loggngBuilder.AddFilter("System", level);
                        ////loggngBuilder.AddFilter(AppId, level);
                        loggngBuilder.AddLog4Net("IOServerHost_log4net.config");
                    });
                    var currentPath = System.AppDomain.CurrentDomain.BaseDirectory;

                    builder.UseWebRoot(Path.Combine(currentPath, "IOServerHost_wwwroot"));
                    if (!string.IsNullOrEmpty(Urls))
                    {
                        var urls = Urls.Split(",");
                        if (urls.Length > 0)
                        {
                            builder.UseUrls(urls);
                            Logger.LogInfo($"[IO Server Host]Server will listen on {Urls}.");
                        }
                    }

                    builder.UseStartup<Startup>();
                });

        private static void ProcessArgs(params string[] args)
        {
            if (args != null && args.Length > 0)
            {
                foreach (var param in args)
                {
                    var spliterIndex = param.IndexOf(":", 0, StringComparison.CurrentCultureIgnoreCase);
                    if (spliterIndex > 0 && spliterIndex < param.Length - 1)
                    {
                        ProcessArg(param.Substring(0, spliterIndex), param.Substring(spliterIndex + 1));
                    }
                }
            }
        }

        private static void ProcessArg(string argName, string argContent)
        {
            if (argName == null || argContent == null)
            {
                return;
            }

            argName = argName.Trim();
            argContent = argContent.Trim();
            if (argContent.StartsWith("'"))
            {
                argContent = argContent.Remove(0, 1);
            }

            if (argContent.EndsWith("'"))
            {
                argContent = argContent.Remove(argContent.Length - 1);
            }

            switch (argName.ToUpperInvariant())
            {
                case "APPID":
                    {
                        AppId = argContent;
                        break;
                    }

                case "BASEPATH":
                    {
                        BasePath = argContent;
                        break;
                    }

                case "URLS":
                    {
                        Urls = argContent;
                        break;
                    }

                case "ENABLEHTTPS":
                    {
                        bool.TryParse(argContent, out bool ret);
                        EnableHttps = ret;
                        break;
                    }

                case "CERTIFICATE":
                    {
                        Certificate = argContent;
                        break;
                    }

                case "CERTIFICATEPWD":
                    {
                        CertificatePWD = argContent;
                        break;
                    }

                case "LOGLEVEL":
                    {
                        LogLevel = argContent;
                        break;
                    }

                    ////case "HTTPSPORT":
                    ////    {
                    ////        int.TryParse(argContent, out int ret);
                    ////        HttpsPort = ret;
                    ////        break;
                    ////    }
            }
        }
    }
}
