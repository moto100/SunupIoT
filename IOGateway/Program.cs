// <copyright file="Program.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOGateway
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Program.
    /// </summary>
    internal class Program
    {
        private static string urls;
        private static string basePath;

        private static void Main(string[] args)
        {
            InitalizeConfig();
            CreateHostBuilder(args).Build().Run();
            Console.ReadLine();
        }

        private static void InitalizeConfig()
        {
            basePath = Directory.GetCurrentDirectory();
            Console.WriteLine("Current path : " + basePath);

            var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            urls = configuration.GetValue<string>("appSettings:ListenOn");
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            if (!string.IsNullOrEmpty(urls))
            {
                var urlsArr = urls.Split(",");
                if (urlsArr.Length > 0)
                {
                    webBuilder.UseUrls(urlsArr);
                    ////Logger.LogInfo($"[IO Server Host]Server will listen on {Urls}.");
                 }
                else
                {
                    webBuilder.UseUrls("http://*:5000");
                    ////Logger.LogInfo($"[IO Server Host]Server will listen on default url http://*:5000.");
                }
            }
            else
            {
                webBuilder.UseUrls("http://*:5000");
                ////Logger.LogInfo($"[IO Server Host]Server will listen on default url http://*:5000.");
             }

            webBuilder.UseStartup<Startup>();
        })
        .ConfigureLogging(builder =>
        {
            builder.ClearProviders();
            var log4path = Path.Combine(basePath, "log4net.config");
            Console.WriteLine("log4net.config path : " + log4path);
            builder.AddLog4Net(log4path);
        });
    }
}
