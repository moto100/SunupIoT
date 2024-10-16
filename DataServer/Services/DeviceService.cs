using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DataServer.Services
{
    public class DeviceService : Device.DeviceBase
    {
        private readonly ILogger<DeviceService> logger;
       
        public DeviceService(ILogger<DeviceService> logger)
        {
            this.logger = logger;
        }

        public override async Task DeviceControl(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
        {
            //return Task.FromResult(new HelloReply
            //{
            //    Message = "Hello " + request.Name
            //});
            var client = new Client(logger, requestStream, responseStream, context);
            client.DataRequestAsync += async request =>
            {
                logger.LogInformation($"Got data from client id {client.Id}, {request.Data}");
                await client.SendResponseAsync(new Response { Data = request.Data });
            };
            await client.RunAsync();
            //var httpContext = context.GetHttpContext();
            //var id = httpContext.Connection.Id;
            //_logger.LogInformation($"Connection id: {id}");
            //try
            //{
            //    await foreach (var request in requestStream.ReadAllAsync())
            //    {
            //        _logger.LogInformation($"Got data from client {id}, {request.Data}");
            //        await responseStream.WriteAsync(new Response { Data = request.Data });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex,null);
            //}
            //return new Response { Data = "done!" };
        }

        private Task Client_DataRequestAsync(Request request)
        {
            logger.LogInformation($"Got data from client {request.Data}");
            //client.SendResponseAsync(new Response { Data = request.Data });
            return Task.CompletedTask;
        }
    }
}
