// <copyright file="DataChangeScript.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptLibrary.Scripts
{
    using Sunup.Contract;

    /// <summary>
    /// DataChangeScript will run when data change of expression happen.
    /// </summary>
    public class DataChangeScript : Script, IDataChange
    {
        ////private DataValue lastDataValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataChangeScript"/> class.
        /// DataChangeScript.
        /// </summary>
        /// <param name="valueExpression">valueExpression.</param>
        /// <param name="scriptContent">scriptContent.</param>
        public DataChangeScript(Expression valueExpression, string scriptContent)
            : base(scriptContent)
        {
            this.ScriptType = ScriptType.DataChange;
            this.ValueExpression = valueExpression;
            this.ValueExpressionIndex = -1;
        }

        /// <summary>
        /// Gets or sets ValueExpressionIndex.
        /// </summary>
        public int ValueExpressionIndex { get; set; }

        /// <summary>
        /// Gets or sets valueExpression.
        /// </summary>
        public Expression ValueExpression { get; set; }

        /// <summary>
        /// Execute script.
        /// </summary>
        public override void Run()
        {
            if (this.ValueExpression != null)
            {
                this.ValueExpression.Notification = this;
                this.ValueExpression.Run();
            }
        }

        /// <summary>
        /// Implementation of IDataChange.
        /// </summary>
        public virtual void OnDataChange()
        {
            ////if (this.lastDataValue == null)
            ////{
            ////    this.lastDataValue = this.ValueExpression.DataValue.Clone();
            ////    base.Run();
            ////}
            ////else if (!this.lastDataValue.Value.Equals(this.ValueExpression.DataValue.Value))
            ////{
            ////    this.lastDataValue.Value = this.ValueExpression.DataValue.Value;
            ////    base.Run();
            ////}
        }
    }
}
