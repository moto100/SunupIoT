// <copyright file="NodeReference.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.PlatformModel
{
    using Sunup.ScriptExecutor;

    /// <summary>
    /// NodeReference.
    /// </summary>
    public class NodeReference : VariableReference
    {
        private Node businessNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeReference"/> class.
        /// </summary>
        /// <param name="businessNode">businessNode.</param>
        /// <param name="referenceName">referenceName.</param>
        public NodeReference(Node businessNode, string referenceName)
        {
            this.businessNode = businessNode;
            this.ReferenceName = referenceName;
        }

        /// <summary>
        /// Gets or sets value of attibute.
        /// </summary>
        public override object Value
        {
            get
            {
                return this.businessNode.DataValue.Value;
            }

            set
            {
                this.businessNode.SetValue(value);
            }
        }
    }
}
