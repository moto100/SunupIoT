// <copyright file="DataSourceType.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource
{
    /// <summary>
    /// DataSource Type.
    /// </summary>
    public enum DataSourceType : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// OPC
        /// </summary>
        OPC = 1,

        /// <summary>
        /// OPCUA
        /// </summary>
        OPCUA = 2,

        /// <summary>
        /// MQTT
        /// </summary>
        MQTT = 3,

        /// <summary>
        /// Socket
        /// </summary>
        Socket = 4,

        /// <summary>
        /// DataSourceSimulator.
        /// </summary>
        DataSourceSimulator = 99,
    }
}
