// <copyright file="OnShowScript.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>
namespace Sunup.ScriptLibrary.Scripts
{
    /// <summary>
    /// OnShowScript just run one time when open a symbol.
    /// </summary>
    public class OnShowScript : Script
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnShowScript"/> class.
        /// OnShowScript.
        /// </summary>
        /// <param name="scriptContent">Script content.</param>
        public OnShowScript(string scriptContent)
            : base(scriptContent)
        {
        }

        /// <inheritdoc/>
        public override ScriptType ScriptType
        {
            get { return ScriptType.OnShow; }
        }
    }
}
