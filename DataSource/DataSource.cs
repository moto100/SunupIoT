// <copyright file="DataSource.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Sunup.Contract;
    using Sunup.ScriptExecutor;

    /// <summary>
    /// DataSource.
    /// </summary>
    public class DataSource
    {
        private List<IDataChange> dataChangeList;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        public DataSource(string name)
        {
            this.Name = name;
            this.dataChangeList = new List<IDataChange>();
            this.SourceType = DataSourceType.Unknown;
            ////this.ReferenceType = ReferenceType.GlobalVariable;
            this.DataSourceReference = new DataSourceReference(this, "DataSource");
        }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets externalSourceType.
        /// </summary>
        public DataSourceType SourceType { get; protected set; }

        /////// <summary>
        /////// Gets or sets ReferenceType.
        /////// </summary>
        ////public ReferenceType ReferenceType { get; set; }

        /// <summary>
        /// Gets or sets SercurityMode.
        /// </summary>
        public SercurityMode SercurityMode { get; set; }

        /// <summary>
        /// Gets or sets DeviceAccessList.
        /// </summary>
        public List<ClientAccess> ClientAccessList { get; set; }

        /// <summary>
        /// Gets or sets DataSourceReference.
        /// </summary>
        public DataSourceReference DataSourceReference { get; set; }

        /// <summary>
        /// AddNotification.
        /// </summary>
        /// <param name="dataChange">dataChange.</param>
        public void AddNotification(IDataChange dataChange)
        {
            this.dataChangeList.Add(dataChange);
        }

        /// <summary>
        /// RemoveNotification.
        /// </summary>
        /// <param name="dataChange">dataChange.</param>
        public void RemoveNotification(IDataChange dataChange)
        {
            this.dataChangeList.Remove(dataChange);
        }

        /// <summary>
        /// Run.
        /// </summary>
        public virtual void Start()
        {
            this.NotifyAll();
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// WriteItem.
        /// </summary>
        /// <param name="dataToWrite">dataToWrite.</param>
        /// <param name="item">item.</param>
        public virtual void Publish(Dictionary<string, dynamic> dataToWrite, WriteItem item)
        {
        }

        /// <summary>
        /// ValidateWrite.
        /// </summary>
        /// <param name="item">item.</param>
        /// <returns>bool.</returns>
        public virtual bool ValidateTobePublishedItem(WriteItem item)
        {
            return true;
        }

        /// <summary>
        /// NotifyAll.
        /// </summary>
        protected void NotifyAll()
        {
            var count = this.dataChangeList.Count;
            for (int i = 0; i < count; i++)
            {
                IDataChange dataChange = this.dataChangeList[i];
                if (dataChange != null)
                {
                    //// notify device to update data.
                    dataChange.OnDataChange();
                }
            }
        }
    }
}
