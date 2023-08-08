// <copyright file="SocketOptions.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.NetSocket
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// SocketOptions.
    /// </summary>
    public class SocketOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SocketOptions"/> class.
        /// </summary>
        public SocketOptions()
        {
            this.Mode = "Server";
        }

        /// <summary>
        /// Gets or sets mode.
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Gets or sets Server.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets Port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets User.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets NumConnections.
        /// </summary>
        public int MaxNumConnections { get; set; }

        /// <summary>
        /// Gets or sets ReceiveBufferSize.
        /// </summary>
        public int ReceiveBufferSize { get; set; }
    }
}
