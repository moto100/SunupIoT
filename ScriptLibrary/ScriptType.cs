// <copyright file="ScriptType.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>
namespace Sunup.ScriptLibrary
{
    /// <summary>
    /// Script type.
    /// </summary>
    public enum ScriptType : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Condition
        /// </summary>
        Condition = 1,

        /// <summary>
        /// DataChange
        /// </summary>
        DataChange = 2,

        /// <summary>
        /// OnShow
        /// </summary>
        OnShow = 97,

        /// <summary>
        /// WhileShowing
        /// </summary>
        WhileShowing = 98,

        /// <summary>
        /// OnHide
        /// </summary>
        OnHide = 99,
    }

    /// <summary>
    /// ConditionScriptType.
    /// </summary>
    public enum ConditionScriptType : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// OnTrue
        /// </summary>
        OnTrue = 1,

        /// <summary>
        /// OnFalse
        /// </summary>
        OnFalse = 2,

        /// <summary>
        /// WhileTrue
        /// </summary>
        WhileTrue = 3,

        /// <summary>
        /// WhileFalse
        /// </summary>
        WhileFalse = 4,
    }
}
