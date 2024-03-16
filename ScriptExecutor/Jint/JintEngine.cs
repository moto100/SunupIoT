// <copyright file="JintEngine.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Esprima.Ast;
    using Jint;
    using Sunup.Contract;

    /// <summary>
    /// Jint script engine.
    /// </summary>
    public class JintEngine : IScriptEngine
    {
        private Engine engine;
        private Program program;
        private Script script;
        private object completionValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="JintEngine"/> class.
        /// </summary>
        public JintEngine()
        {
            this.engine = new Engine(cfg => cfg.AllowClr());
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

            for (int i = 0; i < references.Count; i++)
            {
                if (references[i].ReferenceNames != null)
                {
                    foreach (var name in references[i].ReferenceNames)
                    {
                        this.engine.SetValue(name, references[i]);
                    }
                }
                else
                {
                    this.engine.SetValue(references[i].ReferenceName, references[i]);
                }
            }
        }

        /// <summary>
        /// Execute script.
        /// </summary>
        /// <param name="script">Script content.</param>
        /// <param name="force">force to run script.</param>
        public void Execute(string script, bool force = false)
        {
            if (string.IsNullOrEmpty(script))
            {
                return;
            }

            if (this.script == null || force)
            {
                this.script = Engine.PrepareScript(script);
            }

            this.engine.Execute(this.script);
        }

        /// <summary>
        /// Evaluate script.
        /// </summary>
        /// <param name="script">Script content.</param>
        /// <param name="force">force to run script.</param>
        public void Evaluate(string script, bool force = false)
        {
            if (string.IsNullOrEmpty(script))
            {
                return;
            }

            if (this.script == null || force)
            {
                this.script = Engine.PrepareScript(script);
            }

            var value = this.engine.Evaluate(this.script);
            this.completionValue = value.ToObject();
        }

        /// <summary>
        /// Get completion value from execution of script.
        /// </summary>
        /// <returns>Value.</returns>
        public object GetValue()
        {
            return this.completionValue;
        }

        /// <summary>
        /// Execute script.
        /// </summary>
        /// <param name="script">Script content.</param>
        /// <returns>List of references' name.</returns>
        string[] IScriptEngine.ExtractVariables(string script)
        {
            List<string> variables = new List<string>();
            var parser = new JintScriptParser();
            if (this.program == null)
            {
                this.program = parser.Parse(script);
            }

            if (this.program != null)
            {
                //// parser.ExtractVaribale(this.program);
                if (parser.Variables != null && parser.Variables.Count > 0)
                {
                    parser.Variables.ForEach(x =>
                    {
                        //// variable name will be like AZA1.ERD1.WSD3
                        variables.Add(string.Join(".", x.ToArray()));
                    });
                }
            }

            return variables.ToArray();
        }
    }
}
