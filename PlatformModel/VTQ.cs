// <copyright file="VTQ.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.PlatformModel
{
    using System;

    /// <summary>
    /// VTQ.
    /// </summary>
    public struct VTQ
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets dataType.
        /// </summary>
        public byte DataType { get; set; }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets quality.
        /// </summary>
        public byte Quality { get; set; }

        /// <summary>
        /// Gets or sets timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /////// <summary>
        /////// Clone.
        /////// </summary>
        /////// <returns>cloned object.</returns>
        ////public VTQ Clone()
        ////{
        ////    VTQ clone = new VTQ();
        ////    clone.Id = this.Id;
        ////    clone.DataType = this.DataType;
        ////    clone.Quality = this.Quality;
        ////    clone.Value = this.Value;
        ////    clone.Timestamp = DateTime.UtcNow;
        ////    return clone;
        ////}
    }
}
