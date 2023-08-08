// <copyright file="DataItem.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource
{
    using Sunup.Contract;

    /// <summary>
    /// DataItem.
    /// </summary>
    public class DataItem
    {
        /// <summary>
        /// Gets or sets Identify.
        /// </summary>
        public string Identify { get; set; }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public DataType ValueType { get; set; }
    }
}
