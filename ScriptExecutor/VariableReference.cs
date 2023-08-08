// <copyright file="VariableReference.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptExecutor
{
    using System;
    using Sunup.Contract;

    /// <summary>
    ///  Variable.
    /// </summary>
    public class VariableReference : IReference
    {
        /// <summary>
        /// Gets or sets value.
        /// </summary>
       public virtual object Value { get; set; }

        /////// <summary>
        /////// Gets or sets attribute name.
        /////// </summary>
        ////string Name { get; set; }

        /// <summary>
        /// Gets or sets reference name.
        /// </summary>
       public string ReferenceName { get; set; }

        /// <summary>
        /// Gets or sets reference names.
        /// </summary>
       public string[] ReferenceNames { get; set; }
    }
}
