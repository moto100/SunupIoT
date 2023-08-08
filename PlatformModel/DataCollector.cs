// <copyright file="DataCollector.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.PlatformModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Timers;
    using Sunup.Contract;
    using Sunup.Diagnostics;

    /// <summary>
    /// DataCollector.
    /// </summary>
    public class DataCollector
    {
        private Dictionary<string, VTQ> changedValuesById;
        private Queue<VTQ> fifoChangedValues;
        private object dataCollectorLock = new object();
        private bool generated;
        private List<VTQ> stagedValues;
        private Timer executionTimer;
        private IAppContainerDataChange appContainerDataChange;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCollector"/> class.
        /// </summary>
        /// <param name="appContainerDataChange">containerDataChange.</param>
        public DataCollector(IAppContainerDataChange appContainerDataChange)
        {
            this.changedValuesById = new Dictionary<string, VTQ>();
            this.fifoChangedValues = new Queue<VTQ>();
            this.appContainerDataChange = appContainerDataChange;
            this.stagedValues = new List<VTQ>();
        }

        /// <summary>
        /// Implementation of IDataChange interface.
        /// </summary>
        /// <param name="changedObject">changed object.</param>
        public void AddChangedData(VTQ changedObject)
        {
            ////if (string.IsNullOrEmpty(changedObject.Name))
            ////{
            ////    return;
            ////}

            ////var vtq = changedObject.Value;
            ////var vtq = changedObject.Value as VTQ;
            ////if (vtq == null)
            ////{
            ////    return;
            ////}

            var id = changedObject.Id;
            lock (this.dataCollectorLock)
            {
                if (!this.changedValuesById.ContainsKey(id))
                {
                    this.changedValuesById[id] = changedObject;
                    this.fifoChangedValues.Enqueue(this.changedValuesById[id]);
                }
                else
                {
                    this.changedValuesById[id] = changedObject;
                }
            }
        }

        /// <summary>
        /// Run.
        /// </summary>
        public void Run()
        {
            this.StartExecutionTimer();
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            this.StopExecutionTimer();
        }

        private void Execute()
        {
            if (this.generated)
            {
                return;
            }

            this.stagedValues.Clear();
            int quality, dataType;
            string id;
            object val;
            DateTime dateTime;
            TimeoutCounter tc = new TimeoutCounter(50);
            while (!tc.HasElapsed() && this.GetChangedValue(out id, out val, out quality, out dataType, out dateTime))
            {
                this.generated = true;
                var exp = new VTQ()
                {
                   Id = id,
                   Value = val,
                   Quality = quality,
                   DataType = dataType,
                   Timestamp = dateTime,
                };
                this.stagedValues.Add(exp);
            }

            if (this.stagedValues.Count > 0 && this.appContainerDataChange != null)
            {
                this.generated = false;
                ////Task.Factory.StartNew(() =>
                ////{
                ////notify  appcontainerbuilder
                this.appContainerDataChange.OnDataChange(this.stagedValues);
                ////});
            }
        }

        private bool GetChangedValue(out string id, out object val, out int quality, out int dataType, out DateTime dateTime)
        {
            id = string.Empty;
            val = string.Empty;
            quality = 0;
            dataType = 0;
            dateTime = DateTime.UtcNow;
            lock (this.dataCollectorLock)
            {
                if (this.fifoChangedValues.Count == 0)
                {
                    return false;
                }

                var vqt = this.fifoChangedValues.Dequeue();
                id = vqt.Id;
                val = vqt.Value;
                quality = vqt.Quality;
                dataType = vqt.DataType;
                dateTime = vqt.Timestamp;
                if (this.changedValuesById.ContainsKey(vqt.Id))
                {
                    this.changedValuesById.Remove(vqt.Id);
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
                    ////this.executionTimer.Enabled = false;
                    try
                    {
                        ////this.Execute();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("[DataCollector] StartExecutionTimer >> Calling Execute() Exception ", ex);
                    }

                    ////this.executionTimer.Enabled = true;
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
