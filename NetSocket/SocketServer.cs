// <copyright file="SocketServer.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.NetSocket
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Jint.Runtime;
    using Sunup.Contract;
    using Sunup.Diagnostics;

    /// <summary>
    /// SocketServer.
    /// </summary>
    public class SocketServer : DataSource
    {
        private readonly object acceptLocker = new object();

        private int port;
        private string serverAddress;
        private string password;
        private string user;
        ////private bool retained;
        private string[] items;
        ////private int quality;
        private object lockObject = new object();
        private int maxNumConnections;   // the maximum number of connections the sample is designed to handle simultaneously
        private int receiveBufferSize; // buffer size to use for each socket I/O operation
        private BufferManager bufferManager;  // represents a large reusable set of buffers for all socket operations
        private Socket listenSocket;            // the socket used to listen for incoming connection requests
        //// pool of reusable ClientSession objects for write, read and accept socket operations
        private ClientSessionPool clientSessionPool;
        private int numConnectedSockets;      // the total number of clients connected to the server
        private Semaphore numberAcceptedClients;
        private ClientSessionList socketUserTokenList;
        ////private DaemonThread daemonThread;
        private int socketTimeOutMS = 1000;
        ////private Dictionary<string, DataItem> itemsDictionary;
        ////private Dictionary<string, string[]> publishedItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServer"/> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="options">options.</param>
        /// <param name="items">items.</param>
        public SocketServer(string name, SocketOptions options, params string[] items)
            : base(name)
        {
            this.SourceType = DataSourceType.Socket;
            this.serverAddress = options.Server;
            this.port = options.Port;
            this.user = options.User;
            this.password = options.Password;
            this.items = items;

            this.numConnectedSockets = 0;
            this.maxNumConnections = options.MaxNumConnections;
            this.receiveBufferSize = options.ReceiveBufferSize;
            //// allocate buffers such that the maximum number of sockets can have one outstanding read and
            ////write posted to the socket simultaneously
            this.bufferManager = new BufferManager(
                this.receiveBufferSize * this.maxNumConnections,
                this.receiveBufferSize);

            this.clientSessionPool = new ClientSessionPool(this.maxNumConnections);
            this.socketUserTokenList = new ClientSessionList();
            this.numberAcceptedClients = new Semaphore(this.maxNumConnections, this.maxNumConnections);
        }

        /// <summary>
        /// Gets or sets a value indicating whether running.
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether running.
        /// </summary>
        public ClientSessionList SocketUserTokenList
        {
            get { return this.socketUserTokenList; }
            set { this.socketUserTokenList = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether running.
        /// </summary>
        public int SocketTimeOutMS
        {
            get { return this.socketTimeOutMS; }
            set { this.socketTimeOutMS = value; }
        }

        /// <summary>
        /// 初始化函数.
        /// </summary>
        public void Init()
        {
            try
            {
                // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds
                // against memory fragmentation
                this.bufferManager.InitBuffer();

                // preallocate pool of SocketAsyncEventArgs objects
                //// SocketAsyncEventArgs readWriteEventArg;

                for (int i = 0; i < this.maxNumConnections; i++)
                {
                    ////////Pre-allocate a set of reusable SocketAsyncEventArgs
                    ////readWriteEventArg = new SocketAsyncEventArgs();

                    ////// assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                    ////if (this.bufferManager.SetBuffer(readWriteEventArg))
                    ////{
                    ////    readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(this.IO_Completed);
                    ////    readWriteEventArg.UserToken = new AsyncUserToken();
                    ////    //// add SocketAsyncEventArg to the pool
                    ////    this.readWritePool.Push(readWriteEventArg);
                    ////}

                    var clientSendEventArg = new SocketAsyncEventArgs();
                    var clientRecieveEventArg = new SocketAsyncEventArgs();

                    if (this.bufferManager.SetBuffer(clientRecieveEventArg))
                    {
                        var client = new ClientSession(null, clientRecieveEventArg, clientSendEventArg);
                        client.OnClose += this.Client_OnClose;
                        client.OnReceiveBytes += this.Client_OnReceiveBytes;
                        client.OnSendBytes += this.Client_OnSendBytes;
                        this.clientSessionPool.Push(client);
                    }
                }

                Logger.LogTrace($"[Socket Server] Init -> Initialized BufferManager and created {this.maxNumConnections} SocketAsyncEventArgs in pool.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Socket Server] Init -> Fail to initialize server.", ex);
                Logger.LogTrace($"[Socket Server] Init -> Fail to initialize server.", ex);
            }
        }

        /// <summary>
        /// Run data source.
        /// </summary>
        public override void Start()
        {
            try
            {
                if (!this.IsRunning)
                {
                    this.Init();
                    this.IsRunning = true;
                    var ip = IPAddress.Parse(this.serverAddress);
                    var endPoint = new IPEndPoint(ip, this.port);
                    //// 创建监听socket.
                    this.listenSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        // 配置监听socket为 dual-mode (IPv4 & IPv6)
                        // 27 is equivalent to IPV6_V6ONLY socket option in the winsock snippet below,
                        this.listenSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                        this.listenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, endPoint.Port));
                    }
                    else
                    {
                        this.listenSocket.Bind(endPoint);
                    }

                    // 开始监听
                    this.listenSocket.Listen(this.maxNumConnections);
                    Logger.LogTrace($"[Socket Server] Run -> Start to listen.");
                    //// 在监听Socket上投递一个接受请求
                    ////this.StartAccept(null);
                    SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
                    acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(this.AcceptEventArg_Completed);
                    this.StartAccept(acceptEventArg);

                    Logger.LogTrace($"[Socket Server] Run -> Start to accept sockect.");
                    ////不断执行检查是否有无效连接
                    ////this.daemonThread = new DaemonThread(this);
                }
            }
            catch (Exception ex)
            {
                this.Stop();

                Logger.LogError($"[Socket Server]Socket Server fail to stop.", ex);
                Logger.LogTrace($"[Socket Server]Socket Server fail to stop.", ex);
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public override async void Stop()
        {
            if (this.listenSocket == null)
            {
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    if (this.IsRunning)
                    {
                        this.IsRunning = false;
                        this.listenSocket.Close();
                    }
                });
                Logger.LogInfo("[Socket Server]Socket Server is stopped.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Socket Server]Socket Server fail to stop.", ex);
                Logger.LogTrace($"[Socket Server]Socket Server fail to stop.", ex);
            }
        }

        /// <summary>
        /// WriteItem.
        /// </summary>
        /// <param name="dataToWrite">dataToWrite.</param>
        /// <param name="item">item.</param>
        public override void Publish(Dictionary<string, dynamic> dataToWrite, WriteItem item)
        {
            ////var publishedItem = this.publishedItems.FirstOrDefault(topic => topic.Value.Contains(item.BoundField));
            ////if (!string.IsNullOrEmpty(publishedItem.Key))
            ////{
            ////     var json = string.Empty;
            ////     try
            ////     {
            ////         json = JsonSerializer.Serialize(dataToWrite);
            ////     }
            ////     catch (Exception ex)
            ////     {
            ////         Logger.LogError("[MQTT Server] Write >>Fail to serialize published data object.", ex);
            ////         Logger.LogTrace("[MQTT Server] Write >>Fail to serialize published data object.", ex);
            ////     }

            ////     if (!string.IsNullOrEmpty(json))
            ////     {
            ////         ////this.Publish(publishedItem.Key, json);
            ////     }
            //// }
            ////else
            ////{
            ////     Logger.LogWarning("[MQTT Server] Write >>Didn't find matched topic.");
            //// }
            var json = string.Empty;
            try
            {
                json = JsonSerializer.Serialize(dataToWrite);
            }
            catch (Exception ex)
            {
                Logger.LogError("[Socket Server] Publish >>Fail to serialize published data object.", ex);
                Logger.LogTrace("[Socket Server] Publish >>Fail to serialize published data object.", ex);
            }

            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    byte[] data = Encoding.UTF8.GetBytes(json);
                    ////byte[] data = new byte[] { 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65, 0X65 };
                    byte[] buff = new byte[data.Length + 4];
                    byte[] len = BitConverter.GetBytes(data.Length);
                    Array.Copy(len, 0, buff, 0, 4);
                    Array.Copy(data, 0, buff, 4, data.Length); ////设置发送数据
                    var sendEventArg = new SocketAsyncEventArgs();
                    sendEventArg.SetBuffer(buff, 0, buff.Length); ////设置发送数据
                    ClientSession[] userTokenArray = null;
                    this.socketUserTokenList.CopyList(ref userTokenArray);
                    Parallel.ForEach(userTokenArray, session =>
                    {
                        session.Send(sendEventArg);
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[Socket Server] Publish >>Fail to send out data object.", ex);
                Logger.LogTrace("[Socket Server] Publish >>Fail to send out data object.", ex);
            }
        }

        /// <summary>
        /// ValidateWrite.
        /// </summary>
        /// <param name="item">item.</param>
        /// <returns>bool.</returns>
        public override bool ValidateTobePublishedItem(WriteItem item)
        {
            return true;
        }

        /////// <summary>
        /////// 关闭socket连接.
        /////// </summary>
        /////// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
        ////public void CloseClientSocket(SocketAsyncEventArgs e)
        ////{
        ////    ////AsyncUserToken token = e.UserToken as AsyncUserToken;
        ////    if (client == null)
        ////    {
        ////        e.AcceptSocket?.Close();

        ////        Interlocked.Decrement(ref this.numConnectedSockets);
        ////        this.numberAcceptedClients.Release(); ////释放线程信号量
        ////        this.clientSessionPool.Push(e); ////SocketAsyncEventArg 对象被释放，压入可重用队列。
        ////        this.socketUserTokenList.Remove(e); ////去除正在连接的用户
        ////        Logger.LogTrace($"[Socket Server] CloseClientSocket -> Closed socket from client.");
        ////        Console.WriteLine($"Connection number is decreasing, Current connection number is : {this.numConnectedSockets}");
        ////        return;
        ////    }

        ////    if (e.SocketError == SocketError.OperationAborted || e.SocketError == SocketError.ConnectionAborted)
        ////    {
        ////        return;
        ////    }

        ////    Socket s = token.Socket as Socket;
        ////    this.CloseClientSocket(s, e);
        ////}

        /////// <summary>
        /////// 同步的使用socket发送数据.
        /////// </summary>
        /////// <param name="socket">socket.</param>
        /////// <param name="buffer">buffer.</param>
        /////// <param name="offset">offset.</param>
        /////// <param name="size">size.</param>
        /////// <param name="timeout">timeout.</param>
        ////public void Send(Socket socket, byte[] buffer, int offset, int size, int timeout)
        ////{
        ////    socket.SendTimeout = 0;
        ////    int startTickCount = Environment.TickCount;
        ////    int sent = 0; // how many bytes is already sent.
        ////    do
        ////    {
        ////        if (Environment.TickCount > startTickCount + timeout)
        ////        {
        ////            ////throw new Exception("Timeout.");
        ////        }

        ////        try
        ////        {
        ////            sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
        ////        }
        ////        catch (SocketException ex)
        ////        {
        ////            if (ex.SocketErrorCode == SocketError.WouldBlock ||
        ////            ex.SocketErrorCode == SocketError.IOPending ||
        ////            ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
        ////            {
        ////                // socket buffer is probably full, wait and try again
        ////                Thread.Sleep(30);
        ////            }
        ////            else
        ////            {
        ////                throw ex; // any serious error occurr
        ////            }
        ////        }
        ////    }
        ////    while (sent < size);
        ////}

        /// <summary>
        /// 关闭socket连接.
        /// </summary>
        /// <param name="client">client.</param>
        private void Client_OnClose(ClientSession client)
        {
            try
            {
                client.Socket?.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Socket Server] Client_OnClose -> Fail to close socket.", ex);
                Logger.LogTrace($"[Socket Server] Client_OnClose -> Fail to close socket.", ex);
            }
            finally
            {
                client.Socket?.Close();
                client.Socket = null;
            }

            try
            {
                Interlocked.Decrement(ref this.numConnectedSockets);
                this.numberAcceptedClients.Release(); ////释放线程信号量
                this.clientSessionPool.Push(client); ////SocketAsyncEventArg 对象被释放，压入可重用队列。
                this.socketUserTokenList.Remove(client); ////去除正在连接的用户
                Console.WriteLine($"Connection number is decreasing, Current connection number is : {this.numConnectedSockets}");
                Logger.LogTrace($"[Socket Server] Client_OnClose -> Closed socket from client.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Socket Server] Client_OnClose -> Fail to release socket resource.", ex);
                Logger.LogTrace($"[Socket Server] Client_OnClose -> Fail to release socket resource.", ex);
            }
        }

        private void Client_OnSendBytes(ClientSession client)
        {
        }

        private void Client_OnReceiveBytes(byte[] data, ClientSession client)
        {
            try
            {
                string info = Encoding.UTF8.GetString(data);
                Logger.LogTrace($"[Socket Server] ProcessReceive -> Received message from {client.RemoteAddress}.");
                ////Console.WriteLine($"[Socket Server] ProcessReceive -> Received message from {info}");
                ////Console.WriteLine($"[Socket Server] ProcessReceive -> Received message from {client.RemoteAddress}. {info}");
                ////s.Send(Encoding.UTF8.GetBytes("Server return >> " + info));
                client.Send(Encoding.UTF8.GetBytes("Server acked sent message"));
            }
            catch (Exception ex)
            {
                this.Client_OnClose(client);
                Logger.LogError($"[Socket Server] Client_OnReceiveBytes -> Fail to receive message from client.", ex);
                Logger.LogTrace($"[Socket Server] Client_OnReceiveBytes -> Fail to receive message from client.", ex);
            }
        }

        /// <summary>
        /// 从客户端开始接受一个连接操作.
        /// </summary>
        private void StartAccept(SocketAsyncEventArgs asyniar)
        {
            try
            {
                // loop while the method completes synchronously
                bool willRaiseEvent = false;
                while (!willRaiseEvent)
                {
                    this.numberAcceptedClients.WaitOne();

                    // socket must be cleared since the context object is being reused
                    asyniar.AcceptSocket = null;
                    willRaiseEvent = this.listenSocket.AcceptAsync(asyniar);
                    if (!willRaiseEvent)
                    {
                        this.ProcessAccept(asyniar);
                        //// Accept the next connection request
                        this.StartAccept(asyniar);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Socket Server] StartAccept -> Fail to start accept socket from client.", ex);
                Logger.LogTrace($"[Socket Server] StartAccept -> Fail to start accept socket from client.", ex);
            }

            ////if (asyniar == null)
            ////{
            ////    asyniar = new SocketAsyncEventArgs();
            ////    asyniar.Completed += new EventHandler<SocketAsyncEventArgs>(this.OnAcceptCompleted);
            ////}
            ////else
            ////{
            ////    ////socket must be cleared since the context object is being reused
            ////    asyniar.AcceptSocket = null;
            ////}

            ////this.numberAcceptedClients.WaitOne();
            ////if (!this.listenSocket.AcceptAsync(asyniar))
            ////{
            ////    this.ProcessAccept(asyniar);
            ////    ////如果I/O挂起等待异步则触发AcceptAsyn_Asyn_Completed事件
            ////    ////此时I/O操作同步完成，不会触发Asyn_Completed事件，所以指定BeginAccept()方法
            ////}
        }

        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                lock (this.acceptLocker)
                {
                    this.ProcessAccept(e);

                    // Accept the next connection request
                    this.StartAccept(e);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Socket Server] OnAcceptCompleted -> Fail to accept client.", ex);
                Logger.LogTrace($"[Socket Server] OnAcceptCompleted -> Fail to accept client.", ex);
            }
        }

        /// <summary>
        /// 监听Socket接受处理.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Socket s = e.AcceptSocket; ////和客户端关联的socket.
                if (s != null && s.Connected)
                {
                    var client = this.clientSessionPool.Pop();
                    try
                    {
                        client.Socket = s;
                        ////AsyncUserToken token = (AsyncUserToken)asyniar.UserToken;

                        ////用户的token操作
                        ////token.Socket = s;
                        ////token.ID = System.Guid.NewGuid().ToString();
                        ////token.ConnectDateTime = DateTime.Now;

                        this.socketUserTokenList.Add(client); ////添加到正在连接列表

                        ////s.Send(Encoding.UTF8.GetBytes("Your GUID:" + token.ID));

                        Logger.LogTrace($"[Socket Server] ProcessAccept -> Start to get message from socket at {s.RemoteEndPoint.ToString()}.");
                        ////if (!s.ReceiveAsync(asyniar)) ////投递接收请求
                        ////{
                        ////    this.ProcessReceive(asyniar);
                        ////}
                        client.ReceiveAsync();
                        Interlocked.Increment(ref this.numConnectedSockets); ////原子操作加1.
                        Console.WriteLine($" Current connection number is : {this.numConnectedSockets}");
                        Logger.LogTrace($"[Socket Server] ProcessAccept -> Accepted socket from {s.RemoteEndPoint.ToString()}.");
                    }
                    catch (SocketException ex)
                    {
                        s?.Close();
                        this.Client_OnClose(client);
                        Logger.LogError($"[Socket Server] ProcessAccept -> Fail to accept client.", ex);
                        Logger.LogTrace($"[Socket Server] ProcessAccept -> Fail to sccept client.", ex);
                    }

                    ////投递下一个接受请求
                    ////this.StartAccept(e);
                }
            }
            ////else
            ////{
            ////    this.CloseClientSocket(e);
            ////}
        }

        ////private void CloseClientSocket(Socket s, SocketAsyncEventArgs e)
        ////{
        ////    try
        ////    {
        ////        s.Shutdown(SocketShutdown.Both);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Logger.LogError($"[Socket Server] CloseClientSocket -> Fail to close socket.", ex);
        ////        Logger.LogTrace($"[Socket Server] CloseClientSocket -> Fail to close socket.", ex);
        ////    }
        ////    finally
        ////    {
        ////        s.Close();
        ////        s = null;
        ////    }

        ////    Interlocked.Decrement(ref this.numConnectedSockets);
        ////    this.numberAcceptedClients.Release(); ////释放线程信号量
        ////    this.clientSessionPool.Push(e); ////SocketAsyncEventArg 对象被释放，压入可重用队列。
        ////    this.socketUserTokenList.Remove(e); ////去除正在连接的用户
        ////    Console.WriteLine($"Connection number is decreasing, Current connection number is : {this.numConnectedSockets}");
        ////    Logger.LogTrace($"[Socket Server] CloseClientSocket -> Closed socket from client.");
        ////}
    }
}
