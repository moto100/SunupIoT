// <copyright file="RequestFunction.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    /// <summary>
    /// Request function type.
    /// </summary>
    public enum RequestFunction : byte
    {
        /// <summary>
        /// UnKnown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Subscribe
        /// </summary>
        Subscribe = 1,

        /// <summary>
        /// Unsubscribe
        /// </summary>
        Unsubscribe = 2,

        /// <summary>
        /// GetSubscribedData
        /// </summary>
        GetSubscribedData = 3,

        /// <summary>
        /// GetSubscribedData
        /// </summary>
        Write = 4,
    }
}
