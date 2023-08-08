// <copyright file="DataSourceReference.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource
{
    using Sunup.Contract;

    /// <summary>
    /// DataSourceReference.
    /// </summary>
    public class DataSourceReference : IReference
    {
        private DataSource dataSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceReference"/> class.
        /// </summary>
        /// <param name="dataSource">dataSource.</param>
        /// <param name="referenceName">referenceName.</param>
        public DataSourceReference(DataSource dataSource, string referenceName)
        {
            this.ReferenceName = referenceName;
            this.dataSource = dataSource;
        }

        /// <summary>
        /// Gets or sets ReferenceName.
        /// </summary>
        public string ReferenceName { get; set; }

        /// <summary>
        /// Gets or sets reference names.
        /// </summary>
        public string[] ReferenceNames { get; set; }

        /// <summary>
        /// Gets or sets DataSet.
        /// </summary>
        public string DataSet { get; set; }
    }
}