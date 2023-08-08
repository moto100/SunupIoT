// <copyright file="AsyncUserToken.cs" company="Sunup">
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

    /// <summary>
    /// 这个类是将USERTOKEN进行了再次封装，MSDN对于异步SOCKET的介绍中总会提到：
    /// 若在异步回调中需要查询更多的信息，则应该建立一个小型类来管理回调时传递的OBJECT对象
    /// USERTOKEN其实就是那个传递的参数，ASYNCUSERTOKEN就是对USERTOKEN的封装，建立的小型类.
    /// </summary>
    public class AsyncUserToken
    {
        /// <summary>
        /// Gets or sets socket.
        /// </summary>
        public Socket Socket
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets socket.
        /// </summary>
        public string ID
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets socket.
        /// </summary>
        public DateTime ActiveDateTime
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets socket.
        /// </summary>
        public DateTime ConnectDateTime
        {
            get; set;
        }
    }
}
