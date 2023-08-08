// <copyright file="BaseTable.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base data table.
    /// </summary>
    /// <typeparam name="T">Table Item Type.</typeparam>
    public class BaseTable<T> : IDisposable
    {
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTable{T}"/> class.
        /// </summary>
        public BaseTable()
        {
            this.Items = new List<T>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="BaseTable{T}"/> class.
        /// </summary>
        ~BaseTable()
        {
            this.Dispose();
        }

        /// <summary>
        /// Gets count of items.
        /// </summary>
        public int Count
        {
            get
            {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// Gets or sets items.
        /// </summary>
        protected List<T> Items { get; set; }

        /// <summary>
        /// Add item into table.
        /// </summary>
        /// <param name="item">Item to be added.</param>
        /// <returns>Index of item.</returns>
        public virtual int AddItem(T item)
        {
            this.Items.Add(item);
            return this.Items.Count - 1;
        }

        /// <summary>
        /// Get item by index.
        /// </summary>
        /// <param name="index">Index of item.</param>
        /// <returns>Found item.</returns>
        public virtual T GetItem(int index)
        {
            return this.Items[index];
        }

        /// <summary>
        /// Clear items of table.
        /// </summary>
        public virtual void Clear()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// Check the item is existing or not.
        /// </summary>
        /// <param name="match">Condition of checking.</param>
        /// <returns>True means existing, false means not.</returns>
        public virtual bool Exists(Predicate<T> match)
        {
            return this.Items.Exists(match);
        }

        /// <summary>
        /// Find out index for specified item.
        /// </summary>
        /// <param name="match">condition of checking.</param>
        /// <returns>Index of matched item in table.</returns>
        public virtual int FindIndex(Predicate<T> match)
        {
            return this.Items.FindIndex(match);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this); ////Notify gc not calling finalizer.
        }

        /// <summary>
        /// Dispose unmanaged and managed resource.
        /// </summary>
        /// <param name="disposeManaged">Dispose unmanaged and managed resource if it is true, otherwise dispose unmaanged resource.</param>
        protected virtual void Dispose(bool disposeManaged)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposeManaged)
            {
                ////TODO:Dispose managed resource.
            }

            //////TODO:Dispose unmanaged resource.
            this.disposed = true;
        }
    }
}