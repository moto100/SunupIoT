// <copyright file="SercurityMode.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource
{
    /// <summary>
    /// SercurityMode Type.
    /// </summary>
    public enum SercurityMode : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// None
        /// </summary>
        None = 1,

        /// <summary>
        /// WhiteList
        /// </summary>
        WhiteList = 2,

        /// <summary>
        /// BlackList
        /// </summary>
        BlackList = 3,
    }
}
