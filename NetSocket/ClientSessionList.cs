// <copyright file="ClientSessionList.cs" company="Sunup">
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
    /// SocketOptions.
    /// </summary>
    public class ClientSessionList
    {
        private List<ClientSession> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSessionList"/> class.
        /// </summary>
        public ClientSessionList() => this.list = new List<ClientSession>();

        /// <summary>
        /// Add.
        /// </summary>
        /// <param name="s">ClientSession.</param>
        public void Add(ClientSession s)
        {
            lock (this.list)
            {
                this.list.Add(s);
            }
        }

        /// <summary>
        /// Remove ClientSession.
        /// </summary>
        /// <param name="s">ClientSession.</param>
        public void Remove(ClientSession s)
        {
            lock (this.list)
            {
                this.list.Remove(s);
            }
        }

        /// <summary>
        /// CopyList.
        /// </summary>
        /// <param name="array">ClientSession.</param>
        public void CopyList(ref ClientSession[] array)
        {
            lock (this.list)
            {
                array = new ClientSession[this.list.Count];
                this.list.CopyTo(array);
            }
        }
    }
}
