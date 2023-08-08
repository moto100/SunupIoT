// <copyright file="Subscription.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Timers;
    using Sunup.Contract;
    using Sunup.PlatformModel;

    /// <summary>
    /// Subscription.
    /// </summary>
    public class Subscription : IAppContainerDataChange
    {
        private readonly int subscriptionId;
        private readonly Application application;
        private List<string> expressions;
        private ConcurrentDictionary<string, VTQ> changedValuesById;
        private Queue<VTQ> fifoChangedValues;
        private object dataCollectLock = new object();
        private bool generated;
        private List<VTQ> stagedValues;
        private Timer executionTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="subscriptionId">windowId.</param>
        /// <param name="application">application.</param>
        public Subscription(int subscriptionId, Application application)
        {
            this.subscriptionId = subscriptionId;
            this.application = application;
            this.expressions = new List<string>();
            this.changedValuesById = new ConcurrentDictionary<string, VTQ>();
            this.fifoChangedValues = new Queue<VTQ>();
            this.stagedValues = new List<VTQ>();
        }

        /// <summary>
        /// Gets or sets count of Callback.
        /// </summary>
        public ISubscriptionCallback DataChangeCallback { get; set; }

        /// <summary>
        /// Gets a value indicating whether started.
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Gets SubscriptionId.
        /// </summary>
        public int SubscriptionId
        {
            get
            {
                return this.subscriptionId;
            }
        }

        /// <summary>
        /// AddExpression.
        /// </summary>
        /// <param name="expressions">expressions.</param>
        public void AddExpression(string[] expressions)
        {
            var noExixted = new List<string>();
            lock (this.dataCollectLock)
            {
                if (expressions != null && expressions.Length > 0)
                {
                    var count = expressions.Length;
                    for (var i = 0; i < count; i++)
                    {
                        var exp = expressions[i];
                        if (!this.expressions.Exists(t => t == exp))
                        {
                            noExixted.Add(exp);
                            this.expressions.Add(exp);
                        }
                    }
                }
            }

            if (noExixted.Count > 0)
            {
                List<VTQ> vTQ = new List<VTQ>();
                foreach (var exp in noExixted)
                {
                   var data = this.application.AppContainerModel.GetVTQ(exp);
                   if (!string.IsNullOrEmpty(data.Id))
                    {
                        vTQ.Add(data);
                    }
                }

                if (vTQ.Count > 0)
                {
                    this.OnDataChange(vTQ);
                }
            }
        }

        /// <summary>
        /// RemoveExpression.
        /// </summary>
        /// <param name="expressions">expressions.</param>
        public void RemoveExpression(string[] expressions)
        {
            var noExixted = new List<string>();
            lock (this.dataCollectLock)
            {
                if (expressions != null && expressions.Length > 0)
                {
                    var count = expressions.Length;
                    for (var i = 0; i < count; i++)
                    {
                        var exp = expressions[i];
                        if (this.expressions.Exists(t => t == exp))
                        {
                            this.expressions.Remove(exp);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Start.
        /// </summary>
        public void Start()
        {
            if (this.application != null && this.application.AppContainerModel != null)
            {
                this.application.AppContainerModel.AddContainerDataChange(this);
                this.IsStarted = true;
            }

            this.StartExecutionTimer();
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            this.StopExecutionTimer();
            if (this.application != null && this.application.AppContainerModel != null)
            {
                this.application.AppContainerModel.RemoveContainerDataChange(this);
            }
        }

        /// <summary>
        /// get changed symbol data.
        /// </summary>
        /// <returns>return changed data list.</returns>
        public VTQ[] GetChangedData()
        {
            var changes = this.stagedValues.ToArray();
            this.generated = false;
            return changes;
        }

        /// <summary>
        /// Receive data from business wrapper..
        /// </summary>
        /// <param name="changedObjects">changedObjects.</param>
        public void OnDataChange(List<VTQ> changedObjects)
        {
            if (changedObjects == null || changedObjects.Count == 0)
            {
                return;
            }

            //// this method is running under aync mode.
            lock (this.dataCollectLock)
            {
                for (int i = 0; i < changedObjects.Count; i++)
                {
                    var vtq = changedObjects[i];
                    if (!string.IsNullOrEmpty(vtq.Id) && this.expressions.Contains(vtq.Id))
                      {
                          if (!this.changedValuesById.ContainsKey(vtq.Id))
                          {
                              this.changedValuesById[vtq.Id] = vtq;
                              this.fifoChangedValues.Enqueue(this.changedValuesById[vtq.Id]);
                          }
                          else
                          {
                              this.changedValuesById[vtq.Id] = vtq;
                          }
                      }
                }
            }
        }

        private void Execute()
        {
            if (this.generated)
            {
                return;
            }

            this.stagedValues = new List<VTQ>();
            TimeoutCounter tc = new TimeoutCounter(50);
            while (!tc.HasElapsed() && this.GetChangedValue(out VTQ dequeuedData))
            {
                this.generated = true;
                lock (this.dataCollectLock)
                {
                    this.stagedValues.Add(dequeuedData);
                }
            }
            //// Nofify connection to process data.
            if (this.DataChangeCallback != null && this.stagedValues.Count > 0)
            {
                this.DataChangeCallback.OnDataChange(this.GetChangedData(), this.subscriptionId, RequestFunction.Subscribe);
            }
        }

        private bool GetChangedValue(out VTQ dequeuedData)
        {
            lock (this.dataCollectLock)
            {
                if (this.fifoChangedValues.Count == 0)
                {
                    dequeuedData = new VTQ() { };
                    return false;
                }

                dequeuedData = this.fifoChangedValues.Dequeue();
                if (this.changedValuesById.ContainsKey(dequeuedData.Id))
                {
                    this.changedValuesById.Remove(dequeuedData.Id, out VTQ value);
                }
            }

            return true;
        }

        private void StartExecutionTimer()
        {
            if (this.executionTimer == null)
            {
                var interval = 50;
                this.executionTimer = new Timer(interval);
                this.executionTimer.Elapsed += (sender, e) =>
                {
                    this.executionTimer.Enabled = false;
                    try
                    {
                        this.Execute();
                    }
                    catch (Exception ex)
                    {
                        Diagnostics.Logger.LogError($"[Window View]StartExecutionTimer fail.", ex);
                    }

                    this.executionTimer.Enabled = true;
                };

                this.executionTimer.Start();
            }
        }

        private void StopExecutionTimer()
        {
            if (this.executionTimer != null)
            {
                this.executionTimer.Stop();
                this.executionTimer.Dispose();
                this.executionTimer = null;
            }
        }
    }
}
