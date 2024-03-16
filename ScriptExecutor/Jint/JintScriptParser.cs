// <copyright file="JintScriptParser.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ScriptExecutor
{
    using System;
    using System.Collections.Generic;
    using Esprima;
    using Esprima.Ast;

    /// <summary>
    /// JintScriptParser.
    /// </summary>
    public class JintScriptParser
    {
        private JavaScriptParser parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="JintScriptParser"/> class.
        /// </summary>
        public JintScriptParser()
        {
            this.parser = new JavaScriptParser();
            this.Variables = new List<Stack<string>>();
        }

        /// <summary>
        /// Gets variables.
        /// </summary>
        public List<Stack<string>> Variables { get; }

        /// <summary>
        /// Parse code.
        /// </summary>
        /// <param name="code">code.</param>
        /// <returns>Program tree.</returns>
        public Program Parse(string code)
        {
            return this.parser.ParseScript(code);
        }

        /// <summary>
        /// Extract variables.
        /// </summary>
        /// <param name="program">program.</param>
        public void ExtractVaribale(Program program)
        {
            if (program == null || program.Body.Count == 0)
            {
                return;
            }

            foreach (var item in program.Body)
            {
                if (item is ExpressionStatement)
                {
                    this.ProcessExpressionStatement(item as ExpressionStatement);
                }
                else if (item is IfStatement)
                {
                    this.ProcessIfStatement(item as IfStatement);
                }
                else if (item is FunctionDeclaration)
                {
                    this.ProcessFunctionDeclaration(item as FunctionDeclaration);
                }
                else if (item is VariableDeclaration)
                {
                    this.ProcessVariableDeclaration(item as VariableDeclaration);
                }
            }
        }

        private void ProcessVariableDeclaration(VariableDeclaration item)
        {
            foreach (var itemA in item.Declarations)
            {
                if (itemA.Init is MemberExpression)
                {
                    Stack<string> names = new Stack<string>();
                    this.ProcessMemberExpression(itemA.Init as MemberExpression, names);
                    if (names.Count > 0)
                    {
                        this.Variables.Add(names);
                    }
                }
            }
        }

        private void ProcessFunctionDeclaration(FunctionDeclaration item)
        {
            if (item.Body is BlockStatement)
            {
                this.ProcessBlockStatement(item.Body as BlockStatement);
            }
        }

        private void ProcessBlockStatement(BlockStatement item)
        {
            foreach (var itemA in item.Body)
            {
                if (itemA is ExpressionStatement)
                {
                    this.ProcessExpressionStatement(itemA as ExpressionStatement);
                }
                else if (itemA is IfStatement)
                {
                    this.ProcessIfStatement(itemA as IfStatement);
                }
                else if (itemA is FunctionDeclaration)
                {
                    this.ProcessFunctionDeclaration(itemA as FunctionDeclaration);
                }
                else if (itemA is VariableDeclaration)
                {
                    this.ProcessVariableDeclaration(itemA as VariableDeclaration);
                }
            }
        }

        private void ProcessIfStatement(IfStatement item)
        {
            if (item.Test is BinaryExpression)
            {
                this.ProcessBinaryExpression(item.Test as BinaryExpression);
            }

            if (item.Alternate is BlockStatement)
            {
                this.ProcessBlockStatement(item.Alternate as BlockStatement);
            }

            if (item.Consequent is BlockStatement)
            {
                this.ProcessBlockStatement(item.Consequent as BlockStatement);
            }
        }

        private void ProcessBinaryExpression(BinaryExpression item)
        {
            if (item.Left is MemberExpression)
            {
                Stack<string> names = new Stack<string>();
                this.ProcessMemberExpression(item.Left as MemberExpression, names);
                if (names.Count > 0)
                {
                    this.Variables.Add(names);
                }
            }
        }

        private void ProcessExpressionStatement(ExpressionStatement item)
        {
            if (item.Expression is AssignmentExpression)
            {
                this.ProcessAssignmentExpression(item.Expression as AssignmentExpression);
            }
            else if (item.Expression is BinaryExpression)
            {
                this.ProcessBinaryExpression(item.Expression as BinaryExpression);
            }
        }

        private void ProcessAssignmentExpression(AssignmentExpression item)
        {
            if (item.Left is MemberExpression)
            {
                Stack<string> names = new Stack<string>();
                this.ProcessMemberExpression(item.Left as MemberExpression, names);
                if (names.Count > 0)
                {
                    this.Variables.Add(names);
                }
            }
        }

        private void ProcessMemberExpression(MemberExpression item, Stack<string> names)
        {
            if (item.Property is Identifier)
            {
                var name = this.ProcessIdentifier(item.Property as Identifier);
                names.Push(name);
            }
            else if (item.Property is Literal)
            {
                var name = this.ProcessLiteral(item.Property as Literal);
                names.Push(name);
            }

            if (item.Object is MemberExpression)
            {
               this.ProcessMemberExpression(item.Object as MemberExpression, names);
            }
            else if (item.Object is Identifier)
            {
                var name = this.ProcessIdentifier(item.Object as Identifier);
                names.Push(name);
            }
        }

        private string ProcessLiteral(Literal item)
        {
            return item.Raw;
        }

        private string ProcessIdentifier(Identifier item)
        {
            return item.Name;
        }
    }
}
