// <copyright file="WriteItem.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    /// <summary>
    /// WriteItem.
    /// </summary>
    public class WriteItem
    {
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets object.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets BoundDevice.
        /// </summary>
        public string BoundDevice { get; set; }

        /// <summary>
        /// Gets or sets BoundField.
        /// </summary>
        public string BoundField { get; set; }
    }
}
