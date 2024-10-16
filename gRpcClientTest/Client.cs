using DataServer;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gRpcClientTest
{
    internal class Client: IDisposable
    {
        AsyncDuplexStreamingCall<Request, Response> clientStreamingCall = null;
        Func<Response, Task> dataResponseAsync = null;
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private string url;
        private ILogger logger;

        public event Func<Response, Task> DataResponseAsync
        {
            add
            {
                this.dataResponseAsync = value;
            }
            remove
            {
                this.dataResponseAsync = null;
            }
        }

        public bool IsRunning { get; private set; }
        public void Dispose()
        {
            cancellationToken?.Dispose();
        }


        public Client(string url, ILogger logger) {
            this.url = url;
            this.logger = logger;
        }

        public async Task RunAsync() {
            try
            {
                //IsRunning = true;
                var handler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                };
                using var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
                {
                    HttpHandler = handler
                });
                var client = new Device.DeviceClient(channel);
                this.clientStreamingCall = client.DeviceControl();
                var cancellationToken = this.cancellationToken.Token;
                //while (!cancellationToken.IsCancellationRequested)
                //{
                //    await Task.Yield();
                   
                  if (this.dataResponseAsync != null  && !cancellationToken.IsCancellationRequested)
                  {
                        await foreach (var reponse in clientStreamingCall.ResponseStream.ReadAllAsync(cancellationToken))
                        {

                            await this.dataResponseAsync(reponse).ConfigureAwait(false);
                        }
                  }
                //}
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, null);
                await Task.Delay(1000);
                await RunAsync();
            }
            //finally
            //{
            //    IsRunning = false;

            //    cancellationToken?.TryCancel();
            //    cancellationToken?.Dispose();
            //    cancellationToken = null;
            //}
        }
        public async Task SendRequstAsync(Request data) {
            try
            {
                var cancellationToken = this.cancellationToken.Token;
                if (clientStreamingCall != null && !cancellationToken.IsCancellationRequested)
                {
                    await clientStreamingCall.RequestStream.WriteAsync(data, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex) {
                this.logger.LogError(ex, null);
            }
        }

        public async Task StopAsync()
        {
            IsRunning = false;
            dataResponseAsync = null;
            if (clientStreamingCall != null )
            {
                await clientStreamingCall.RequestStream.CompleteAsync().ConfigureAwait(false);
            }

            StopInternal();
        }


        void StopInternal()
        {
            cancellationToken?.TryCancel();
        }
    }
}
 