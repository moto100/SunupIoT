// <copyright file="ReferenceType.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptExecutor
{
    /// <summary>
    /// Reference Type.
    /// </summary>
    public enum ReferenceType : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// SymbolAttribute
        /// </summary>
        SymbolAttribute = 1,

        /// <summary>
        /// ExternalReference
        /// </summary>
        ExternalReference = 2,

        /// <summary>
        /// PlatformModelNode
        /// </summary>
        PlatformModelNode = 3,

        /// <summary>
        /// Global Variable
        /// </summary>
        GlobalVariable = 4,
    }
}
