// <copyright file="ScriptExecutor.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>
namespace Sunup.ScriptExecutor
{
    using System;
    using System.Collections.Generic;
    using Sunup.Contract;
    using Sunup.Diagnostics;

    /// <summary>
    /// Script executor.
    /// </summary>
    public class ScriptExecutor
    {
        private IScriptEngine engine;

        private string scriptContent;

        private List<IReference> referenceList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptExecutor"/> class.
        /// </summary>
        /// <param name="scriptContent">Script content.</param>
        /// <param name="referenceList">Reference list.</param>
        public ScriptExecutor(string scriptContent, List<IReference> referenceList)
        {
            this.scriptContent = scriptContent;
            this.referenceList = referenceList;
            this.engine = EngineFactory.CreateEngine();
            this.engine.AddReferences(this.referenceList);
        }

        /// <summary>
        /// Execute a script.
        /// </summary>
        public void Execute()
        {
            try
            {
                this.engine.Execute(this.scriptContent);
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Script Executor]Exception happen on Execution >>Script: {this.scriptContent}.", ex);
            }
        }

        /// <summary>
        /// Get complition value.
        /// </summary>
        /// <returns>Value of result.</returns>
        public object GetValue()
        {
            try
            {
               return this.engine.GetValue();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Script Executor]Exception happen on GetValue >>Expression/Script: {this.scriptContent}.", ex);
            }

            return null;
        }
    }
}