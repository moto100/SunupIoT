// <copyright file="Expression.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptLibrary
{
    using System.Collections.Generic;
    using Sunup.Contract;
    using Sunup.ScriptExecutor;

    /// <summary>
    /// Expression.
    /// </summary>
    public class Expression : IDataChange
    {
        private object lastValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Expression"/> class.
        /// </summary>
        /// <param name="scriptContent">Script content.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="expressionType">ExpressionType.Reference.</param>
        public Expression(string scriptContent, DataValue defaultValue, ExpressionType expressionType = ExpressionType.Reference)
        {
            this.ScriptContent = scriptContent;
            this.DataValue = defaultValue;
            this.lastValue = null;
            this.ExpressionType = expressionType;
            this.ReferenceList = new List<IReference>();
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
        /// Gets or sets a value indicating whether ExpressionType.
        /// </summary>
        public ExpressionType ExpressionType { get; set; }

        /// <summary>
        /// Gets or sets instance of IDataChange interface.
        /// </summary>
        public IDataChange Notification { get; set; }

        /// <summary>
        /// Gets or sets DataValue.
        /// </summary>
        public DataValue DataValue { get; set; }

        /// <summary>
        /// Gets or sets script executor.
        /// </summary>
        protected ScriptExecutor ScriptExecutor { get; set; }

        /// <summary>
        /// Exexute expression.
        /// </summary>
        public void Run()
        {
            ////if (!this.IsInitialized)
            ////{
            ////    this.InternalRun(null);
            ////    this.IsInitialized = true;
            ////}
            if (this.ScriptExecutor == null && !string.IsNullOrEmpty(this.ScriptContent))
            {
                this.ScriptExecutor = new ScriptExecutor(this.ScriptContent, this.ReferenceList);
            }

            if (this.ScriptExecutor != null)
            {
                this.ScriptExecutor.Evaluate();
            }

            this.InternalRun();
        }

        /// <summary>
        /// On data change.
        /// </summary>
        public void OnDataChange()
        {
            ////this.IsInitialized = true;
            this.InternalRun();
        }

        private void InternalRun()
        {
            if (this.ScriptExecutor == null)
            {
                return;
            }

            var value = this.GetValue();
            if (value != null)
            {
                this.DataValue = this.DataValue.Clone(value);
                if (this.Notification != null && !value.Equals(this.lastValue))
                {
                    this.lastValue = value;
                    this.Notification.OnDataChange();
                }
            }
        }

        /// <summary>
        /// GetValue.
        /// </summary>
        /// <returns>value.</returns>
        private object GetValue()
        {
            if (this.ScriptExecutor != null)
            {
                this.ScriptExecutor.Evaluate();
                return this.ScriptExecutor.GetValue();
            }
            else
            {
                return null;
            }
        }
    }
}
