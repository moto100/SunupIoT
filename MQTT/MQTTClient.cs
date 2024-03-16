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
    using MQTTnet.Packets;
    using MQTTnet.Protocol;
    using Sunup.Contract;
    using Sunup.Diagnostics;

    /// <summary>
    /// MQTTClient.
    /// </summary>
    public class MQTTClient : DataSource
    {
        private MqttClient mqttClient = null;
        private MqttClientOptions options = null;
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
        public override void Start()
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
                        Task.Run(() => this.ConnectToServer());
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
                        Task.Run(() => this.StopServer());
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
                    Task.Run(() => this.Publish(publishedItem.Key, json));
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
        /// <returns>Task.</returns>
        public async Task Publish(string topic, string message)
        {
            try
            {
                if (this.mqttClient == null || this.mqttClient.IsConnected == false)
                {
                    await Task.CompletedTask;
                }

                Logger.LogTrace("[MQTT Client]Publish >>Topic: " + topic + "; QoS: " + this.quality + "; Retained: " + this.retained + ";");
                Logger.LogTrace("[MQTT Client]Publish >>Message: " + message);
                MqttApplicationMessageBuilder mamb = new MqttApplicationMessageBuilder()
                 .WithTopic(topic)
                 .WithPayload(message).WithRetainFlag(this.retained);
                if (this.quality == 0)
                {
                    mamb = mamb.WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce);
                }
                else if (this.quality == 1)
                {
                    mamb = mamb.WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce);
                }
                else if (this.quality == 2)
                {
                    mamb = mamb.WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce);
                }

                await this.mqttClient.PublishAsync(mamb.Build()).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Client] Fail to publish.", exp);
                Logger.LogTrace("[MQTT Client] Fail to publish.", exp);
            }
        }

        private async Task StopServer()
        {
            await this.mqttClient.DisconnectAsync().ConfigureAwait(false);
            this.manuallyStop = true;
        }

        private async Task ConnectToServer()
        {
            try
            {
                var clientId = Guid.NewGuid().ToString().Substring(0, 5);
                MqttClientOptionsBuilder optionsBuilder = new MqttClientOptionsBuilder()
                        .WithTcpServer(this.mqttServer, this.port)
                        ////.WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                        .WithClientId(clientId)
                        .WithCleanSession(true)
                        .WithKeepAlivePeriod(TimeSpan.FromSeconds(5))
                        ////.WithWillDelayInterval(10)
                        .WithWillTopic($"LastWill/{clientId.Trim()}")
                        .WithWillPayload(Encoding.UTF8.GetBytes($"client {clientId} lost the connection!"))
                        .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce);

                if (!string.IsNullOrEmpty(this.user) && !string.IsNullOrEmpty(this.password))
                {
                    optionsBuilder.WithCredentials(this.user, this.password);
                }

                this.options = optionsBuilder.Build();
                var factory = new MqttFactory();
                this.mqttClient = factory.CreateMqttClient() as MqttClient;
                this.mqttClient.ConnectedAsync += this.Connected;
                this.mqttClient.DisconnectedAsync += this.Disconnected;
                this.mqttClient.ApplicationMessageReceivedAsync += this.MqttApplicationMessageReceived;
                await this.mqttClient.ConnectAsync(this.options).ConfigureAwait(false);
                this.manuallyStop = false;
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
                    await this.mqttClient.SubscribeAsync(new MqttClientSubscribeOptions() { TopicFilters = listTopic }).ConfigureAwait(false);
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

                await this.mqttClient.ConnectAsync(this.options).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Client]Disconnected >>", exp);
                Logger.LogTrace("[MQTT Client]Disconnected >>", exp);
            }
        }

        private Task MqttApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                string text = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                string topic = e.ApplicationMessage.Topic;
                string qoS = e.ApplicationMessage.QualityOfServiceLevel.ToString();
                string retained = e.ApplicationMessage.Retain.ToString();
                Logger.LogTrace("[MQTT Client]Received message >>Topic:" + topic + "; QoS: " + qoS + "; Retained: " + retained + ";");
                Logger.LogTrace("[MQTT Client]Received message >>Msg: " + text);

                var json = this.ToJsonString(topic, text);
                if (!string.IsNullOrEmpty(json))
                {
                    this.DataSourceReference.DataSet = json;
                    Task.Run(() => this.NotifyAll());
                }
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Client]MessageReceived >>", exp);
                Logger.LogTrace("[MQTT Client]MessageReceived >>", exp);
            }

            return Task.CompletedTask;
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
