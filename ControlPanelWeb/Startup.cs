// <copyright file="Startup.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ControlPanelWeb
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Sunup.Diagnostics;

    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        /// <summary>
        /// ConfigureServices.
        /// </summary>
        /// <param name="services">services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new JsonContractResolver();
            });
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.Name = ".Sunup";
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
            });

            ////services.AddCors(option => option.AddPolicy("cors", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().AllowAnyOrigin()));
            services.AddCors(option => option.AddPolicy("cors", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
            ////Logger.LogInfo($"[IO Server Host]Configured services.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        /// <summary>
        /// Configure.
        /// </summary>
        /// <param name="app">app.</param>
        /// <param name="env">env.</param>
        /// <param name="loggerFactory">loggerFactory.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Sunup");
            Logger.MSLogger = logger;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Logger.LogInfo($"[IO Server Host]Configuring app.");
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("cors");

            ////app.UseAuthorization();
            ////app.UseWebSockets();
            ////app.Use(async (context, next) =>
            ////{
            ////    if (context.WebSockets.IsWebSocketRequest)
            ////    {
            ////        using (IServiceScope scope = app.ApplicationServices.CreateScope())
            ////        {
            ////            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            ////            await this.Echo(context, webSocket);
            ////        }
            ////    }
            ////    else
            ////    {
            ////        await next();
            ////    }
            ////});
            ////app.UseEndpoints(endpoints =>
            ////{
            ////    ////endpoints.MapControllerRoute(
            ////    ////    name: "default",
            ////    ////    pattern: "{controller=Home}/{action=Index}/{id?}");

            ////});
            app.UseSession();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            app.Map("/Service/ws", WebSocketService.Map);
            ////var basepath = Program.BasePath;
            ////////this.InitalizeConfig(Program.BasePath);
            ////if (!string.IsNullOrEmpty(basepath))
            ////{
            ////    IOServer.IOServerAgent.Init(basepath);
            ////}
            ControlPanel.AdminAgent.Init(string.Empty);
            Logger.LogInfo($"[IO Server Host]Configured app.");
        }
    }
}
