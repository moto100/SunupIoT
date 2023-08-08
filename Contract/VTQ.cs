// <copyright file="VTQ.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    using System;

    /// <summary>
    /// VTQ.
    /// </summary>
    public struct VTQ
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets dataType.
        /// </summary>
        public int DataType { get; set; }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets quality.
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        /// Gets or sets timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
