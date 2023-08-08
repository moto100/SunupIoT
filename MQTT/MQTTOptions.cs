// <copyright file="MQTTOptions.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.MQTT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// MQTTOptions.
    /// </summary>
    public class MQTTOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MQTTOptions"/> class.
        /// </summary>
        public MQTTOptions()
        {
            this.Mode = "Client";
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
        /// Gets or sets Quality.
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Retained.
        /// </summary>
        public bool Retained { get; set; }
    }
}
