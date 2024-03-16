// <copyright file="Node.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.PlatformModel
{
    using System;
    using System.Collections.Generic;
    using Sunup.Contract;
    using Sunup.Diagnostics;
    using Sunup.ScriptLibrary;

    /// <summary>
    /// ItemNode.
    /// </summary>
    public class Node : IDataChange
    {
        private IList<IDataChange> dataChangeList;
        ////private object lastValue;
        private Expression expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
        {
            ////this.ReferenceType = ReferenceType.PlatformModelNode;
            this.dataChangeList = new List<IDataChange>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="nameWithoutDot">nameWithoutDot.</param>
        /// <param name="defaultValue">Initial data value.</param>
        public Node(string name, string nameWithoutDot, DataValue defaultValue)
            : this()
        {
            this.Name = name.Remove(0, 5); //// remove "Root."
            this.Reference = new NodeReference(this, nameWithoutDot);
            this.DataValue = defaultValue;
            ////this.lastValue = null;
        }

        /////// <summary>
        /////// Gets or sets reference type.
        /////// </summary>
        ////public ReferenceType ReferenceType { get; set; }

        /// <summary>
        /// Gets or sets AppContainer type.
        /// </summary>
        public NodeContainer AppContainer { get; set; }

        /// <summary>
        /// Gets or sets attribute name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets inbound field.
        /// </summary>
        public string InboundField { get; set; }

        /// <summary>
        /// Gets or sets inbound device.
        /// </summary>
        public string InboundDevice { get; set; }

        /// <summary>
        /// Gets or sets outbound field.
        /// </summary>
        public string OutboundField { get; set; }

        /// <summary>
        /// Gets or sets outbound device.
        /// </summary>
        public string OutboundDevice { get; set; }

        /// <summary>
        /// Gets reference.
        /// </summary>
        public NodeReference Reference { get; }

        /// <summary>
        /// Gets or sets initial data value.
        /// </summary>
        public DataValue DataValue { get; set; }

        /// <summary>
        /// Gets or sets expression.
        /// </summary>
        public Expression Expression
        {
            get
            {
                return this.expression;
            }

            set
            {
                this.expression = value;
                this.expression.Notification = this;
            }
        }

        /// <summary>
        /// Gets or sets script.
        /// </summary>
        public Script Script { get; set; }

        /// <summary>
        /// Set value of attribute.
        /// </summary>
        /// <param name="value">Value.</param>
        public void SetValue(object value)
        {
            if (value != null && !value.Equals(this.DataValue.Value))
            {
                ////if (this.lastValue != null && (bool.Parse(value.ToString()) == bool.Parse(this.lastValue.ToString())))
                ////{
                ////    this.DataValue.Value = value;
                ////}

                ////this.lastValue = this.DataValue.Value;
                ////if (this.lastValue != null && (bool.Parse(value.ToString()) == bool.Parse(this.lastValue.ToString())))
                ////{
                ////    this.DataValue.Value = value;
                ////}

                Logger.LogTrace($"[Business Node]Value changed >>Name: {this.Name}, Old Value: {this.DataValue.Value}, New Value: {value}.");
                this.DataValue = this.DataValue.Clone(value);
                if (this.Script != null)
                {
                    this.Script.Run();
                }

                this.NotifyAll();
            }
            else
            {
                Logger.LogTrace($"[Business Node]Value changed >>Name: {this.Name}, value don't change.");
            }
        }

        /// <summary>
        /// Implementation of IDataChange interface.
        /// </summary>
        public void OnDataChange()
        {
            if (this.Expression != null)
            {
                var o = this.Expression.DataValue;
                if (o.Value != null)
                {
                    var old = this.DataValue.Value;
                    this.DataValue = this.DataValue.Clone(o.Value);
                    if (this.Script != null && !old.Equals(o.Value))
                    {
                        ////this.DataValue.Value = o.Value;
                        this.Script.Run();
                    }

                    this.NotifyAll();
                }
            }
        }

        /// <summary>
        /// Add notification.
        /// </summary>
        /// <param name="dataChange">Notification of IDataChange.</param>
        public void AddNotification(IDataChange dataChange)
        {
            this.dataChangeList.Add(dataChange);
        }

        private void NotifyAll()
        {
            var count = this.dataChangeList.Count;
            for (int i = 0; i < count; i++)
            {
                IDataChange dataChange = this.dataChangeList[i];
                if (dataChange != null)
                {
                    ////ChangedItem changedObject = new ChangedItem();
                    ////changedObject.Name = this.Name;
                    ////changedObject.Value = new VTQ()
                    ////{
                    ////    Id = this.Name,
                    ////    Value = this.DataValue.Value,
                    ////    DataType = (int)this.DataValue.DataType,
                    ////    Timestamp = DateTime.UtcNow,
                    ////    Quality = 1,
                    ////};

                    dataChange.OnDataChange();
                }
            }

            if (this.AppContainer != null)
            {
                var data = new VTQ()
                {
                    Id = this.Name,
                    Value = this.DataValue.Value,
                    DataType = (byte)this.DataValue.DataType,
                    Timestamp = DateTime.UtcNow,
                    Quality = 1,
                };
                this.AppContainer.AddChangedData(data);
            }
        }
    }
}
