// <copyright file="DataType.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    /// <summary>
    /// Data Type.
    /// </summary>
    public enum DataType : byte
    {
        /// <summary>
        /// unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Number.
        /// </summary>
        Integer = 1,

        /// <summary>
        /// string.
        /// </summary>
        String = 2,

        /// <summary>
        /// bool.
        /// </summary>
        Bool = 3,

        /// <summary>
        /// DateTime.
        /// </summary>
        DateTime = 4,

        /// <summary>
        /// Float.
        /// </summary>
        Float = 5,

        /// <summary>
        /// Double.
        /// </summary>
        Double = 6,
    }
}
