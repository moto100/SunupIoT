// <copyright file="ExpressionType.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>
namespace Sunup.ScriptLibrary
{
    /// <summary>
    /// Expression type.
    /// </summary>
    public enum ExpressionType : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Static
        /// </summary>
        Static = 1,

        /// <summary>
        /// Reference
        /// </summary>
        Reference = 2,

        /// <summary>
        /// Compound
        /// </summary>
        Compound = 3,
    }
}
