// <copyright file="ClientSessionPool.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.NetSocket
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;

    /// <summary>
    /// ClientSessionPool.
    /// </summary>
    public class ClientSessionPool
    {
        private Stack<ClientSession> pool;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSessionPool"/> class, Initializes the object pool to the specified size.
        /// </summary>
        /// <param name="capacity">The "capacity" parameter is the maximum number of ClientSession objects the pool can hold.</param>
        public ClientSessionPool(int capacity)
        {
            this.pool = new Stack<ClientSession>(capacity);
        }

        /// <summary>
        /// Gets the number of ClientSession instances in the pool.
        /// </summary>
        public int Count
        {
            get
            {
                return this.pool.Count;
            }
        }

        /// <summary>
        /// Add a SocketAsyncEventArg instance to the pool.
        /// </summary>
        /// <param name="item">The "item" parameter is the ClientSession instance to add to the pool.</param>
        /// <exception cref="ArgumentNullException">fail to add a null object.</exception>
        public void Push(ClientSession item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a ClientSessionPool cannot be null");
            }

            lock (this.pool)
            {
                this.pool.Push(item);
            }
        }

        /// <summary>
        /// Removes a ClientSession instance from the pool and returns the object removed from the pool.
        /// </summary>
        /// <returns>object.</returns>
        public ClientSession Pop()
        {
            lock (this.pool)
            {
                return this.pool.Pop();
            }
        }
    }
}
