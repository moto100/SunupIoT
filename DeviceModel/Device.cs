// <copyright file="Device.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DeviceModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Sunup.Contract;
    using Sunup.DataSource;
    using Sunup.Diagnostics;
    using Sunup.ScriptExecutor;
    using Sunup.ScriptLibrary;

    /// <summary>
    /// Device.
    /// </summary>
    public class Device : IDataChange
    {
        private IList<IDataChange> dataChangeList;
        private Dictionary<string, dynamic> fieldValues;
        private Dictionary<string, dynamic> writtenValues;
        private DataSource dataSource;
        private Script inScript;
        private Script outScript;
        private DeviceReference deviceReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        /// <param name="name">device name.</param>
        public Device(string name)
        {
            this.dataChangeList = new List<IDataChange>();
            this.fieldValues = new Dictionary<string, dynamic>();
            this.writtenValues = new Dictionary<string, dynamic>();
            this.Name = name;
            ////this.ReferenceType = ReferenceType.GlobalVariable;
            this.deviceReference = new DeviceReference(this, "Device");
        }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        public string Name { get; set; }

        /////// <summary>
        /////// Gets or sets externalSourceType.
        /////// </summary>
        ////public DataSourceType SourceType { get; protected set; }

        /////// <summary>
        /////// Gets or sets ReferenceType.
        /////// </summary>
        ////public ReferenceType ReferenceType { get; set; }

        /// <summary>
        /// Gets or sets InputDataProcessMode.
        /// </summary>
        public DataProcessMode InputDataProcessMode { get; set; }

        /////// <summary>
        /////// Gets or sets Value.
        /////// </summary>
        ////public object Value { get; set; }

        /////// <summary>
        /////// Gets or sets ReferenceName.
        /////// </summary>
        ////public string ReferenceName { get; set; }

        /////// <summary>
        /////// Gets script.
        /////// </summary>
        ////public DataSource DataSource
        ////{
        ////    get
        ////    {
        ////        return this.dataSource;
        ////    }
        ////}

        /// <summary>
        /// Gets DeviceReference.
        /// </summary>
        public DeviceReference DeviceReference
        {
            get
            {
                return this.deviceReference;
            }
        }

        /// <summary>
        /// Gets or sets script.
        /// </summary>
        public Script InParser
        {
            get
            {
                return this.inScript;
            }

            set
            {
                this.inScript = value;
                if (this.inScript != null)
                {
                    this.inScript.ReferenceList.Add(this.deviceReference);
                    this.inScript.ReferenceList.Add(new ConsoleReference());
                    if (this.dataSource != null)
                    {
                        this.inScript.ReferenceList.Add(this.dataSource.DataSourceReference);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets script.
        /// </summary>
        public Script OutParser
        {
            get
            {
                return this.outScript;
            }

            set
            {
                this.outScript = value;
                if (this.outScript != null)
                {
                    this.outScript.ReferenceList.Add(this.deviceReference);
                    this.outScript.ReferenceList.Add(new ConsoleReference());
                }
            }
        }

        /// <summary>
        /// Add notification.
        /// </summary>
        /// <param name="dataChange">Notification of IDataChange.</param>
        public void AddNotification(IDataChange dataChange)
        {
            this.dataChangeList.Add(dataChange);
        }

        /// <summary>
        /// Remove notification.
        /// </summary>
        /// <param name="dataChange">Notification of IDataChange.</param>
        public void RemoveNotification(IDataChange dataChange)
        {
            this.dataChangeList.Remove(dataChange);
        }

        /// <summary>
        /// Set dataSource.
        /// </summary>
        /// <param name="dataSource">dataSource.</param>
        public void SetDataSource(DataSource dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            if (this.dataSource != null)
            {
                this.RemoveDataSource();
            }

            this.dataSource = dataSource;
            this.dataSource.AddNotification(this);

            ////if (this.script != null && !string.IsNullOrEmpty(this.script.ScriptContent))
            ////{
            ////    var scriptContent = this.script.ScriptContent;
            ////    this.script = new Script(scriptContent);
            ////    ////this.deviceReference.ReferenceName = "$Device";
            ////    this.script.ReferenceList.Add(this.deviceReference);
            ////    this.script.ReferenceList.Add(this.dataSource.DataSourceReference);
            ////    this.script.ReferenceList.Add(new ConsoleReference());
            ////}
        }

        /// <summary>
        /// Remove dataSource.
        /// </summary>
        public void RemoveDataSource()
        {
            if (this.dataSource != null)
            {
                this.dataSource.RemoveNotification(this);
                this.dataSource = null;
            }
        }

        ///// <summary>
        /////// Add Field.
        /////// </summary>
        /////// <param name="fieldName">device field name.</param>
        ////public void AddField(string fieldName)
        ////{
        ////    ////if (string.IsNullOrEmpty(fieldName))
        ////    ////{
        ////    ////    return;
        ////    ////}

        ////    ////if (!this.fieldValues.ContainsKey(fieldName.ToUpper()))
        ////    ////{
        ////    ////    this.fieldValues.Add(fieldName.ToUpper(), null);
        ////    ////}
        ////}

        /// <summary>
        /// Update Field Value.
        /// </summary>
        /// <param name="fieldName">device field name.</param>
        /// <param name="value">value.</param>
        public void SetField(string fieldName, object value)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return;
            }

            var field = fieldName.ToUpper();
            ////if (this.fieldValues.ContainsKey(field))
            ////{
            ////if (this.fieldValues.Count <= License.DataPointNumber)
            ////{
            ////    this.fieldValues[field] = value;
            ////}
            this.fieldValues[field] = value;
            ////}
            ////this.deviceReference.SetField(fieldName, value);
        }

        /// <summary>
        /// Update Field Value.
        /// </summary>
        /// <param name="fieldName">device field name.</param>
        /// <param name="value">value.</param>
        public void SetWrittenField(string fieldName, object value)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return;
            }

            /////var field = fieldName.ToUpper();
            ////if (this.fieldValues.ContainsKey(field))
            ////{
            this.writtenValues[fieldName] = value;
            ////}
            ////this.deviceReference.SetField(fieldName, value);
        }

        /// <summary>
        /// Implementation of IDataChange interface.
        /// </summary>
        public void OnDataChange()
        {
            Logger.LogTrace($"[Device]Data change >> Got data from the device: {this.Name}.");
            Logger.LogTrace($"[Device]Data change >> InputDataProcessMode: {this.InputDataProcessMode}.");
            var changed = false;
            if (this.InputDataProcessMode == DataProcessMode.ScriptProcess)
            {
               if (this.InParser != null)
                {
                    Logger.LogTrace($"[Device]Data change >> Run script to parse returned result from the device: {this.Name}.");
                    this.InParser.Run();
                    changed = true;
                    ////this.dataSource.DataSourceReference.DataSet = null;
                }
            }
            else if (this.InputDataProcessMode == DataProcessMode.SimplyMaping)
            {
                Logger.LogTrace($"[Device]Data change >> Simply map returned result from the device: {this.Name}.");
                if (this.dataSource.DataSourceReference != null && !string.IsNullOrEmpty(this.dataSource.DataSourceReference.DataSet))
                {
                    try
                    {
                        JObject jsonObject = JsonConvert.DeserializeObject<JObject>(this.dataSource.DataSourceReference.DataSet);
                        foreach (var item in jsonObject)
                        {
                            var valueType = item.Value.Type;
                            if (item.Value != null)
                            {
                                switch (valueType)
                                {
                                    case JTokenType.Integer:
                                        changed = true;
                                        this.SetField(item.Key, item.Value.Value<int>());
                                        break;
                                    case JTokenType.Float:
                                        changed = true;
                                        this.SetField(item.Key, item.Value.Value<float>());
                                        break;
                                    case JTokenType.Boolean:
                                        changed = true;
                                        this.SetField(item.Key, item.Value.Value<bool>());
                                        break;
                                    case JTokenType.String:
                                        changed = true;
                                        this.SetField(item.Key, item.Value.Value<string>());
                                        break;
                                    case JTokenType.Date:
                                        changed = true;
                                        this.SetField(item.Key, item.Value.Value<DateTime>());
                                        break;
                                }
                            }
                        }

                        ////jsonObject = null;
                        ////this.dataSource.DataSourceReference.DataSet = null;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"[Device]Data change >>It failed to simply map returned result from the device, Error : {ex.Message}.", ex);
                    }
                }
            }
            else if (this.InputDataProcessMode == DataProcessMode.Complex)
            {
                Logger.LogTrace($"[Device]Data change >> Complex map returned result from the device: {this.Name}.");
                if (this.dataSource.DataSourceReference != null && !string.IsNullOrEmpty(this.dataSource.DataSourceReference.DataSet))
                {
                    try
                    {
                        JObject jsonObject = JsonConvert.DeserializeObject<JObject>(this.dataSource.DataSourceReference.DataSet);
                        foreach (var item in jsonObject)
                        {
                            var valueType = item.Value.Type;
                            if (item.Value != null)
                            {
                                switch (valueType)
                                {
                                    case JTokenType.Integer:
                                        changed = true;
                                        this.deviceReference.SetField(item.Key, item.Value.Value<int>());
                                        break;
                                    case JTokenType.Float:
                                        changed = true;
                                        this.deviceReference.SetField(item.Key, item.Value.Value<float>());
                                        break;
                                    case JTokenType.Boolean:
                                        changed = true;
                                        this.deviceReference.SetField(item.Key, item.Value.Value<bool>());
                                        break;
                                    case JTokenType.String:
                                        changed = true;
                                        this.deviceReference.SetField(item.Key, item.Value.Value<string>());
                                        break;
                                    case JTokenType.Date:
                                        changed = true;
                                        this.deviceReference.SetField(item.Key, item.Value.Value<DateTime>());
                                        break;
                                }
                            }
                        }

                        jsonObject = null;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"[Device]Data change >>It failed to complex map returned result from the device, Error : {ex.Message}.", ex);
                    }
                }

                if (this.InParser != null)
                {
                    Logger.LogTrace($"[Device]Data change >> Run script to parse returned result from the device: {this.Name}.");
                    this.InParser.Run();
                    changed = true;
                }

                ////this.dataSource.DataSourceReference.DataSet = null;
            }

            if (changed)
            {
                this.NotifyAll();
            }
        }

        /// <summary>
        /// Run.
        /// </summary>
        public void Run()
        {
            if (this.dataSource != null)
            {
                ////Task.Factory.StartNew(() =>
                ////{
                    this.dataSource.Run();
                ////});
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            if (this.dataSource != null)
            {
                ////Task.Factory.StartNew(() =>
                ////{
                    this.dataSource.Stop();
                ////});
            }
        }

        /// <summary>
        /// Get value for specified field name.
        /// </summary>
        /// <param name="fieldName">field name.</param>
        /// <returns>return value if has,or return null.</returns>
        public dynamic GetValue(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                Logger.LogTrace($"[Device]Get value >> Field name is null or empty.");
                return null;
            }

            var field = fieldName.ToUpper();
            if (this.fieldValues.ContainsKey(field))
            {
                Logger.LogTrace($"[Device]Get Value >> Got value for the field:{fieldName}.");
                return this.fieldValues[field];
            }
            else
            {
                Logger.LogWarning($"[Device]Get value >> the device: {this.Name} don't have the field:{fieldName}.");
            }

            return null;
            ////return this.deviceReference.GetValue(filedName);
        }

        /// <summary>
        /// Get value for specified field name.
        /// </summary>
        /// <param name="fieldName">field name.</param>
        /// <returns>return value if has,or return null.</returns>
        public dynamic GetWrittenValue(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                Logger.LogTrace($"[Device]Get Written Value >> Field name is null or empty.");
                return null;
            }

            ////var field = fieldName.ToUpper();
            if (this.writtenValues.ContainsKey(fieldName))
            {
                Logger.LogTrace($"[Device]Get Written Value >> Got value for the field:{fieldName}.");
                return this.writtenValues[fieldName];
            }
            else
            {
                Logger.LogWarning($"[Device]Get Written Value >> the device: {this.Name} don't have the field:{fieldName}.");
            }

            return null;
            ////return this.deviceReference.GetValue(filedName);
        }

        /// <summary>
        /// WriteItem.
        /// </summary>
        /// <param name="item">item.</param>
        public void WriteItem(WriteItem item)
        {
            if (!License.IsLicenseValid)
            {
                return;
            }

            if (this.dataSource != null && this.dataSource.ValidateTobePublishedItem(item))
            {
                this.writtenValues.Clear();
                this.writtenValues[item.BoundField] = item.Value;
                if (this.outScript != null)
                {
                    this.outScript.Run();
                    ////var value = this.outScript.GetValue();
                }

                if (this.writtenValues.Count > 0)
                {
                    var temp = new Dictionary<string, dynamic>(this.writtenValues);
                    this.dataSource.Publish(temp, item);
                }
            }
        }

        private void NotifyAll()
        {
            var count = this.dataChangeList.Count;
            for (int i = 0; i < count; i++)
            {
                IDataChange dataChange = this.dataChangeList[i];
                if (dataChange != null)
                {
                    Logger.LogTrace($"[Device]Notify devices proxy >> the device: {this.Name} have new changed data.");
                    //// notify devices proxy to update data.
                    dataChange.OnDataChange();
                }
            }
        }
    }
}
