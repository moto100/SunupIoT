// <copyright file="ClientSession.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.NetSocket
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Org.BouncyCastle.Bcpg;
    using Sunup.Diagnostics;

    /// <summary>
    /// SocketOptions.
    /// </summary>
    public class ClientSession
    {
        private Socket sessionSocket;
        private SocketAsyncEventArgs clientSendEventArg;
        private SocketAsyncEventArgs clientRecieveEventArg;
        ////private List<byte> readBytes = new List<byte>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSession"/> class.
        /// </summary>
        /// <param name="socket">socket.</param>
        /// <param name="clientRecieveEventArg">clientRecieveEventArg.</param>
        /// <param name="clientSendEventArg">clientSendEventArg.</param>
        public ClientSession(Socket socket, SocketAsyncEventArgs clientRecieveEventArg, SocketAsyncEventArgs clientSendEventArg)
        {
            this.sessionSocket = socket;
            this.clientSendEventArg = clientSendEventArg;
            this.clientRecieveEventArg = clientRecieveEventArg;
            this.clientSendEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(this.IO_Completed);
            this.clientRecieveEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(this.IO_Completed);
        }

        /// <summary>
        /// Close.
        /// </summary>
        /// <param name="client">c.</param>
        public delegate void Close(ClientSession client);

        /// <summary>
        /// ReceiveData.
        /// </summary>
        /// <param name="data">c.</param>
        /// <param name="client">client.</param>
        public delegate void ReceiveBytes(byte[] data, ClientSession client);

        /// <summary>
        /// SendData.
        /// </summary>
        /// <param name="client">client.</param>
        public delegate void SendBytes(ClientSession client);

        /// <summary>
        /// OnClose
        /// </summary>
        public event Close OnClose;

        /// <summary>
        /// OnReceiveBytes
        /// </summary>
        public event ReceiveBytes OnReceiveBytes;

        /// <summary>
        /// OnSendData
        /// </summary>
        public event SendBytes OnSendBytes;

        /// <summary>
        /// Gets or sets Socket.
        /// </summary>
        public Socket Socket
        {
            get
            {
                return this.sessionSocket;
            }

            set
            {
                this.sessionSocket = value;
                this.RemoteAddress = this.sessionSocket?.RemoteEndPoint.ToString();
            }
        }

        /// <summary>
        /// Gets RemoteAddress.
        /// </summary>
        public string RemoteAddress { get; private set; }

        /// <summary>
        /// ReceiveAsync.
        /// </summary>
        /// <exception cref="InvalidOperationException">InvalidOperationException.</exception>
        public void ReceiveAsync()
        {
            try
            {
                if (this.sessionSocket == null || !this.sessionSocket.Connected)
                {
                    this.CloseSocket();
                }

                if (!this.sessionSocket.ReceiveAsync(this.clientRecieveEventArg)) ////投递接收请求
                {
                    this.ProcessReceive(this.clientRecieveEventArg);
                }
            }
            catch (Exception ex)
            {
                this.CloseSocket();
                Logger.LogError($"[Socket Server] ClientSession:ReceiveAsync -> Fail to receive from client.", ex);
                Logger.LogTrace($"[Socket Server] ClientSession:ReceiveAsync -> Fail to receive from client.", ex);
            }
        }

        /// <summary>
        /// 异步的发送数据.
        /// </summary>
        /// <param name="data">data.</param>
        public void Send(byte[] data)
        {
            try
            {
                if (this.sessionSocket == null || !this.sessionSocket.Connected)
                {
                    this.CloseSocket();
                }

                if (this.clientSendEventArg.SocketError == SocketError.Success)
                {
                    ////AsyncUserToken token = (AsyncUserToken)e.UserToken;
                    ////Socket s = (Socket)token.Socket;
                    ////Socket s = e.AcceptSocket; ////和客户端关联的socket
                    ////对要发送的消息,制定简单协议,头4字节指定包的大小,方便客户端接收(协议可以自己定)
                    ////byte[] buff = new byte[data.Length + 4];
                    ////byte[] len = BitConverter.GetBytes(data.Length);
                    ////Array.Copy(len, 0, buff, 0, 4);
                    ////Array.Copy(data, 0, buff, 4, data.Length); ////设置发送数据
                    ////this.clientSendEventArg.SetBuffer(buff, 0, buff.Length); ////设置发送数据
                    this.clientSendEventArg.SetBuffer(data, 0, data.Length); ////设置发送数据
                                                                             ////Console.WriteLine($"Send->> e.Offset : {this.clientSendEventArg.Offset}; e.Buffer.length : {this.clientSendEventArg.Buffer.Length},data.Length {data.Length} ");

                    if (!this.sessionSocket.SendAsync(this.clientSendEventArg)) ////投递发送请求，这个函数有可能同步发送出去，这时返回false，并且不会引发SocketAsyncEventArgs.Completed事件
                    {
                        // 同步发送时处理发送完成事件
                        this.ProcessSend(this.clientSendEventArg);
                    }
                    ////else
                    ////{
                    ////    this.CloseClientSocket(e);
                    ////}
                }
            }
            catch (Exception ex)
            {
                this.CloseSocket();
                Logger.LogError($"[Socket Server] ClientSession:Send -> Fail to send message to client.", ex);
                Logger.LogTrace($"[Socket Server] ClientSession:Send -> Fail to send message to client.", ex);
            }
        }

        /// <summary>
        /// 异步的发送数据.
        /// </summary>
        /// <param name="e">与接收完成操作相关联的SocketAsyncEventArg对象.</param>
        public void Send(SocketAsyncEventArgs e)
        {
            try
            {
                if (this.sessionSocket == null || !this.sessionSocket.Connected)
                {
                    this.CloseSocket();
                }

                if (e.SocketError == SocketError.Success)
                {
                    ////AsyncUserToken token = (AsyncUserToken)e.UserToken;
                    ////Socket s = (Socket)token.Socket;
                    ////Socket s = e.AcceptSocket; ////和客户端关联的socket
                    if (!this.Socket.SendAsync(e)) ////投递发送请求，这个函数有可能同步发送出去，这时返回false，并且不会引发SocketAsyncEventArgs.Completed事件
                    {
                        // 同步发送时处理发送完成事件
                        this.ProcessSend(e);
                    }
                    ////else
                    ////{
                    ////    this.CloseClientSocket(e);
                    ////}
                }
            }
            catch (Exception ex)
            {
                this.CloseSocket();
                Logger.LogError($"[Socket Server] ClientSession:Send -> Fail to send message to client.", ex);
                Logger.LogTrace($"[Socket Server] ClientSession:Send -> Fail to send message to client.", ex);
            }
        }

        /// <summary>
        /// 接收完成时处理函数.
        /// </summary>
        /// <param name="e">与接收完成操作相关联的SocketAsyncEventArg对象.</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                if (this.sessionSocket == null || !this.sessionSocket.Connected)
                {
                    this.CloseSocket();
                }

                if (e.SocketError == SocketError.Success) ////if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    // 检查远程主机是否关闭连接.
                    if (e.BytesTransferred > 0)
                    {
                        ////AsyncUserToken token = (AsyncUserToken)e.UserToken;
                        ////token.ActiveDateTime = DateTime.Now;
                        ////Socket s = (Socket)token.Socket;
                        ////判断所有需接收的数据是否已经完成
                        ////if (this.Socket.Available == 0)
                        ////{
                        ////从侦听者获取接收到的消息
                        ////String received = Encoding.ASCII.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                        ////echo the data received back to the client
                        ////e.SetBuffer(e.Offset, e.BytesTransferred);

                        byte[] data = new byte[e.BytesTransferred];
                        Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred); ////从e.Buffer块中复制数据出来，保证它可重用
                        ////this.readBytes.AddRange(data);
                        ////Console.WriteLine($" e.Offset : {e.Offset}; e.Buffer.length : {e.Buffer.Length},data.Length {data.Length} ");
                        if (this.OnReceiveBytes != null)
                        {
                            this.OnReceiveBytes(data, this);
                            ////this.readBytes = new List<byte>();
                        }

                        ////TODO 处理数据 ,区分数据是否是转发 ,也可以处理成广播
                        ////IPEndPoint iep1 = new IPEndPoint(IPAddress.Any, 12030);
                        ////s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                        ////s.SendTo(data, iep1);

                        ////if (info == "Listens_all")
                        ////{
                        ////    try
                        ////    {
                        ////        //pc端的用户，存进全局列表
                        ////        dt.socketlist.Add(token);
                        ////    }
                        ////    catch { }
                        ////}
                        ////else
                        ////{
                        ////    //遍历pc端登陆的客户端，并发送消息
                        ////    for (int i = 0; i < dt.socketlist.Count; i++)
                        ////    {
                        ////        try
                        ////        {
                        ////            Socket tmp = ((AsyncUserToken)dt.socketlist[i]).Socket;
                        ////            tmp.Send(data);
                        ////        }
                        ////        catch { }

                        ////    }
                        ////}
                        ////增加服务器接收的总字节数
                        ////}
                        ////else
                        ////{
                        ////    byte[] data = new byte[e.BytesTransferred];
                        ////    Array.Copy(e.Buffer, e.Offset, data, 0, data.Length); ////从e.Buffer块中复制数据出来，保证它可重用
                        ////    this.readBytes.AddRange(data);
                        ////    ////Console.WriteLine($" e.Offset : {e.Offset}; e.Buffer.length : {e.Buffer.Length},data.Length {data.Length} ");
                        ////}

                        if (!this.sessionSocket.ReceiveAsync(e)) ////为接收下一段数据，投递接收请求，这个函数有可能同步完成，这时返回false，并且不会引发SocketAsyncEventArgs.Completed事件
                        {
                            ////同步接收时处理接收完成事件
                            this.ProcessReceive(e);
                        }
                    }
                    else
                    {
                        this.CloseSocket();
                    }
                }
                else
                {
                    this.CloseSocket();
                }
            }
            catch (Exception ex)
            {
                this.CloseSocket();
                Logger.LogError($"[Socket Server] ClientSession:ProcessReceive -> Fail to process message from client.", ex);
                Logger.LogTrace($"[Socket Server] ClientSession:ProcessReceive -> Fail to process message from client.", ex);
            }
        }

        /// <summary>
        /// 发送完成时处理函数.
        /// </summary>
        /// <param name="e">与发送完成操作相关联的SocketAsyncEventArg对象.</param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            try
            {
                if (this.sessionSocket == null || !this.sessionSocket.Connected)
                {
                    this.CloseSocket();
                }

                if (e.SocketError == SocketError.Success)
                {
                    ////AsyncUserToken token = (AsyncUserToken)e.UserToken;
                    ////Socket s = (Socket)token.Socket;
                    ////token.ActiveDateTime = DateTime.Now;
                    Logger.LogTrace($"[Socket Server] ProcessSend -> Sent data to client.");
                    if (this.OnSendBytes != null)
                    {
                        this.OnSendBytes(this);
                    }
                }
                else
                {
                    this.CloseSocket();
                }
            }
            catch (Exception ex)
            {
                this.CloseSocket();
                Logger.LogError($"[Socket Server] ClientSession:ProcessSend -> Fail to process end message from client.", ex);
                Logger.LogTrace($"[Socket Server] ClientSession:ProcessSend -> Fail to process end message from client.", ex);
            }
        }

        /// <summary>
        /// 当Socket上的发送或接收请求被完成时，调用此函数.
        /// </summary>
        /// <param name="sender">激发事件的对象.</param>.
        /// <param name="e">与发送或接收完成操作相关联的SocketAsyncEventArg对象.</param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            ////AsyncUserToken userToken = (AsyncUserToken)e.UserToken;
            ////userToken.ActiveDateTime = DateTime.Now;
            //// Determine which type of operation just completed and call the associated handler.
            try
            {
                ////lock (userToken)
                ////{
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Send:
                        this.ProcessSend(e);
                        break;
                    case SocketAsyncOperation.Receive:
                        this.ProcessReceive(e);
                        break;
                    default:
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
                ////}                ////}
            }
            catch (Exception ex)
            {
                this.CloseSocket();
                Logger.LogError($"[Socket Server] OnIOCompleted -> Fail to accept/receive from client.", ex);
                Logger.LogTrace($"[Socket Server] OnIOCompleted -> Fail to accept/receive from client.", ex);
            }
        }

        private void CloseSocket()
        {
            try
            {
                ////this.readBytes = new List<byte>();
                this.Socket?.Close();
                if (this.OnClose != null)
                {
                    this.OnClose(this);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Socket Server] CloseSocket -> Fail to close socket.", ex);
                Logger.LogTrace($"[Socket Server] CloseSocket -> Fail to close socket.", ex);
            }
        }
    }
}
