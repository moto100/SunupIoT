// <copyright file="CSScriptEngine.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptExecutor
{
    using System;
    using System.Collections.Generic;
    using Sunup.Contract;
    ////using CSScriptLib;

    /// <summary>
    /// CS-ScriptEngine engine.
    /// </summary>
    public class CSScriptEngine : IScriptEngine
    {
        ////private dynamic scriptMethod;
        ////private object value;
        private IList<IReference> references;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSScriptEngine"/> class.
        /// </summary>
        public CSScriptEngine()
        {
            ////this.scriptMethod = null;
        }

        /// <summary>
        /// Add references to Jint engine.
        /// </summary>
        /// <param name="references">References.</param>
        public void AddReferences(IList<IReference> references)
        {
            if (references == null || references.Count == 0)
            {
                return;
            }

            this.references = references;
        }

        /// <summary>
        /// Execute script.
        /// </summary>
        /// <param name="script">Script content.</param>
        public void Execute(string script)
        {
            ////if (this.scriptMethod == null)
            ////{
            ////    this.scriptMethod = CSScript.Evaluator.CreateDelegate<dynamic>(
            ////          @"
            ////          dynamic GetCalculatedValue(dynamic Root)
            ////          {
            ////            return Root.Value +10;
            ////          }");
            ////}

            ////this.value = this.scriptMethod(this.references[0]);
        }

        /// <summary>
        /// Get completion value from execution of script.
        /// </summary>
        /// <returns>Value.</returns>
        public object GetValue()
        {
            return null;
            ////return this.value;
        }

        /// <summary>
        /// Execute script.
        /// </summary>
        /// <param name="script">Script content.</param>
        /// <returns>List of references' name.</returns>
        string[] IScriptEngine.ExtractVariables(string script)
        {
            throw new NotImplementedException();
        }
    }
}
