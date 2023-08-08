// <copyright file="Script.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptLibrary
{
    using System.Collections.Generic;
    using Sunup.Contract;
    using Sunup.ScriptExecutor;

    /// <summary>
    /// Script.
    /// </summary>
    public class Script
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        /// <param name="scriptContent">script content.</param>
        public Script(string scriptContent)
        {
            this.ScriptContent = scriptContent;
            this.ReferenceIndexes = new List<int>();
            this.ReferenceTypes = new List<int>();
            this.ReferenceList = new List<IReference>();
        }

        private Script()
        {
        }

        /// <summary>
        /// Gets or sets script content.
        /// </summary>
        public string ScriptContent { get; set; }

        /// <summary>
        /// Gets or sets reference list.
        /// </summary>
        public List<IReference> ReferenceList { get; set; }

        /// <summary>
        /// Gets or sets reference indexes.
        /// </summary>
        public List<int> ReferenceIndexes { get; set; }

        /// <summary>
        /// Gets or sets referencet types.
        /// </summary>
        public List<int> ReferenceTypes { get; set; }

        /// <summary>
        /// Gets or sets script type.
        /// </summary>
        public virtual ScriptType ScriptType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets reference IsInitialized.
        /// </summary>
        protected bool IsInitialized { get; set; }

        /// <summary>
        /// Gets or sets script executor.
        /// </summary>
        protected ScriptExecutor ScriptExecutor { get; set; }

        /// <summary>
        /// Add reference.
        /// </summary>
        /// <param name="referenceIndex">Index of reference.</param>
        /// <param name="referenceType">Type of referenec.</param>
        public void AddReference(int referenceIndex, int referenceType)
        {
            this.ReferenceIndexes.Add(referenceIndex);
            this.ReferenceTypes.Add(referenceType);
        }

        /// <summary>
        /// Execute script.
        /// </summary>
        public virtual void Run()
        {
            if (this.ScriptExecutor == null && !string.IsNullOrEmpty(this.ScriptContent))
            {
                this.ScriptExecutor = new ScriptExecutor(this.ScriptContent, this.ReferenceList);
            }

            if (this.ScriptExecutor != null)
            {
                this.ScriptExecutor.Execute();
            }
        }

        /// <summary>
        /// GetValue.
        /// </summary>
        /// <returns>value.</returns>
        public virtual object GetValue()
        {
            if (this.ScriptExecutor != null)
            {
               return this.ScriptExecutor.GetValue();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public virtual void Stop()
        {
        }
    }
}