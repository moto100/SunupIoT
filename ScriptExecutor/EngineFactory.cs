// <copyright file="EngineFactory.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptExecutor
{
    /// <summary>
    /// Engine factory.
    /// </summary>
    public class EngineFactory
    {
        /// <summary>
        /// Create a default script execution engine.
        /// </summary>
        /// <returns>Return a default engine.</returns>
        public static IScriptEngine CreateEngine()
        {
            return new JintEngine();
        }
    }
}
