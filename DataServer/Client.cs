using DataServer.Services;
using Grpc.Core;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataServer
{
    public class Client
    {
        private ServerCallContext context;
        private ILogger logger;
        private IAsyncStreamReader<Request> requestStream;
        private IServerStreamWriter<Response> responseStream;
        Func<Request, Task> dataRequestAsync = null;
        CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public event Func<Request, Task> DataRequestAsync
        {
            add
            {
                this.dataRequestAsync = value;
            }
            remove
            {
                this.dataRequestAsync = null;
            }
        }
        public bool IsRunning { get; private set; }
        public string Id { get; private set; }

        public Client(ILogger logger, IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context) {
            this.requestStream = requestStream;
            this.responseStream = responseStream;
            this.context  = context;
            this.logger = logger;
            var httpContext = context.GetHttpContext();
            this.Id = httpContext.Connection.Id;
        }

        public async Task RunAsync()
        {
            var cancellationToken = _cancellationToken.Token;
            if (this.dataRequestAsync != null && !cancellationToken.IsCancellationRequested)
            {
                await foreach (var request in requestStream.ReadAllAsync(cancellationToken))
                {
                    await this.dataRequestAsync(request).ConfigureAwait(false);
                    //await SendResponseAsync(new Response { Data = request.Data });
                }
            }

            //await foreach (var request in requestStream.ReadAllAsync())
            //{
            //    logger.LogInformation($"Got data from client {id}, {request.Data}");
            //    await responseStream.WriteAsync(new Response { Data = request.Data });
            //}
        }

        public async Task SendResponseAsync(Response data)
        {
            try
            {
                var cancellationToken = _cancellationToken.Token;
                if (responseStream != null && !cancellationToken.IsCancellationRequested)
                {
                    await responseStream.WriteAsync(data, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, null);
            }
        }
    }
}
