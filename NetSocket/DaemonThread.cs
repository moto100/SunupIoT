// <copyright file="DaemonThread.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.NetSocket
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Sunup.Diagnostics;

    /// <summary>
    /// DaemonThread.
    /// </summary>
    public class DaemonThread
    {
        private Thread thread;
        private SocketServer asyncSocketServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DaemonThread"/> class.
        /// </summary>
        /// <param name="asyncSocketServer">asyncSocketServer.</param>
        public DaemonThread(SocketServer asyncSocketServer)
        {
            this.asyncSocketServer = asyncSocketServer;
            this.thread = new Thread(this.DaemonThreadStart);
            this.thread.Start();
        }

        /// <summary>
        /// DaemonThreadStart.
        /// </summary>
        public void DaemonThreadStart()
        {
            while (this.thread.IsAlive)
            {
                ClientSession[] userTokenArray = null;
                this.asyncSocketServer.SocketUserTokenList.CopyList(ref userTokenArray);
                for (int i = 0; i < userTokenArray.Length; i++)
                {
                    if (!this.thread.IsAlive)
                    {
                        break;
                    }

                    try
                    {
                        ////ClientSession userToke = userTokenArray[i];
                        ////if ((DateTime.Now - userToke.ActiveDateTime).Milliseconds > this.asyncSocketServer.SocketTimeOutMS) ////超时Socket断开
                        ////{
                        ////    lock (userTokenArray[i])
                        ////    {
                        ////        this.asyncSocketServer.CloseClientSocket(userTokenArray[i]);
                        ////    }
                        ////}
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"[Socket Server]Daemon thread check timeout socket error.", ex);
                        Logger.LogTrace($"[Socket Server]Daemon thread check timeout socket error.", ex);
                    }
                }

                for (int i = 0; i < 60 * 1000 / 10; i++) ////每分钟检测一次
                {
                    if (!this.thread.IsAlive)
                    {
                        break;
                    }

                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>
        /// Close.
        /// </summary>
        public void Close()
        {
            this.thread.Abort();
            this.thread.Join();
        }
    }
}
