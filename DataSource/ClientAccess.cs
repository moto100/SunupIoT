// <copyright file="ClientAccess.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource
{
    using Sunup.Contract;

    /// <summary>
    /// DeviceAccess.
    /// </summary>
    public class ClientAccess
    {
        /// <summary>
        /// Gets or sets DeviceId.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets Identify.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Enabled.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
