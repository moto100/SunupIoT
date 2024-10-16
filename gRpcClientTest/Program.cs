
using DataServer;
using Grpc.Core;
using Grpc.Health.V1;
using Grpc.Net.Client;
using gRpcClientTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;



////Client client = new Client("https://localhost:7250",null);

////client.DataResponseAsync += Client_DataResponseAsync;

////Task Client_DataResponseAsync(Response reponse)
////{
////    Console.WriteLine($"Got responsed data {reponse.Data}");
////    return Task.CompletedTask;
////}

////var task= Task.Run(async () => await client.RunAsync());

////var _random = new Random();
////while (true)
////{
////    var count = _random.Next(1, 10);

////    //Console.WriteLine("Sending count {count} at: {time}", count, DateTimeOffset.Now);
////    await client.SendRequstAsync(new Request { Data =$"{count}" });

////    await Task.Delay(1000);
////}

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();
//var counterClient = host.Services.GetRequiredService<Counter.CounterClient>();
host.Run();


Console.WriteLine("Shutting down");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

