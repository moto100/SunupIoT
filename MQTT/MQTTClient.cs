// <copyright file="MQTTClient.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.MQTT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Connecting;
    using MQTTnet.Client.Disconnecting;
    using MQTTnet.Client.Options;
    using MQTTnet.Client.Receiving;
    using MQTTnet.Protocol;
    using Sunup.Contract;
    using Sunup.Diagnostics;

    /// <summary>
    /// MQTTClient.
    /// </summary>
    public class MQTTClient : DataSource
    {
        private MqttClient mqttClient = null;
        private IMqttClientOptions options = null;
        private string mqttServer;
        private int port;
        private string password;
        private string user;
        private bool retained;
        private string[] items;
        private int quality;
        private object lockObject = new object();
        private bool manuallyStop = false;
        private Dictionary<string, DataItem> itemsDictionary;
        private Dictionary<string, string[]> publishedItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="MQTTClient"/> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="options">options.</param>
        /// <param name="items">items.</param>
        public MQTTClient(string name, MQTTOptions options, string[] items)
            : base(name)
        {
            this.SourceType = DataSourceType.MQTT;
            this.mqttServer = options.Server;
            this.port = options.Port;
            this.user = options.User;
            this.password = options.Password;
            this.quality = options.Quality;
            this.retained = options.Retained;
            this.items = items;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MQTTClient"/> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="options">options.</param>
        /// <param name="items">items.</param>
        /// <param name="publishedItems">publishedItems.</param>
        public MQTTClient(string name, MQTTOptions options, Dictionary<string, DataItem> items, Dictionary<string, string[]> publishedItems)
            : base(name)
        {
            this.SourceType = DataSourceType.MQTT;
            this.mqttServer = options.Server;
            this.port = options.Port;
            this.user = options.User;
            this.password = options.Password;
            this.quality = options.Quality;
            this.retained = options.Retained;
            this.itemsDictionary = items;
            this.publishedItems = publishedItems;
            if (items != null)
            {
                this.items = items.Keys.ToArray<string>();
            }
        }

        /// <summary>
        /// Run data source.
        /// </summary>
        public override void Run()
        {
            if (this.items == null || this.items.Length == 0)
            {
                return;
            }

            if (this.mqttClient == null || this.mqttClient.IsConnected == false)
            {
                lock (this.lockObject)
                {
                    if (this.mqttClient == null || this.mqttClient.IsConnected == false)
                    {
                        this.ConnectToServer();
                        this.manuallyStop = false;
                    }
                }
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public override void Stop()
        {
            if (this.mqttClient == null)
            {
                return;
            }

            if (this.mqttClient != null && this.mqttClient.IsConnected)
            {
                lock (this.lockObject)
                {
                    if (this.mqttClient != null && this.mqttClient.IsConnected)
                    {
                        this.mqttClient.DisconnectAsync();
                        this.manuallyStop = true;
                    }
                }
            }
        }

        /// <summary>
        /// WriteItem.
        /// </summary>
        /// <param name="dataToWrite">dataToWrite.</param>
        /// <param name="item">item.</param>
        public override void Publish(Dictionary<string, dynamic> dataToWrite, WriteItem item)
        {
            var publishedItem = this.publishedItems.FirstOrDefault(topic => topic.Value.Contains(item.BoundField));
            if (!string.IsNullOrEmpty(publishedItem.Key))
            {
                var json = string.Empty;
                try
                {
                    json = JsonSerializer.Serialize(dataToWrite);
                }
                catch (Exception ex)
                {
                    Logger.LogError("[MQTT Client] Write >>Fail to serialize published data object.", ex);
                    Logger.LogTrace("[MQTT Client] Write >>Fail to serialize published data object.", ex);
                }

                if (!string.IsNullOrEmpty(json))
                {
                    this.Publish(publishedItem.Key, json);
                }
            }
            else
            {
                Logger.LogWarning("[MQTT Client] Write >>Didn't find matched topic.");
            }
        }

        /// <summary>
        /// ValidateWrite.
        /// </summary>
        /// <param name="item">item.</param>
        /// <returns>bool.</returns>
        public override bool ValidateTobePublishedItem(WriteItem item)
        {
            return true;
        }

        /// <summary>
        /// Publish data.
        /// </summary>
        /// <param name="topic">Topic.</param>
        /// <param name="message">Message.</param>
        public void Publish(string topic, string message)
        {
            try
            {
                if (this.mqttClient == null || this.mqttClient.IsConnected == false)
                {
                    return;
                }

                Logger.LogTrace("[MQTT Client]Publish >>Topic: " + topic + "; QoS: " + this.quality + "; Retained: " + this.retained + ";");
                Logger.LogTrace("[MQTT Client]Publish >>Message: " + message);
                MqttApplicationMessageBuilder mamb = new MqttApplicationMessageBuilder()
                 .WithTopic(topic)
                 .WithPayload(message).WithRetainFlag(this.retained);
                if (this.quality == 0)
                {
                    mamb = mamb.WithAtMostOnceQoS();
                }
                else if (this.quality == 1)
                {
                    mamb = mamb.WithAtLeastOnceQoS();
                }
                else if (this.quality == 2)
                {
                    mamb = mamb.WithExactlyOnceQoS();
                }

                this.mqttClient.PublishAsync(mamb.Build());
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Client] Fail to publish.", exp);
                Logger.LogTrace("[MQTT Client] Fail to publish.", exp);
            }
        }

        private void ConnectToServer()
        {
            try
            {
                var clientId = Guid.NewGuid().ToString().Substring(0, 5);
                MqttClientOptionsBuilder optionsBuilder = new MqttClientOptionsBuilder()
                        .WithTcpServer(this.mqttServer, this.port)
                        .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                        .WithClientId(clientId)
                        .WithCleanSession(true)
                        .WithKeepAlivePeriod(TimeSpan.FromSeconds(5))
                        .WithWillDelayInterval(10)
                        .WithWillMessage(new MqttApplicationMessage()
                        {
                            Topic = $"LastWill/{clientId.Trim()}",
                            Payload = Encoding.UTF8.GetBytes($"client {clientId} lost the connection!"),
                            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
                        });

                if (!string.IsNullOrEmpty(this.user) && !string.IsNullOrEmpty(this.password))
                {
                    optionsBuilder.WithCredentials(this.user, this.password);
                }

                this.options = optionsBuilder.Build();
                var factory = new MqttFactory();
                this.mqttClient = factory.CreateMqttClient() as MqttClient;
                this.mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(new Func<MqttClientConnectedEventArgs, Task>(this.Connected));
                this.mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(new Func<MqttClientDisconnectedEventArgs, Task>(this.Disconnected));
                this.mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(new Action<MqttApplicationMessageReceivedEventArgs>(this.MqttApplicationMessageReceived));
                this.mqttClient.ConnectAsync(this.options);
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Client]", exp);
                Logger.LogTrace("[MQTT Client]", exp);
            }
        }

        private async Task Connected(MqttClientConnectedEventArgs e)
        {
            try
            {
                Logger.LogInfo("[MQTT Client]Connected server.");
                List<MqttTopicFilter> listTopic = new List<MqttTopicFilter>();
                if (this.items != null && this.items.Length > 0)
                {
                    foreach (var item in this.items)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var topicFilterBulder = new MqttTopicFilterBuilder().WithTopic(item).Build();
                            listTopic.Add(topicFilterBulder);
                            Logger.LogInfo("[MQTT Client]Subscribe >>Topic: " + item);
                        }
                    }
                }

                if (listTopic.Count > 0)
                {
                    await this.mqttClient.SubscribeAsync(listTopic.ToArray());
                    Logger.LogInfo("[MQTT Client]Subscribe items successfully");
                }
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Client]Connected >>", exp);
                Logger.LogTrace("[MQTT Client]Connected >>", exp);
            }
        }

        private async Task Disconnected(MqttClientDisconnectedEventArgs e)
        {
            try
            {
                if (this.manuallyStop)
                {
                    return;
                }

                Logger.LogInfo("[MQTT Client]Disconnected >>Disconnected server");

                await Task.Delay(TimeSpan.FromSeconds(5));

                await this.mqttClient.ConnectAsync(this.options);
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Client]Disconnected >>", exp);
                Logger.LogTrace("[MQTT Client]Disconnected >>", exp);
            }
        }

        private void MqttApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                string text = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                string topic = e.ApplicationMessage.Topic;
                string qoS = e.ApplicationMessage.QualityOfServiceLevel.ToString();
                string retained = e.ApplicationMessage.Retain.ToString();
                Logger.LogTrace("[MQTT Client]Received message >>Topic:" + topic + "; QoS: " + qoS + "; Retained: " + retained + ";");
                Logger.LogTrace("[MQTT Client]Received message >>Msg: " + text);

                var json = this.ToJsonString(topic, text);
                if (!string.IsNullOrEmpty(json))
                {
                    this.DataSourceReference.DataSet = json;
                    this.NotifyAll();
                }
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Client]MessageReceived >>", exp);
                Logger.LogTrace("[MQTT Client]MessageReceived >>", exp);
            }
        }

        private string ToJsonString(string topic, string payload)
        {
            var item = this.itemsDictionary[topic];

            if (item == null || string.IsNullOrEmpty(payload))
            {
                return null;
            }
            else
            {
                Logger.LogTrace(payload);
                return payload;
            }
        }
    }
}
