// <copyright file="NodeContainer.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.PlatformModel
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Timers;
    using Sunup.Contract;
    using Sunup.DeviceModel;
    using Sunup.Diagnostics;

    /// <summary>
    /// NodeContainer.
    /// </summary>
    public class NodeContainer : IDataChange
    {
        ////private IList<IDataChange> dataChangeList;
        private Dictionary<string, Node> nodes;
        private List<IReference> globalVariables;
        private DevicesProxy devicesProxy;
        ////private DataCollector dataCollector;
        private ConcurrentDictionary<string, VTQ> changedValuesById;
        private Queue<VTQ> fifoChangedValues;
        private object dataCollectLock = new object();
        private bool generated;
        private List<VTQ> stagedValues;
        private Timer executionTimer;
        private IAppContainerDataChange appContainerDataChange;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeContainer"/> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="containerDataChange">containerDataChange.</param>
        /// <param name="devicesProxy">device proxy.</param>
        public NodeContainer(string name, IAppContainerDataChange containerDataChange, DevicesProxy devicesProxy)
        {
            this.Name = name;
            this.nodes = new Dictionary<string, Node>();
            this.globalVariables = new List<IReference>();
            this.devicesProxy = devicesProxy;
            this.devicesProxy.AddNotification(this);
            this.changedValuesById = new ConcurrentDictionary<string, VTQ>();
            this.fifoChangedValues = new Queue<VTQ>();
            this.stagedValues = new List<VTQ>();
            this.appContainerDataChange = containerDataChange;
        }

        /// <summary>
        /// Gets name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets devicesProxy.
        /// </summary>
        public DevicesProxy DevicesProxy
        {
            get
            {
                return this.devicesProxy;
            }
        }

        /// <summary>
        /// Implementation of IDataChange interface.
        /// </summary>
        public void OnDataChange()
        {
            Logger.LogTrace($"[AppContainer]Got Notification from devices proxy.");
            //// get notification from devices proxy.
            foreach (var node in this.nodes.Values)
            {
                if (node != null)
                {
                    var item = node; //// as BusinessNode;
                    if (item != null && item.Expression != null)
                    {
                        item.Expression.Run();
                    }
                }
            }
        }

        /// <summary>
        /// Return data of whole container.
        /// </summary>
        public void GetData()
        {
            //// to do.
        }

        /// <summary>
        /// Add node.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="dafaultValue">dafaultValue.</param>
        /// <param name="expressionString">expressionString.</param>
        /// <param name="scriptString">scriptString.</param>
        /// <param name="dataChange">dataChange.</param>
        /// <param name="isHiden">isHiden.</param>
        /// <param name="boundDevice">boundDevice.</param>
        /// <param name="boundField">boundField.</param>
        public void AddNode(string name, DataValue dafaultValue, string expressionString = null, string scriptString = null, IDataChange dataChange = null, bool isHiden = false, string boundDevice = null, string boundField = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger.LogError($"[AppContainer]Create a node >>Name is empty or null");
                return;
            }

            Logger.LogInfo($"[AppContainer]Create a node >>Name: {name}");

            var nameWithoutDot = name.Replace(" ", string.Empty).Replace(".", "__");
            var node = new Node(name, nameWithoutDot, dafaultValue);

            if (!string.IsNullOrEmpty(expressionString))
            {
                Logger.LogInfo($"[AppContainer]Create a node >>Name: {name} , Expression: {expressionString}.");
                node.Expression = new Sunup.ScriptLibrary.Expression(expressionString, dafaultValue.Clone());
            }

            if (!string.IsNullOrEmpty(scriptString))
            {
                Logger.LogInfo($"[AppContainer]Create a node >>Name: {name} , Script: {scriptString}.");
                node.Script = new Sunup.ScriptLibrary.Script(scriptString);
            }

            if (!string.IsNullOrEmpty(boundDevice))
            {
                Logger.LogInfo($"[AppContainer]Create a node >>Name: {name} , BoundDevice: {boundDevice}.");
                node.BoundDevice = boundDevice;
            }

            if (!string.IsNullOrEmpty(boundField))
            {
                Logger.LogInfo($"[AppContainer]Create a node >>Name: {name} , BoundField: {boundField}.");
                node.BoundField = boundField;
            }

            if (dataChange != null)
            {
                Logger.LogInfo($"[AppContainer]Create a node >>Name: {name} , add dataChange handlder for it.");
                node.AddNotification(dataChange);
            }
            else if (!isHiden)
            {
                node.AppContainer = this;
            }

            Logger.LogInfo($"[AppContainer]Add node >>Name: {name}");
            this.nodes[nameWithoutDot] = node;
            ////Logger.LogInfo($"[AppContainer]Added node >>Name: {name}");
        }

        /// <summary>
        /// AddGlobalVariables.
        /// </summary>
        /// <param name="varibale">varibale.</param>
        public void AddGlobalVariables(IReference varibale)
        {
            this.globalVariables.Add(varibale);
        }

        /// <summary>
        /// Build up references.
        /// </summary>
        public void Build()
        {
            Logger.LogInfo($"[AppContainer]Build up all nodes.");
            foreach (var nodeItem in this.nodes.Values)
            {
                var node = nodeItem; //// as BusinessNode;
                if (node != null && node.Expression != null && !string.IsNullOrEmpty(node.Expression.ScriptContent))
                {
                    Logger.LogInfo($"[AppContainer]Build node >>Name: {node.Name}. Expression: {node.Expression.ScriptContent}");
                    string transformedScript = null;
                    string expressionScript = node.Expression.ScriptContent;
                    ScriptHelper scriptHelper = new ScriptHelper(expressionScript);
                    var referenceList = new List<IReference>();
                    var referenceNodeList = scriptHelper.TransformScript(this.nodes, out transformedScript);
                    Logger.LogInfo($"[AppContainer]Build node >>Name: {node.Name}, Parse expression for it.");
                    node.Expression.ScriptContent = transformedScript;
                    if (referenceNodeList.Count > 0)
                    {
                        referenceNodeList.ForEach(x =>
                        {
                            x.AddNotification(node.Expression);
                            referenceList.Add(x.Reference);
                        });
                        Logger.LogInfo($"[AppContainer]Build node >>Name: {node.Name}, Add reference list for its expression.");
                    }

                    if (this.globalVariables != null & this.globalVariables.Count > 0)
                    {
                        this.globalVariables.ForEach(x =>
                        {
                            referenceList.Add(x);
                        });
                        Logger.LogInfo($"[AppContainer]Build node >>Name: {node.Name}, Add global reference list for its expression.");
                    }

                    if (this.DevicesProxy != null)
                    {
                        referenceList.Add(this.DevicesProxy.DevicesProxyReference);
                        Logger.LogInfo($"[AppContainer]Build node >>Name: {node.Name}, Add devices proxy reference list for its expression.");

                        Dictionary<string, Device> devices = this.DevicesProxy.Devices;

                        foreach (var device in devices.Values)
                        {
                            ////device.DeviceReference.ReferenceName = device.Name;
                            referenceList.Add(device.DeviceReference);
                        }
                    }

                    node.Expression.ReferenceList = referenceList;
                }

                if (node != null && node.Script != null && !string.IsNullOrEmpty(node.Script.ScriptContent))
                {
                    Logger.LogInfo($"[AppContainer]Build node >>Name: {node.Name}. Script: {node.Script.ScriptContent}");
                    string transformedScript = null;
                    string script = node.Script.ScriptContent;
                    var scriptHelper = new ScriptHelper(script);
                    var referenceList = new List<IReference>();
                    var referenceNodeList = scriptHelper.TransformScript(this.nodes, out transformedScript);
                    Logger.LogInfo($"[AppContainer]Build node >>Name: {node.Name}, Parse script for its script.");
                    if (referenceNodeList.Count > 0)
                    {
                        referenceNodeList.ForEach(x =>
                        {
                            referenceList.Add(x.Reference);
                        });

                        Logger.LogInfo($"[AppContainer]Build node >>Name: {node.Name}, Add reference list for its script.");
                    }

                    if (this.globalVariables != null & this.globalVariables.Count > 0)
                    {
                        this.globalVariables.ForEach(x =>
                        {
                            referenceList.Add(x);
                        });
                        Logger.LogInfo($"[AppContainer]Build node >>Name: {node.Name}, Add global reference list for its script.");
                    }

                    if (this.DevicesProxy != null)
                    {
                        referenceList.Add(this.DevicesProxy.DevicesProxyReference);
                        Logger.LogInfo($"[AppContainer]Build node >>Name: {node.Name}, Add devices proxy reference list for its script.");

                        Dictionary<string, Device> devices = this.DevicesProxy.Devices;

                        foreach (var device in devices.Values)
                        {
                            ////device.DeviceReference.ReferenceName = device.Name;
                            referenceList.Add(device.DeviceReference);
                        }
                    }

                    node.Script.ScriptContent = transformedScript;
                    node.Script.ReferenceList = referenceList;
                }
            }
        }

        /// <summary>
        /// Run.
        /// </summary>
        public void Run()
        {
            this.StartExecutionTimer();
            ////if (this.dataCollector != null)
            ////{
            ////    this.dataCollector.Run();
            ////    Logger.LogInfo($"[AppContainer]Run data collector.");
            ////}

            //// initialize node value.
            if (this.nodes.Count > 0)
            {
                Logger.LogInfo($"[AppContainer]Initialize nodes.");
                foreach (var node in this.nodes.Values)
                {
                    if (node != null)
                    {
                        var item = node; //// as node;

                        if (item != null && item.Expression != null)
                        {
                            item.Expression.Run();
                        }
                        else if (item != null)
                        {
                            var data = new VTQ()
                            {
                                Id = item.Name,
                                Value = item.DataValue.Value,
                                DataType = (byte)item.DataValue.DataType,
                                Timestamp = DateTime.UtcNow,
                                Quality = 1,
                            };
                            this.AddChangedData(data);
                        }
                    }
                }
            }

            if (this.devicesProxy != null)
            {
                this.devicesProxy.Run();
                Logger.LogInfo($"[AppContainer]Run devices proxy.");
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            this.StopExecutionTimer();
            ////if (this.dataCollector != null)
            ////{
            ////    this.dataCollector.Stop();
            ////    Logger.LogInfo($"[AppContainer]Stop data collector.");
            ////}

            if (this.DevicesProxy != null)
            {
                this.DevicesProxy.Stop();
                Logger.LogInfo($"[AppContainer]Stop devices proxy.");
            }
        }

        /// <summary>
        /// WriteItem.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="value">value.</param>
        public void WriteItem(string name, string value)
        {
            if (this.devicesProxy != null && !string.IsNullOrEmpty(name))
            {
                var temp = "Root__" + name.Replace(" ", string.Empty).Replace(".", "__");
                if (this.nodes.ContainsKey(temp))
                {
                    var node = this.nodes[temp];
                    if (string.IsNullOrEmpty(node.BoundDevice))
                    {
                        Logger.LogWarning($"[AppContainer]Try to write data to device >>Device name is null or empty.");
                        return;
                    }

                    if (string.IsNullOrEmpty(node.BoundField))
                    {
                        Logger.LogWarning($"[AppContainer]Try to write data to device >>Field name is null or empty.");
                        return;
                    }

                    if (!string.IsNullOrEmpty(node.BoundDevice) && !string.IsNullOrEmpty(node.BoundField))
                    {
                        object val = null;
                        switch (node.DataValue.DataType)
                        {
                            case DataType.Integer:
                                if (int.TryParse(value, out int outInt))
                                {
                                    val = outInt;
                                }

                                break;
                            case DataType.Bool:
                                if (bool.TryParse(value, out bool outBool))
                                {
                                    val = outBool;
                                }

                                break;

                            case DataType.Float:
                                if (float.TryParse(value, out float outFloat))
                                {
                                    val = outFloat;
                                }

                                break;

                            case DataType.Double:
                                if (double.TryParse(value, out double outDouble))
                                {
                                    val = outDouble;
                                }

                                break;
                            case DataType.DateTime:
                                if (DateTime.TryParse(value, out DateTime dateTime))
                                {
                                    val = dateTime;
                                }

                                break;
                            case DataType.String:
                                val = value;
                                break;
                        }

                        if (val != null)
                        {
                            var item = new WriteItem() { Name = name, Value = val };
                            item.BoundDevice = node.BoundDevice;
                            item.BoundField = node.BoundField;
                            this.devicesProxy.WriteItem(item);
                        }
                        else
                        {
                            Logger.LogWarning($"[AppContainer]WriteItem >>Failed to parse written value. DataType is {node.DataValue.DataType} , but value is {value}");
                        }
                    }
                }
            }
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
            lock (this.dataCollectLock)
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

        private void Execute()
        {
            if (this.generated)
            {
                return;
            }

            this.stagedValues.Clear();
            TimeoutCounter tc = new TimeoutCounter(50);
            while (!tc.HasElapsed() && this.GetChangedValue(out VTQ dequeuedData))
            {
                this.generated = true;
                this.stagedValues.Add(dequeuedData);
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
                        Logger.LogError("[DataCollector] StartExecutionTimer >> Calling Execute() Exception ", ex);
                    }

                    this.executionTimer.Enabled = true;
                };

                this.executionTimer.Start();
                Logger.LogInfo($"[AppContainer]Run data collector.");
            }
        }

        private void StopExecutionTimer()
        {
            if (this.executionTimer != null)
            {
                this.executionTimer.Stop();
                this.executionTimer.Dispose();
                this.executionTimer = null;
                Logger.LogInfo($"[AppContainer]Stop data collector.");
            }
        }
    }
}
