// <copyright file="ConnectionType.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    /// <summary>
    /// Result status type.
    /// </summary>
    public enum ConnectionType : byte
    {
        /// <summary>
        /// UnKnown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Opened
        /// </summary>
        Opened = 1,

        /// <summary>
        /// InvalidJson
        /// </summary>
        Closed = 2,
    }
}
