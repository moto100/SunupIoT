
//using DeviceProxyServer;
//using Grpc.Health.V1;
//using Grpc.Net.Client;
//using gRpcClientTest;
//using System;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//var handler = new SocketsHttpHandler
//{
//    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
//    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
//    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
//    EnableMultipleHttp2Connections = true
//};
//using var channel =  GrpcChannel.ForAddress("https://localhost:7250", new GrpcChannelOptions
//{
//    HttpHandler = handler
//});

//var client = new Greeter.GreeterClient(channel);

//var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
//Console.WriteLine("Greeting: " + reply.Message);

////await Task.Delay(10 * 1000);
//var channel1 = GrpcChannel.ForAddress("https://localhost:7250");
//var client1 = new Health.HealthClient(channel1);

//var response = await client1.CheckAsync(new HealthCheckRequest());
//var status = response.Status;

//Console.WriteLine("Shutting down");
//Console.WriteLine("Press any key to exit...");
//Console.ReadKey();



//static GrpcChannel CreateChannel()
//{
//    var connectionFactory = new NamedPipesConnectionFactory("MyPipeName");
//    var socketsHttpHandler = new SocketsHttpHandler
//    {
//        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
//        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
//        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
//        EnableMultipleHttp2Connections = true,
//        ConnectCallback = connectionFactory.ConnectAsync
//    };

//    return GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions
//    {
//        HttpHandler = socketsHttpHandler,
//    });
//}