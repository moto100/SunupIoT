// <copyright file="AppContainer.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.App
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Sunup.Contract;
    using Sunup.DataSource;
    using Sunup.DataSource.MQTT;
    using Sunup.DataSource.Simulator;
    using Sunup.DeviceModel;
    using Sunup.Diagnostics;
    using Sunup.InfluxDBConnection;
    using Sunup.PlatformModel;

    /// <summary>
    /// AppContainer.
    /// </summary>
    public class AppContainer : IAppContainerDataChange
    {
        private string filePath;
        private ConcurrentDictionary<string, VTQ> lastVTQSet;
        private object dataLock = new object();
        private NodeContainer nodeContainer;
        ////private int currentNodeNumber = 0;
        private InfluxDBClient influxDBClient;
        private List<string> storedNodes;
        private bool enableInfluxDB = false;
        private List<IAppContainerDataChange> containerDataChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppContainer"/> class.
        /// </summary>
        /// <param name="filePath">filePath.</param>
        public AppContainer(string filePath)
        {
            this.containerDataChanges = new List<IAppContainerDataChange>();
            this.lastVTQSet = new ConcurrentDictionary<string, VTQ>();
            this.filePath = filePath;
            this.storedNodes = new List<string>();
        }

        /// <summary>
        /// AddContainerDataChange.
        /// </summary>
        /// <param name="containerDataChange">containerDataChange.</param>
        public void AddContainerDataChange(IAppContainerDataChange containerDataChange)
        {
            if (containerDataChange == null)
            {
                return;
            }

            this.containerDataChanges.Add(containerDataChange);

            //// send initial data to containerdatachange instance like subscription when this method be called.
            Task.Run(() =>
            {
                lock (this.dataLock)
                {
                    var count = this.lastVTQSet.Count;
                    if (count > 0)
                    {
                        VTQ[] datas = new VTQ[count];
                        this.lastVTQSet.Values.CopyTo(datas, 0);
                        containerDataChange.OnDataChange(new List<VTQ>(datas));
                    }
                }
            });
        }

        /// <summary>
        /// RemoveContainerDataChange.
        /// </summary>
        /// <param name="containerDataChange">containerDataChange.</param>
        public void RemoveContainerDataChange(IAppContainerDataChange containerDataChange)
        {
            this.containerDataChanges.Remove(containerDataChange);
        }

        /// <summary>
        /// GetVTQ.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>VTQ.</returns>
        public VTQ GetVTQ(string id)
        {
            var ret = this.lastVTQSet.ContainsKey(id);
            if (ret)
            {
                return this.lastVTQSet[id];
            }
            else
            {
                return new VTQ()
                {
                    Id = string.Empty,
                };
            }
        }

        /// <summary>
        /// Run.
        /// </summary>
        public void Run()
        {
            Logger.LogInfo($"[Container Wrapper]Load config file.");
            var nodeContainer = this.ReadConfiguration();
            if (nodeContainer == null)
            {
                Logger.LogError($"[Container Wrapper]Load config file >>path is empty or content is invalid Json format.");
                return;
            }

            ////Logger.LogInfo($"[Container Wrapper]Loaded config file.");
            var name = nodeContainer.Value<string>("Name");

            this.nodeContainer = new NodeContainer(name, this, new DevicesProxy());
            Logger.LogInfo($"[Container Wrapper]Load global variables.");
            this.nodeContainer.AddGlobalVariables(new ConsoleReference());

            Logger.LogInfo($"[Container Wrapper]Loaded global variables.");
            var nodes = nodeContainer["Nodes"];

            if (nodes != null)
            {
                Logger.LogInfo($"[Container Wrapper]Loading nodes.");
                this.BuildNodes("Root", nodes, this.nodeContainer);
                Logger.LogInfo($"[Container Wrapper]Loaded nodes.");
            }
            else
            {
                Logger.LogError($"[Container Wrapper]Loading nodes >>none of nodes.");
                return;
            }

            var deviceSecurityJson = nodeContainer["DeviceSecurity"];
            var securityMode = SercurityMode.None;
            var deviceAccessList = new List<ClientAccess>();
            if (deviceSecurityJson != null)
            {
                Logger.LogInfo($"[Container Wrapper]Loading device security configuration.");
                var securityModeInt = deviceSecurityJson.Value<int>("EnableMode");
                securityMode = (SercurityMode)securityModeInt;
                var deviceAccessJson = deviceSecurityJson["Devices"];
                foreach (var deviceJson in deviceAccessJson)
                {
                    var enable = deviceJson.Value<bool>("Enabled");
                    var deviceId = deviceJson.Value<string>("DeviceId");
                    var userName = deviceJson.Value<string>("UserName");
                    var password = deviceJson.Value<string>("Password");
                    deviceAccessList.Add(new ClientAccess()
                    {
                        DeviceId = deviceId,
                        UserName = userName,
                        Password = password,
                        Enabled = enable,
                    });
                }

                Logger.LogInfo($"[Container Wrapper]Loaded device security configuration.");
            }

            var dataStorageJson = nodeContainer["DataStorage"];
            if (dataStorageJson != null)
            {
                this.enableInfluxDB = dataStorageJson.Value<bool>("EnableInfluxDB");
                if (this.enableInfluxDB)
                {
                    Logger.LogInfo($"[Container Wrapper]Loading data storage configuration.");
                    var influxDBAddress = dataStorageJson.Value<string>("InfluxDBAddress");
                    var influxDBOrganizationId = dataStorageJson.Value<string>("InfluxDBOrganizationId");
                    var influxDBBucketName = dataStorageJson.Value<string>("InfluxDBBucketName");
                    var influxDBMeasurement = dataStorageJson.Value<string>("InfluxDBMeasurement");
                    var influxDBAccessToken = dataStorageJson.Value<string>("InfluxDBAccessToken");
                    this.influxDBClient = new InfluxDBClient(influxDBAddress, influxDBOrganizationId, influxDBBucketName, influxDBMeasurement, influxDBAccessToken);
                    Logger.LogInfo($"[Container Wrapper]Loaded data storage configuration.");
                }
            }

            var devicesJson = nodeContainer["Devices"];

            if (devicesJson != null)
            {
                Logger.LogInfo($"[Container Wrapper]Loading devices.");
                Device device = null;
                foreach (var deviceJson in devicesJson)
                {
                    var enable = deviceJson.Value<bool>("Enable");
                    if (!enable)
                    {
                        var deviceName = deviceJson.Value<string>("Name");
                        Logger.LogWarning($"[Container Wrapper]Loading device >>device: {deviceName} is an disabled.");
                        continue;
                    }

                    var deviceType = deviceJson.Value<int>("DeviceType");

                    DataSourceType dataSourceType = (DataSourceType)deviceType;
                    switch (dataSourceType)
                    {
                        case DataSourceType.MQTT:
                            device = this.CreateMQTTDevice(deviceJson, securityMode, deviceAccessList);
                            break;
                        case DataSourceType.DataSourceSimulator:
                            device = this.CreateSourceSimulator(deviceJson);
                            break;
                    }

                    if (device != null)
                    {
                        if (deviceJson["InputDataProcessMode"] != null)
                        {
                            var inputDataProcessModeString = deviceJson.Value<int>("InputDataProcessMode");
                            device.InputDataProcessMode = (DataProcessMode)inputDataProcessModeString;
                        }
                        else
                        {
                            device.InputDataProcessMode = DataProcessMode.ScriptProcess;
                        }
                        ////Logger.LogInfo($"[Container Wrapper]Loading device >>device name {device.Name}.");
                        this.nodeContainer.DevicesProxy.AddDevice(device);
                        ////Logger.LogInfo($"[Container Wrapper]Loaded device >>device name {device.Name}.");
                    }
                    else
                    {
                        Logger.LogWarning($"[Container Wrapper]Loading device >>device: {device.Name} is an unsupported device.");
                    }
                }

                Logger.LogInfo($"[Container Wrapper]Loaded devices.");
            }
            else
            {
                Logger.LogInfo($"[Container Wrapper]Loading devices >>none of device.");
            }

            this.nodeContainer.Build();
            this.nodeContainer.Run();
            if (this.influxDBClient != null)
            {
                this.influxDBClient.Start();
            }
        }

        /// <summary>
        /// Receive data from data collector.
        /// </summary>
        /// <param name="changedObject">changedObject.</param>
        public void OnDataChange(List<VTQ> changedObject)
        {
            ////Task.Factory.StartNew(() =>
            ////{
            ////    lock (this.dataLock)
            ////    {
            ////    }
            ////});

            if (changedObject != null && changedObject.Count > 0)
            {
                var count = changedObject.Count;
                if (count > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var change = changedObject[i];
                        this.lastVTQSet[change.Id] = change;
                    }
                }
            }

            //// Notify subscription to process data.
            if (this.containerDataChanges != null && changedObject != null)
            {
                var count = this.containerDataChanges.Count;
                if (count > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var change = this.containerDataChanges[i];
                        Task.Run(() =>
                        {
                            change.OnDataChange(changedObject);
                        });
                    }
                }
            }

            if (this.enableInfluxDB && this.storedNodes.Count > 0 && this.influxDBClient != null && this.influxDBClient.IsStarted)
            {
                Task.Run(async () =>
                {
                    List<VTQ> list = new List<VTQ>();
                    for (int i = 0; i < changedObject.Count; i++)
                    {
                        if (this.storedNodes.Exists(x => x == changedObject[i].Id))
                        {
                            list.Add(changedObject[i]);
                        }
                    }

                    if (list.Count > 0)
                    {
                        await this.influxDBClient.WriteObject(list.ToArray());
                    }
                });
           }
        }

        /// <summary>
        /// WriteItem.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="value">value.</param>
        public void WriteItem(string name, string value)
        {
            this.nodeContainer.WriteItem(name, value);
        }

        private Device CreateSourceSimulator(JToken deviceJson)
        {
            var deviceName = deviceJson.Value<string>("Name");
            Logger.LogInfo($"[Container Wrapper]Loading simulator device >>device name :{deviceName}.");
            var optionsJson = deviceJson["Options"];
            var itemsJson = deviceJson["Items"];
            var deviceParseJson = deviceJson.Value<string>("ResultParser");

            SimulatorOptions options = new SimulatorOptions();
            options.GenerateIntData = optionsJson.Value<bool>("GenerateIntData");
            options.GenerateBoolData = optionsJson.Value<bool>("GenerateBoolData");
            options.GenerateStringData = optionsJson.Value<bool>("GenerateStringData");
            options.DataInstanceNumber = optionsJson.Value<int>("DataInstanceNumber");
            options.TimerInterval = optionsJson.Value<int>("TimerInterval");
            options.MinInteger = optionsJson.Value<int>("MinInteger");
            options.MaxInteger = optionsJson.Value<int>("MaxInteger");
            options.IntegerStep = optionsJson.Value<int>("IntegerStep");

            List<string> items = new List<string>();
            if (itemsJson != null)
            {
                foreach (var item in itemsJson)
                {
                    items.Add(item.Value<string>());
                }
            }

            SourceSimulator dataSource = new SourceSimulator(deviceName + "_Source", options, items.ToArray());
            var device = new Device(deviceName);
            dataSource.SercurityMode = SercurityMode.None;
            dataSource.ClientAccessList = null;
            device.SetDataSource(dataSource);
            if (deviceParseJson != null)
            {
                device.InParser = new Sunup.ScriptLibrary.Script(deviceParseJson);
            }

            Logger.LogInfo($"[Container Wrapper]Loaded simulator device >>device name :{deviceName}.");
            return device;
        }

        private void BuildNodes(string prefix, JToken token, NodeContainer businessContainer)
        {
            foreach (var node in token)
            {
                ////if (this.currentNodeNumber >= License.DataPointNumber)
                ////{
                ////    return;
                ////}
                ////else
                ////{
                ////    this.currentNodeNumber++;
                ////}

                var name = node.Value<string>("Name");
                var prefixWithName = prefix + "." + name;
                var dataValueTypeInt = node.Value<int>("ValueType");

                var isHiden = false;
                var isStored = false;
                if (node["IsHiden"] != null)
                {
                  isHiden = node.Value<bool>("IsHiden");
                }

                DataType dataValueType = (DataType)dataValueTypeInt;
                object dataValue = null;

                switch (dataValueType)
                {
                    case DataType.Integer:
                        if (node["DefaultValue"] != null)
                        {
                            dataValue = node.Value<int>("DefaultValue");
                        }
                        else
                        {
                            dataValue = 0;
                        }

                        break;
                    case DataType.String:
                        if (node["DefaultValue"] != null)
                        {
                            dataValue = node.Value<string>("DefaultValue");
                        }
                        else
                        {
                            dataValue = string.Empty;
                        }

                        break;
                    case DataType.Bool:
                        if (node["DefaultValue"] != null)
                        {
                            dataValue = node.Value<bool>("DefaultValue");
                        }
                        else
                        {
                            dataValue = false;
                        }

                        break;
                    case DataType.Float:
                        if (node["DefaultValue"] != null)
                        {
                            dataValue = node.Value<float>("DefaultValue");
                        }
                        else
                        {
                            dataValue = 0.0f;
                        }

                        break;
                    case DataType.Double:
                        if (node["DefaultValue"] != null)
                        {
                            dataValue = node.Value<double>("DefaultValue");
                        }
                        else
                        {
                            dataValue = 0.0d;
                        }

                        break;
                    case DataType.DateTime:
                        if (node["DefaultValue"] != null)
                        {
                            dataValue = node.Value<DateTime>("DefaultValue");
                        }
                        else
                        {
                            dataValue = DateTime.Now;
                        }

                        break;
                }

                if (node["IsStored"] != null)
                {
                    isStored = node.Value<bool>("IsStored");
                    if (isStored && dataValueType != DataType.String && dataValueType != DataType.DateTime)
                    {
                        this.storedNodes.Add(prefixWithName.Remove(0, 5));
                    }
                }

                var expression = node.Value<string>("Expression");
                var script = node.Value<string>("Action");
                var inboundDevice = node.Value<string>("InboundDevice");
                var inboundField = node.Value<string>("InboundField");
                var outboundDevice = node.Value<string>("OutboundDevice");
                var outboundField = node.Value<string>("OutboundField");
                businessContainer.AddNode(prefixWithName, new DataValue(dataValue, dataValueType), expression, script, null, isHiden, inboundDevice, inboundField, outboundDevice, outboundField);

                var nodes = node["Nodes"];
                if (nodes != null)
                {
                    this.BuildNodes(prefixWithName, nodes, businessContainer);
                }
            }
        }

        private Device CreateMQTTDevice(JToken deviceJson, SercurityMode sercurityMode, List<ClientAccess> deviceAccesses)
        {
            var deviceName = deviceJson.Value<string>("Name");

            Logger.LogInfo($"[Container Wrapper]Loading MQTT device >>device name :{deviceName}.");

            var optionsJson = deviceJson["Options"];
            var itemsJson = deviceJson["SubscriptionTopics"];
            var deviceParseJson = deviceJson.Value<string>("SubscriptionResultParser");
            var publishedItemsJson = deviceJson["PublishmentTopics"];
            var publishedDeviceParseJson = deviceJson.Value<string>("OnPublishmentParser");

            MQTTOptions options = new MQTTOptions();
            options.Server = optionsJson.Value<string>("Server");
            options.Port = optionsJson.Value<int>("Port");
            options.User = optionsJson.Value<string>("User");
            options.Password = optionsJson.Value<string>("Password");
            options.Quality = optionsJson.Value<int>("Quality");
            options.Retained = optionsJson.Value<bool>("Retained");
            options.Mode = optionsJson.Value<string>("Mode");

            Dictionary<string, DataItem> items = new Dictionary<string, DataItem>();
            if (itemsJson != null)
            {
                foreach (var item in itemsJson)
                {
                    var name = item.Value<string>("Topic");
                    ////var valueType = item.Value<int>("ValueType");
                    items[name] = new DataItem()
                    {
                        Identify = name,
                        ////ValueType = (DataType)valueType,
                    };

                    ////items.Add(item.Value<string>());
                }
            }

            Dictionary<string, string[]> publishedItems = new Dictionary<string, string[]>();
            if (publishedItemsJson != null)
            {
                foreach (var item in publishedItemsJson)
                {
                    var topic = item.Value<string>("Topic");
                    var fields = item.Value<string>("Fields");
                    if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(fields))
                    {
                        publishedItems[topic] = fields.Split(",");
                    }

                    ////items.Add(item.Value<string>());
                }
            }

            DataSource dataSource;
            if (options.Mode == "Server")
            {
                dataSource = new MQTTServer(deviceName + "_Source", options, items, publishedItems);
            }
            else
            {
                dataSource = new MQTTClient(deviceName + "_Source", options, items, publishedItems);
            }

            dataSource.SercurityMode = sercurityMode;
            dataSource.ClientAccessList = deviceAccesses;
            var device = new Device(deviceName);
            device.SetDataSource(dataSource);
            if (deviceParseJson != null)
            {
                device.InParser = new Sunup.ScriptLibrary.Script(deviceParseJson);
            }

            if (publishedDeviceParseJson != null)
            {
                device.OutParser = new Sunup.ScriptLibrary.Script(publishedDeviceParseJson);
            }

            Logger.LogInfo($"[Container Wrapper]Loaded MQTT device >>device name :{deviceName}.");

            return device;
        }

        private JObject ReadConfiguration()
        {
            if (string.IsNullOrEmpty(this.filePath))
            {
                Logger.LogError($"[Container Wrapper]Read configuration >>file path is null or empty.");
                return null;
            }

            var filePath = this.filePath;

            if (File.Exists(filePath))
            {
                StreamReader r = null;
                try
                {
                    r = new StreamReader(filePath);
                    var json = r.ReadToEnd();
                    JObject jo = JsonConvert.DeserializeObject<JObject>(json);
                    r.Close();
                    r.Dispose();
                    return jo;
                }
                catch (Exception ex)
                {
                    if (r != null)
                    {
                        r.Close();
                        r.Dispose();
                    }

                    Logger.LogError($"[Container Wrapper]Read configuration >>It failed to deserialize object, Error : {ex.Message}.", ex);
                }
            }
            else
            {
                Logger.LogError($"[Container Wrapper]Read configuration >>file is not exising.");
            }

            return null;
        }
    }
}
