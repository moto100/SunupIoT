// <copyright file="SimulatorOptions.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.Simulator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// SimulatorOptions.
    /// </summary>
    public class SimulatorOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether GenerateBoolData.
        /// </summary>
        public bool GenerateBoolData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether GenerateIntData.
        /// </summary>
        public bool GenerateIntData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether GenerateStringData.
        /// </summary>
        public bool GenerateStringData { get; set; }

        /// <summary>
        /// Gets or sets DataInstanceNumber.
        /// </summary>
        public int DataInstanceNumber { get; set; }

        /// <summary>
        /// Gets or sets TimerInterval.
        /// </summary>
        public int TimerInterval { get; set; }

        /// <summary>
        /// Gets or sets MinInt.
        /// </summary>
        public int MinInteger { get; set; }

        /// <summary>
        /// Gets or sets MaxInt.
        /// </summary>
        public int MaxInteger { get; set; }

        /// <summary>
        /// Gets or sets Step.
        /// </summary>
        public int IntegerStep { get; set; }
    }
}
