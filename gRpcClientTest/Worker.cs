using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataServer;
using Microsoft.Extensions.Configuration;

namespace gRpcClientTest
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly Client client;
        private readonly Random random;
        public Worker(ILogger<Worker> logger,IConfiguration configuration)
        {
            this.logger = logger;
            random = new Random();
            client = new Client("https://localhost:7250", logger);
            client.DataResponseAsync += Client_DataResponseAsync;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting client streaming call at: {time}", DateTimeOffset.Now);
            // Don't pass cancellation token to the call. The call is completed in StopAsync when service stops.
            //_clientStreamingCall = _counterClient.AccumulateCount(cancellationToken: CancellationToken.None);
           
            await base.StartAsync(cancellationToken);
        }

        private Task Client_DataResponseAsync(Response reponse)
        {
            Console.WriteLine($"Got responsed data {reponse.Data}");
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Debug.Assert(_clientStreamingCall != null);
            var read = Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var count = random.Next(1, 10);

                    //Console.WriteLine("Sending count {count} at: {time}", count, DateTimeOffset.Now);
                    await client.SendRequstAsync(new Request { Data = $"{count}" });

                    await Task.Delay(1000);
                }
            }, stoppingToken);



            // Count until the worker exits
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    var count = _random.Next(1, 10);

            //    _logger.LogInformation("Sending count {count} at: {time}", count, DateTimeOffset.Now);
            //    await _clientStreamingCall.RequestStream.WriteAsync(new CounterRequest { Count = count });

            //    await Task.Delay(1000, stoppingToken);
            //}
            await client.RunAsync();
            

            //await read;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            //Debug.Assert(_clientStreamingCall != null);

            // Tell server that the client stream has finished
            //_logger.LogInformation("Finishing call at: {time}", DateTimeOffset.Now);
            //await _clientStreamingCall.RequestStream.CompleteAsync();
            // Log total
            //var response = await _clientStreamingCall.ResponseStream.ReadAllAsync();
            //_logger.LogInformation("Total count: {count}", response.Count);
            await client.StopAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
