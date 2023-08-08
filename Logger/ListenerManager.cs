// <copyright file="ListenerManager.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Diagnostics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Log infor.
    /// </summary>
    public class ListenerManager
    {
        private static ListenerManager instance;
        private Dictionary<string, IListener> listeners;

        /// <summary>
        /// Initializes static members of the <see cref="ListenerManager"/> class.
        /// </summary>
        static ListenerManager()
        {
            instance = new ListenerManager();
        }

        private ListenerManager()
        {
            this.listeners = new Dictionary<string, IListener>();
        }

        /// <summary>
        /// Gets Listeners.
        /// </summary>
        public static ListenerManager Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Gets Listeners.
        /// </summary>
        public Dictionary<string, IListener> Listeners
        {
            get
            {
                return this.listeners;
            }
        }

        /// <summary>
        /// AddListener.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="listener">listener.</param>
        public void AddListener(string name, IListener listener)
        {
            this.listeners[name] = listener;
        }

        /// <summary>
        /// RemoveListener.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="listener">listener.</param>
        public void RemoveListener(string name, IListener listener)
        {
            this.listeners.Remove(name);
        }
    }
}
