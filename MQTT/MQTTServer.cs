// <copyright file="MQTTServer.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.MQTT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using MQTTnet;
    using MQTTnet.Protocol;
    using MQTTnet.Server;
    using Sunup.Contract;
    using Sunup.Diagnostics;

    /// <summary>
    /// MQTTServer.
    /// </summary>
    public class MQTTServer : DataSource
    {
        private MqttServer mqttServer = null;
        private int port;
        private string mqttServerAddress;
        private string password;
        private string user;
        private bool retained;
        private string[] items;
        private int quality;
        private object lockObject = new object();
        private Dictionary<string, DataItem> itemsDictionary;
        private Dictionary<string, string[]> publishedItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="MQTTServer"/> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="options">options.</param>
        /// <param name="items">items.</param>
        public MQTTServer(string name, MQTTOptions options, string[] items)
            : base(name)
        {
            this.SourceType = DataSourceType.MQTT;
            this.mqttServerAddress = options.Server;
            this.port = options.Port;
            this.user = options.User;
            this.password = options.Password;
            this.quality = options.Quality;
            this.retained = options.Retained;
            this.items = items;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MQTTServer"/> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="options">options.</param>
        /// <param name="items">items.</param>
        /// <param name="publishedItems">publishedItems.</param>
        public MQTTServer(string name, MQTTOptions options, Dictionary<string, DataItem> items, Dictionary<string, string[]> publishedItems)
            : base(name)
        {
            this.SourceType = DataSourceType.MQTT;
            this.mqttServerAddress = options.Server;
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

            if (this.mqttServer == null || !this.mqttServer.IsStarted)
            {
                lock (this.lockObject)
                {
                    if (this.mqttServer == null || !this.mqttServer.IsStarted)
                    {
                       Task.Run(() => this.StartServer());
                    }
                }
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public override void Stop()
        {
           Task.Run(() => this.StopServer());
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
                    Logger.LogError("[MQTT Server] Write >>Fail to serialize published data object.", ex);
                    Logger.LogTrace("[MQTT Server] Write >>Fail to serialize published data object.", ex);
                }

                if (!string.IsNullOrEmpty(json))
                {
                    Task.Run(() => this.Publish(publishedItem.Key, json));
                }
            }
           else
           {
                Logger.LogWarning("[MQTT Server] Write >>Didn't find matched topic.");
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
        private async Task Publish(string topic, string message)
        {
            try
            {
                if (this.mqttServer == null || !this.mqttServer.IsStarted)
                {
                    await Task.CompletedTask;
                }

                Logger.LogTrace("[MQTT Server]Publish >>Topic: " + topic + "; QoS: " + this.quality + "; Retained: " + this.retained + ";");
                Logger.LogTrace("[MQTT Server]Publish >>Message: " + message);
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

                await this.mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(mamb.Build())).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Server] Fail to publish.", exp);
                Logger.LogTrace("[MQTT Server] Fail to publish.", exp);
            }
        }

        private async Task StopServer()
        {
            if (this.mqttServer == null)
            {
                return;
            }

            try
            {
                await this.mqttServer.StopAsync().ConfigureAwait(false);
                this.mqttServer = null;
                Logger.LogInfo("[MQTT Server]MQTT Server is stopped.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[MQTT Server]MQTT Server fail to stop.", ex);
                Logger.LogTrace($"[MQTT Server]MQTT Server fail to stop.", ex);
            }
        }

        private async Task StartServer()
        {
            try
            {
                var mqttFactory = new MqttFactory();

                var optionsBuilder = mqttFactory.CreateServerOptionsBuilder()
                    .WithDefaultEndpoint()
                .WithDefaultEndpointBoundIPAddress(IPAddress.Parse(this.mqttServerAddress)).WithDefaultEndpointPort(this.port);

                this.mqttServer = mqttFactory.CreateMqttServer(optionsBuilder.Build());
                if (!string.IsNullOrEmpty(this.user) && !string.IsNullOrEmpty(this.password))
                {
                    this.mqttServer.ValidatingConnectionAsync += clientConnection =>
                    {
                        var currentUser = this.user;
                        var currentPWD = this.password;
                        if (this.SercurityMode == SercurityMode.None)
                        {
                            if (currentUser == null || currentPWD == null)
                            {
                                clientConnection.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return Task.CompletedTask;
                            }

                            if (clientConnection.UserName != currentUser)
                            {
                                clientConnection.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return Task.CompletedTask;
                            }

                            if (clientConnection.Password != currentPWD)
                            {
                                clientConnection.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return Task.CompletedTask;
                            }
                        }
                        else if (this.SercurityMode == SercurityMode.BlackList)
                        {
                            if (this.ClientAccessList != null && this.ClientAccessList.Exists(x => x.Enabled && x.DeviceId == clientConnection.ClientId))
                            {
                                clientConnection.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                                return Task.CompletedTask;
                            }
                        }
                        else if (this.SercurityMode == SercurityMode.WhiteList)
                        {
                            if (this.ClientAccessList != null)
                            {
                                var both = this.ClientAccessList.Exists(x => x.Enabled && !string.IsNullOrEmpty(x.DeviceId) && !string.IsNullOrEmpty(x.UserName) && !string.IsNullOrEmpty(x.Password) && clientConnection.ClientId == x.DeviceId && clientConnection.UserName == x.UserName && clientConnection.Password == x.Password);
                                if (!both)
                                {
                                    var deviceMarched = this.ClientAccessList.Exists(x => x.Enabled && !string.IsNullOrEmpty(x.DeviceId) && string.IsNullOrEmpty(x.UserName) && clientConnection.ClientId == x.DeviceId);
                                    if (!deviceMarched)
                                    {
                                        var namePasswordMarched = this.ClientAccessList.Exists(x => x.Enabled && string.IsNullOrEmpty(x.DeviceId) && !string.IsNullOrEmpty(x.UserName) && !string.IsNullOrEmpty(x.Password) && clientConnection.UserName == x.UserName && clientConnection.Password == x.Password);
                                        if (!namePasswordMarched)
                                        {
                                            clientConnection.ReasonCode = MqttConnectReasonCode.NotAuthorized;
                                            return Task.CompletedTask;
                                        }
                                    }
                                }
                            }
                        }

                        clientConnection.ReasonCode = MqttConnectReasonCode.Success;
                        return Task.CompletedTask;
                    };
                }

                this.mqttServer.InterceptingSubscriptionAsync += clientConnection =>
                {
                    clientConnection.ProcessSubscription = true;
                    return Task.CompletedTask;
                };

                this.mqttServer.InterceptingPublishAsync += clientConnection =>
                {
                    clientConnection.ProcessPublish = true;
                    return Task.CompletedTask;
                };
                this.mqttServer.StartedAsync += this.OnMqttServerStarted;
                this.mqttServer.StoppedAsync += this.OnMqttServerStopped;

                this.mqttServer.ClientConnectedAsync += this.OnMqttServerClientConnected;
                this.mqttServer.ClientDisconnectedAsync += this.OnMqttServerClientDisconnected;
                this.mqttServer.ClientSubscribedTopicAsync += this.OnMqttServerClientSubscribedTopic;
                this.mqttServer.ClientUnsubscribedTopicAsync += this.OnMqttServerClientUnsubscribedTopic;
                this.mqttServer.InterceptingPublishAsync += this.OnMqttServer_ApplicationMessageReceived;

                await this.mqttServer.StartAsync().ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Server]", exp);
                Logger.LogTrace("[MQTT Server]", exp);
            }
        }

        private Task OnMqttServerStarted(EventArgs e)
        {
            Logger.LogInfo("[MQTT Server]MQTT Server is started.");
            return Task.CompletedTask;
        }

        private Task OnMqttServerStopped(EventArgs e)
        {
            Logger.LogInfo("[MQTT Server]MQTT Server is stopped.");
            return Task.CompletedTask;
        }

        private Task OnMqttServerClientConnected(ClientConnectedEventArgs e)
        {
            Logger.LogInfo($"[MQTT Server]Client[{e.ClientId}] is connected.");
            return Task.CompletedTask;
        }

        private Task OnMqttServerClientDisconnected(ClientDisconnectedEventArgs e)
        {
            Logger.LogInfo($"[MQTT Server]Client[{e.ClientId}] is disconnected.");
            return Task.CompletedTask;
        }

        private Task OnMqttServerClientSubscribedTopic(ClientSubscribedTopicEventArgs e)
        {
            Logger.LogInfo($"[MQTT Server]Client[{e.ClientId}] subcribed topic[{e.TopicFilter}].");
            return Task.CompletedTask;
        }

        private Task OnMqttServerClientUnsubscribedTopic(ClientUnsubscribedTopicEventArgs e)
        {
            Logger.LogInfo($"[MQTT Server]Client[{e.ClientId}] unsubcribed topic[{e.TopicFilter}].");
            return Task.CompletedTask;
        }

        private Task OnMqttServer_ApplicationMessageReceived(InterceptingPublishEventArgs e)
        {
            try
            {
                string text = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                string topic = e.ApplicationMessage.Topic;
                string qoS = e.ApplicationMessage.QualityOfServiceLevel.ToString();
                string retained = e.ApplicationMessage.Retain.ToString();
                Logger.LogTrace("[MQTT Server]Received message >>Topic:" + topic + "; QoS: " + qoS + "; Retained: " + retained + ";");
                Logger.LogTrace("[MQTT Server]Received message >>Msg: " + text);
                if (this.items.Contains(topic))
                {
                    var json = this.ToJsonString(topic, text);
                    if (!string.IsNullOrEmpty(json))
                    {
                        this.DataSourceReference.DataSet = json;
                        Task.Run(() => this.NotifyAll());
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Server]", exp);
                Logger.LogTrace("[MQTT Server]", exp);
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
