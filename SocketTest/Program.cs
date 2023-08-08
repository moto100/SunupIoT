using System;
using Sunup.DataSource.NetSocket;
namespace SocketTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var options = new SocketOptions
            {
                Port = 9002,
                Server = "192.168.36.129",
                //Server = "192.168.31.236",
                MaxNumConnections = 10000,
                ReceiveBufferSize = 1024
            };
            var server = new SocketServer("socketTest", options);
            server.Run();
            Console.WriteLine("Start to accept connection...");
            Console.ReadLine();
        }
    }
}
