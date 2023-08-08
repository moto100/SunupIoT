// <copyright file="DataProcessMode.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    /// <summary>
    /// Data Type.
    /// </summary>
    public enum DataProcessMode : byte
    {
        /// <summary>
        /// unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Simply.
        /// </summary>
        SimplyMaping = 1,

        /// <summary>
        /// ScriptProcess.
        /// </summary>
        ScriptProcess = 2,

        /// <summary>
        /// Complex.
        /// </summary>
        Complex = 3,
    }
}
