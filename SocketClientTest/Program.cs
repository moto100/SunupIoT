using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SocketClientTest
{
    internal class Program
    {
        private static byte[] result = new byte[1024];
        //static string IP = "192.168.31.236";
        static string IP = "192.168.36.129";
        static int Port = 9002;
        static int id = 0;
        static string sentMessage = "##0098ST=3";
       
        static int clientCount = 10000;
        static void Main(string[] args)
        {
            ////Console.OutputEncoding = Encoding.UTF8;
            for(int  i =0; i< clientCount; i++)
            {
                ////new Thread(SendMessage).Start();
                var id = i.ToString() + " >>> ";
                Task.Run(() =>
                {
                    StartSocketClientConnection(id);
                });
            }
            Console.ReadLine();
        }

        private static void StartSocketClientConnection(string id) {
            //设定服务器IP地址  
            ////Interlocked.Increment(ref id);
            
            IPAddress ip = IPAddress.Parse(IP);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var receiverSocketAsyncEventArgs = new SocketAsyncEventArgs();
            receiverSocketAsyncEventArgs.Completed += IO_Completed;
            receiverSocketAsyncEventArgs.SetBuffer(new byte[1024]);
            //var senderSocketAsyncEventArgs = new SocketAsyncEventArgs();
            //senderSocketAsyncEventArgs.Completed += IO_Completed;
            
            //senderSocketAsyncEventArgs.SetBuffer(sentBytes,0, sentBytes.Length);
            //string sendMessage = id + "##0098ST=3";
            //var send = Encoding.UTF8.GetBytes(sendMessage);
            try
            {
                
                clientSocket.Connect(new IPEndPoint(ip, Port)); //配置服务器IP与端口  
                receiverSocketAsyncEventArgs.UserToken = clientSocket;
               //senderSocketAsyncEventArgs.UserToken = clientSocket;
                if (!clientSocket.ReceiveAsync(receiverSocketAsyncEventArgs))
                {
                    ProcessReceive(receiverSocketAsyncEventArgs);
                }
                Console.WriteLine(id + " connect to server successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(id + "Failed to connect to server！");
                return;
            }

            //if (!clientSocket.SendAsync(senderSocketAsyncEventArgs))
            //{
            //    ProcessSend(senderSocketAsyncEventArgs);
            //}
            //通过clientSocket接收数据  
            //int receiveLength = clientSocket.Receive(result);
            //Console.WriteLine("接收服务器消息：{0}", Encoding.ASCII.GetString(result, 0, receiveLength));
            //通过 clientSocket 发送数据  
            //for (int i = 0; i < 1000; i++)
            var temp = $"client :{id} sent message to  server : {sentMessage}";
            while (true)
            {
                try
                {
                    ///Task.Delay(1000);
                    Thread.Sleep(100);
                    byte[] sentBytes = Encoding.UTF8.GetBytes(temp);
                    clientSocket.Send(sentBytes, SocketFlags.None);
                    ////Console.WriteLine(id + "Sent message to server ：{0}", sendMessage);
                }
                catch
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    break;
                }
            }
            //Console.WriteLine(id + "Finished");

        }

      

        private static void ProcessSend(SocketAsyncEventArgs e)
        {
            var client = e.UserToken as Socket;
            if (e.SocketError == SocketError.Success)
            {
                
                if (client != null)
                {
                    byte[] sentBytes = Encoding.UTF8.GetBytes(sentMessage);
                    e.SetBuffer(sentBytes, 0, sentBytes.Length);
                    if (!client.SendAsync(e))
                    {
                        ProcessSend(e);
                    }
                }
            }
            else
            {
                CloseSocket(e.ConnectSocket);
            }
        }

        private static void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            ////AsyncUserToken userToken = (AsyncUserToken)e.UserToken;
            ////userToken.ActiveDateTime = DateTime.Now;
            //// Determine which type of operation just completed and call the associated handler.
           
                ////lock (userToken)
                ////{
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Send:
                        ProcessSend(e);
                        break;
                    case SocketAsyncOperation.Receive:
                        ProcessReceive(e);
                        break;
                    default:
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
                ////}                ////}
           
        }

        private void SendEessage(SocketAsyncEventArgs e)
        { 
        }

        
        private static void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success) ////if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                // 检查远程主机是否关闭连接.
                if (e.BytesTransferred > 0)
                {
                    ////判断所有需接收的数据是否已经完成
                    //if (clientSocket.Available == 0)
                    //{
                        ////从侦听者获取接收到的消息

                        byte[] data = new byte[e.BytesTransferred];
                        Array.Copy(e.MemoryBuffer.ToArray(), e.Offset, data, 0, data.Length); ////从e.Buffer块中复制数据出来，保证它可重用
                        //readBytes.AddRange(data);
                        
                        //readBytes = new List<byte>();
                        string info = Encoding.UTF8.GetString(data);
                        //Console.WriteLine($" Received message from server. {info}");

                    //}
                    ////else
                    ////{
                    ////    byte[] data = new byte[e.BytesTransferred];
                    ////    Array.Copy(e.Buffer, e.Offset, data, 0, data.Length); ////从e.Buffer块中复制数据出来，保证它可重用
                    ////    readBytes.AddRange(data);
                    ////    ////Console.WriteLine($" e.Offset : {e.Offset}; e.Buffer.length : {e.Buffer.Length},data.Length {data.Length} ");
                    ////}
                    var client = e.UserToken as Socket;
                    if (client != null)
                    {
                        if (!client.ReceiveAsync(e)) ////为接收下一段数据，投递接收请求，这个函数有可能同步完成，这时返回false，并且不会引发SocketAsyncEventArgs.Completed事件
                        {
                            ////同步接收时处理接收完成事件
                            ProcessReceive(e);
                        }
                    }
                }
            //    else
            //    {
            //        CloseSocket(clientSocket);
            //    }
            }
            //else
            //{
            //    CloseSocket(clientSocket);
            //}
        }
        private static void CloseSocket(Socket clientSocket)
        {
            clientSocket?.Close();
            
        }
    }
}
