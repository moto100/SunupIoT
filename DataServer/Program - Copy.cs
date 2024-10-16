//using DeviceProxyServer.Services;
//using Microsoft.AspNetCore.Server.Kestrel.Core;
//using Microsoft.Extensions.Diagnostics.HealthChecks;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddGrpc();
////builder.WebHost.ConfigureKestrel(serverOptions =>
////{
////    serverOptions.ListenNamedPipe("MyPipeName", listenOptions =>
////    {
////        listenOptions.UseHttps();
////    });
////});
////builder.Services.Configure<HealthCheckPublisherOptions>(options =>
////{
////    options.Delay = TimeSpan.Zero;
////    options.Period = TimeSpan.FromSeconds(20);
////});
//var a = true;
////builder.Services.AddGrpcHealthChecks(o =>
////{
////    o.Services.Map("DeviceProxyServer.Greeter", r =>
////    {
////        if (a)
////            return true;
////        else
////            return false;

////    }
////    );

////});
////builder.Services.AddGrpcHealthChecks(o =>
////{
////    o.Services.Map("greet.Greeter", r => r.Tags.Contains("AAA"));

////})
//builder.Services.AddGrpcHealthChecks()
//  .AddCheck("", () =>
//{
//    if (a)
//        return HealthCheckResult.Healthy();
//    else
//        return HealthCheckResult.Unhealthy();

//});
//var app = builder.Build();

//// Configure the HTTP request pipeline.
//app.MapGrpcService<GreeterService>();
//app.MapGrpcHealthChecksService();
//app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

//app.Run();
