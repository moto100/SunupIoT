// <copyright file="DataValue.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    using System;

    /// <summary>
    /// Data value.
    /// </summary>
    public struct DataValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataValue"/> struct.
        /// </summary>
        /// <param name="value">value.</param>
        /// <param name="dataType">data type.</param>
        public DataValue(object value, DataType dataType)
        {
            this.Value = value;
            this.DataType = dataType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValue"/> struct.
        /// </summary>
        /// <param name="value">Integer value.</param>
        public DataValue(int value)
        {
            this.Value = value;
            this.DataType = DataType.Integer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValue"/> struct.
        /// </summary>
        /// <param name="value">Integer value.</param>
        public DataValue(bool value)
        {
            this.Value = value;
            this.DataType = DataType.Bool;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValue"/> struct.
        /// </summary>
        /// <param name="value">Integer value.</param>
        public DataValue(string value)
        {
            this.Value = value;
            this.DataType = DataType.String;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValue"/> struct.
        /// </summary>
        /// <param name="value">Integer value.</param>
        public DataValue(DateTime value)
        {
            this.Value = value;
            this.DataType = DataType.DateTime;
        }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets data type.
        /// </summary>
        public DataType DataType { get; private set; }

        /// <summary>
        /// Clone a new data value instance from this.
        /// </summary>
        /// <returns>A new data instance.</returns>
        public DataValue Clone()
        {
            var newOne = new DataValue(this.Value, this.DataType);
            return newOne;
        }

        /// <summary>
        /// Clone a new data value instance from this.
        /// </summary>
        /// <param name="value">value.</param>
        /// <returns>A new data instance.</returns>
        public DataValue Clone(object value)
        {
            var newOne = new DataValue(value, this.DataType);
            return newOne;
        }
    }
}
