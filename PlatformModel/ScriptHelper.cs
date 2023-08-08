// <copyright file="ScriptHelper.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>
namespace Sunup.PlatformModel
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Sunup.ScriptExecutor;

    /// <summary>
    /// Script executor.
    /// </summary>
    public class ScriptHelper
    {
        private static readonly Regex VariableRegex = new Regex(@"(Root(\s*\.\s*.+)+)\.\s*Value", RegexOptions.IgnoreCase);
        private string scriptContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptHelper"/> class.
        /// </summary>
        /// <param name="scriptContent">Script content.</param>
        public ScriptHelper(string scriptContent)
        {
            this.scriptContent = scriptContent;
        }

        /// <summary>
        /// Execute a script.
        /// </summary>
        /// <returns>return variable list.</returns>
        public List<string> ExtractVariables()
        {
            List<string> variables = new List<string>();
            if (string.IsNullOrEmpty(this.scriptContent))
            {
                return variables;
            }

            var matchs = VariableRegex.Matches(this.scriptContent);
            var count = matchs.Count;
            if (count > 0)
            {
                variables = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    var fullVariableName = matchs[i].Groups[0].Value; ////Root.业务模型.设备模型1.设备模型2.Value
                    var variableName = matchs[i].Groups[1].Value; ////Root.业务模型.设备模型1.设备模型2
                    if (!variables.Contains(variableName))
                    {
                        variables.Add(variableName);
                    }
                }
            }

            return variables;
        }

        /// <summary>
        /// Transform script,replace "." with "__" in reference name, and find out references for this scirpt.
        /// </summary>
        /// <param name="references">references.</param>
        /// <param name="transformedScript">transformedScript.</param>
        /// <returns>return references for this script.</returns>
        public List<Node> TransformScript(Dictionary<string, Node> references, out string transformedScript)
        {
            transformedScript = null;
            var originalScript = this.scriptContent;
            var referenceList = new List<Node>();
            var variableNames = this.ExtractVariables();
            if (variableNames != null && variableNames.Count > 0)
            {
                foreach (string variableName in variableNames)
                {
                    var transformedName = variableName.Replace(" ", string.Empty).Replace(".", "__");
                    if (references.ContainsKey(transformedName))
                    {
                        var node = references[transformedName];
                        referenceList.Add(node);
                        originalScript = originalScript.Replace(variableName + ".Value", transformedName + ".Value");
                    }
                }
            }

            transformedScript = originalScript;
            return referenceList;
        }
    }
}