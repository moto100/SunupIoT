// <copyright file="OnHideScript.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptLibrary.Scripts
{
    /// <summary>
    /// OnHideScript just run one time when close or unload a symbol.
    /// </summary>
    public class OnHideScript : Script
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnHideScript"/> class.
        /// </summary>
        /// <param name="scriptContent">Script content.</param>
        public OnHideScript(string scriptContent)
            : base(scriptContent)
        {
        }

        /// <inheritdoc/>
        public override ScriptType ScriptType
        {
            get { return ScriptType.OnHide; }
        }
    }
}
