// <copyright file="WhileShowingScript.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptLibrary.Scripts
{
    using System.Timers;

    /// <summary>
    /// WhileShowingScript will run cyclically when symbol is opening.
    /// </summary>
    public class WhileShowingScript : Script
    {
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WhileShowingScript"/> class.
        /// </summary>
        /// <param name="scriptContent">Script content.</param>
        /// <param name="interval">interval time.</param>
        public WhileShowingScript(string scriptContent, int interval)
            : base(scriptContent)
        {
            this.Interval = interval;
        }

        /// <summary>
        /// Gets interval.
        /// </summary>
        public int Interval { get; private set; }

        /// <inheritdoc/>
        public override ScriptType ScriptType
        {
            get { return ScriptType.WhileShowing; }
        }

        /// <summary>
        /// Run script cyclically.
        /// </summary>
        public override void Run()
        {
            if (this.timer == null)
            {
                var interval = this.Interval < 1000 ? 1000 : this.Interval;
                this.timer = new Timer(interval);
                this.timer.Elapsed += (sender, e) =>
                {
                    base.Run();
                };

                this.timer.Start();
            }
        }
    }
}
