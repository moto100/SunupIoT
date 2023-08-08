// <copyright file="SocketServerClientHandler.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.NetSocket
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net.Sockets;
    using Org.BouncyCastle.Ocsp;
    using Sunup.Diagnostics;

    /// <summary>
    /// SocketServerClientHandler.
    /// </summary>
    public class SocketServerClientHandler
    {
        private Socket clientSocket = null;

        private string remoteIP;
        //// /private int recvsize = 0;

        private int dataSize = 10;
        private byte[] byteBuf = new byte[10];
        private List<byte> recvBufs = new List<byte>();

        private AsyncCallback doBeginRcvData = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServerClientHandler"/> class.
        /// </summary>
        /// <param name="socket">socket.</param>
        public SocketServerClientHandler(Socket socket)
        {
            this.ClientId = Guid.NewGuid().ToString();
            this.doBeginRcvData = new AsyncCallback(this.DoBeginRcvData);

            this.clientSocket = socket;
            try
            {
                this.remoteIP = socket.RemoteEndPoint.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError("[Socket Server] SocketServerClientHandler >>Get remote end point err. ", ex);
                Logger.LogTrace("[Socket Server] SocketServerClientHandler >>Get remote end point err. ", ex);
            }
        }

        /// <summary>
        /// Disconnected.
        /// </summary>
        /// <param name="c">c.</param>
        public delegate void Disconnected(SocketServerClientHandler c);

        /// <summary>
        /// Disconnected.
        /// </summary>
        /// <param name="c">c.</param>
        public delegate void ReceiveData(SocketServerClientHandler c);

        /// <summary>
        /// OnDisconnected
        /// </summary>
        public event Disconnected OnDisconnected;

        /// <summary>
        /// OnDisconnected
        /// </summary>
        public event ReceiveData OnReceiveData;

        /// <summary>
        /// Gets ClientSocket.
        /// </summary>
        public Socket ClientSocket
        {
            get { return this.clientSocket; }
        }

        /// <summary>
        /// Gets ClientSocket.
        /// </summary>
        public string ClientId
        {
            get;  private set;
        }

        /// <summary>
        /// Gets remoteIP.
        /// </summary>
        public string RemoteIP
        {
            get { return this.remoteIP; }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            this.clientSocket?.Dispose();
            this.clientSocket = null;
        }

        /// <summary>
        /// BeginReceive.
        /// </summary>
        /// <returns>true.</returns>
        public bool BeginReceive()
        {
            try
            {
                this.clientSocket.ReceiveAsync(new SocketAsyncEventArgs());
                this.clientSocket.BeginReceive(this.byteBuf, 0, this.byteBuf.Length, SocketFlags.None, this.doBeginRcvData, null);
            }
            catch (Exception ex)
            {
                Logger.LogError("[Socket Server] SocketServerClientHandler >>BeginReceive error. ", ex);
                Logger.LogTrace("[Socket Server] SocketServerClientHandler >>BeginReceive error. ", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// SendAsync.
        /// </summary>
        /// <param name="bytsCmd">bytsCmd.</param>
        /// <returns>true.</returns>
        public bool SendAsync(byte[] bytsCmd)
        {
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            ////lock (socketAsyncEventlist)
            ////{
            ////    if (socketAsyncEventlist.Count > 0)
            ////    {
            ////        e = socketAsyncEventlist[socketAsyncEventlist.Count - 1];
            ////        socketAsyncEventlist.RemoveAt(socketAsyncEventlist.Count - 1);
            ////    }
            ////}

            ////if (e == null)
            ////{
            ////    e = new SocketAsyncEventArgs();
            ////    e.Completed += (object sender, SocketAsyncEventArgs ent) =>
            ////    {
            ////        lock (socketAsyncEventlist)
            ////        {
            ////            socketAsyncEventlist.Add(e);
            ////        }
            ////    };
            ////}

            try
            {
                e.SetBuffer(bytsCmd, 0, bytsCmd.Length);
            }
            catch (Exception ex)
            {
                ////lock (socketAsyncEventlist)
                ////{
                ////    socketAsyncEventlist.Add(e);
                ////}

                Logger.LogError("[Socket Server] SocketServerClientHandler >> SetBuffer error. ", ex);
                Logger.LogTrace("[Socket Server] SocketServerClientHandler >> SetBuffer error. ", ex);
                return false;
            }

            try
            {
                if (this.clientSocket.SendAsync(e))
                {
                    ////Returns true if the I/O operation is pending.
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[Socket Server] SocketServerClientHandler >> SendAsync error. ", ex);
                Logger.LogTrace("[Socket Server] SocketServerClientHandler >> SendAsync error. ", ex);

                return false;
            }

            ////Returns false if the I/O operation completed synchronously.
            ////In this case, The SocketAsyncEventArgs.Completed event on the e parameter will not be raised and
            ////the e object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.
            ////lock (socketAsyncEventlist)
            ////{
            ////    socketAsyncEventlist.Add(e);
            ////}

            return true;
        }

        private void RaiseDisconnectedEvent()
        {
            Disconnected handler = this.OnDisconnected;
            if (handler != null)
            {
                try
                {
                    this.OnDisconnected(this);
                }
                catch (Exception ex)
                {
                    Logger.LogError("[Socket Server] SocketServerClientHandler >> RaiseDisconnectedEvent error. ", ex);
                    Logger.LogTrace("[Socket Server] SocketServerClientHandler >> RaiseDisconnectedEvent error. ", ex);
                }
            }
        }

        private void DoBeginRcvData(IAsyncResult ar)
        {
            int ret = 0;
            try
            {
                ret = this.clientSocket.EndReceive(ar);
            }
            catch (Exception ex)
            {
                Logger.LogError("[Socket Server] SocketServerClientHandler >> DoBeginRcvData error. ", ex);
                Logger.LogTrace("[Socket Server] SocketServerClientHandler >> DoBeginRcvData error. ", ex);

                this.Stop();
                this.RaiseDisconnectedEvent();

                return;
            }

            if (ret > 0)
            {
                ////this.recvsize += ret;
                ////if (this.byteBuf[this.recvsize - 1] == 0)
                ////{
                ////    return;
                ////}

                ////if (this.recvsize >= this.dataSize)
                ////{
                ////    byte[] buff2 = new byte[this.dataSize + 1024];
                ////    this.byteBuf.CopyTo(buff2, 0);
                ////    this.byteBuf = buff2;
                ////    this.dataSize += 1024;
                ////}

                this.recvBufs.AddRange(this.byteBuf);
                if (ret < this.dataSize || this.byteBuf[this.dataSize - 1] == 0)
                {
                    ////收到后发送回一个数据包
                    this.SendAsync(this.recvBufs.ToArray());
                    this.recvBufs = new List<byte>();
                }

                this.byteBuf = new byte[this.dataSize];

                if (this.BeginReceive() == false)
                {
                    this.Stop();
                    this.RaiseDisconnectedEvent();
                }
            }
            else
            {
                if (this.OnReceiveData != null)
                {
                    this.OnReceiveData(this);
                }

                this.Stop();
                this.RaiseDisconnectedEvent();
            }
        }
    }
}
