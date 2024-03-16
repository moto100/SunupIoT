// <copyright file="IScriptEngine.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptExecutor
{
    using System.Collections.Generic;
    using Sunup.Contract;

    /// <summary>
    /// Interface of script engine.
    /// </summary>
    public interface IScriptEngine
    {
        /// <summary>
        /// Execute script.
        /// </summary>
        /// <param name="script">Script content.</param>
        /// <param name="force">force to run script.</param>
        void Execute(string script, bool force = false);

        /// <summary>
        /// Evaluate script.
        /// </summary>
        /// <param name="script">Script content.</param>
        /// <param name="force">force to run script.</param>
        void Evaluate(string script, bool force = false);

        /// <summary>
        /// Add references.
        /// </summary>
        /// <param name="references">References.</param>
        void AddReferences(IList<IReference> references);

        /// <summary>
        /// Return completion value.
        /// </summary>
        /// <returns>Result of final statement.</returns>
        object GetValue();

        /// <summary>
        /// Extract variables.
        /// </summary>
        /// <param name="script">Script content.</param>
        /// <returns>List of reference names.</returns>
        string[] ExtractVariables(string script);
    }
}
