// <copyright file="NodeItem.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.InfluxDBConnection
{
    /// <summary>
    /// NodeItem.
    /// </summary>
    public struct NodeItem
    {
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets object.
        /// </summary>
        public dynamic Value { get; set; }
    }
}
