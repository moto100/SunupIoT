// <copyright file="ConditionScript.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptLibrary.Scripts
{
    using System.Timers;
    using Sunup.Contract;

    /// <summary>
    /// Condition script will run when the data change of expression happen and condition be satisfied.
    /// </summary>
    public class ConditionScript : DataChangeScript
    {
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionScript"/> class.
        /// </summary>
        /// <param name="conditionScriptType">Condition script type. </param>
        /// <param name="valueExpression">Value expression.</param>
        /// <param name="scriptContent">Script content.</param>
        public ConditionScript(ConditionScriptType conditionScriptType, Expression valueExpression, string scriptContent)
            : base(valueExpression, scriptContent)
        {
            this.ScriptType = ScriptType.Condition;
            this.ConditionScriptType = conditionScriptType;
            this.Interval = 1000;
        }

        /// <summary>
        /// Gets or sets condition script type.
        /// </summary>
        public ConditionScriptType ConditionScriptType { get; set; }

        /// <summary>
        /// Gets or sets interval.
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// On data change.
        /// </summary>
        public override void OnDataChange()
        {
            var dataValue = this.ValueExpression.DataValue.Value;
            if (this.ConditionScriptType == ConditionScriptType.OnTrue)
            {
                bool value = false;
                bool.TryParse(dataValue.ToString(), out value);
                if (value)
                {
                    this.Run();
                }
            }
            else if (this.ConditionScriptType == ConditionScriptType.OnFalse)
            {
                bool value = false;
                bool.TryParse(dataValue.ToString(), out value);
                if (!value)
                {
                    this.Run();
                }
            }
            else if (this.ConditionScriptType == ConditionScriptType.WhileTrue)
            {
                bool value = false;
                bool.TryParse(dataValue.ToString(), out value);
                if (value)
                {
                    this.StartTimer();
                }
                else
                    {
                    this.StopTimer();
                }
            }
            else if (this.ConditionScriptType == ConditionScriptType.WhileFalse)
            {
                bool value = false;
                bool.TryParse(dataValue.ToString(), out value);
                if (!value)
                {
                    this.StartTimer();
                }
                else
                {
                    this.StopTimer();
                }
            }
        }

        private void StartTimer()
        {
            if (this.timer == null)
            {
                var interval = this.Interval < 1000 ? 1000 : this.Interval;
                this.timer = new Timer(interval);
                this.timer.Elapsed += (sender, e) =>
                {
                    this.Run();
                };

                this.timer.Start();
            }
        }

        private void StopTimer()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Dispose();
                this.timer = null;
            }
        }
    }
}
