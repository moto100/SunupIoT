// <copyright file="MQTTServer.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.MQTT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Receiving;
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
        public override void Run()
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
                        this.StartServer();
                    }
                }
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public override async void Stop()
        {
            if (this.mqttServer == null)
            {
                return;
            }

            try
            {
                await this.mqttServer?.StopAsync();
                this.mqttServer = null;
                Logger.LogInfo("[MQTT Server]MQTT Server is stopped.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[MQTT Server]MQTT Server fail to stop.", ex);
                Logger.LogTrace($"[MQTT Server]MQTT Server fail to stop.", ex);
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
                    Logger.LogError("[MQTT Server] Write >>Fail to serialize published data object.", ex);
                    Logger.LogTrace("[MQTT Server] Write >>Fail to serialize published data object.", ex);
                }

                if (!string.IsNullOrEmpty(json))
                {
                    this.Publish(publishedItem.Key, json);
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
        public void Publish(string topic, string message)
        {
            try
            {
                if (this.mqttServer == null || !this.mqttServer.IsStarted)
                {
                    return;
                }

                Logger.LogTrace("[MQTT Server]Publish >>Topic: " + topic + "; QoS: " + this.quality + "; Retained: " + this.retained + ";");
                Logger.LogTrace("[MQTT Server]Publish >>Message: " + message);
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

                this.mqttServer.PublishAsync(mamb.Build());
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Server] Fail to publish.", exp);
                Logger.LogTrace("[MQTT Server] Fail to publish.", exp);
            }
        }

        private async void StartServer()
        {
            try
            {
                var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpointBoundIPAddress(IPAddress.Parse(this.mqttServerAddress)).WithDefaultEndpointPort(this.port);
                if (!string.IsNullOrEmpty(this.user) && !string.IsNullOrEmpty(this.password))
                {
                    optionsBuilder.WithConnectionValidator(
                    clientConnection =>
                    {
                        var currentUser = this.user;
                        var currentPWD = this.password;
                        if (this.SercurityMode == SercurityMode.None)
                        {
                            if (currentUser == null || currentPWD == null)
                            {
                                clientConnection.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return;
                            }

                            if (clientConnection.Username != currentUser)
                            {
                                clientConnection.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return;
                            }

                            if (clientConnection.Password != currentPWD)
                            {
                                clientConnection.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return;
                            }
                        }
                        else if (this.SercurityMode == SercurityMode.BlackList)
                        {
                            if (this.ClientAccessList != null && this.ClientAccessList.Exists(x => x.Enabled && x.DeviceId == clientConnection.ClientId))
                            {
                                clientConnection.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                                return;
                            }
                        }
                        else if (this.SercurityMode == SercurityMode.WhithList)
                        {
                            if (this.ClientAccessList != null)
                            {
                                var both = this.ClientAccessList.Exists(x => x.Enabled && !string.IsNullOrEmpty(x.DeviceId) && !string.IsNullOrEmpty(x.UserName) && !string.IsNullOrEmpty(x.Password) && clientConnection.ClientId == x.DeviceId && clientConnection.Username == x.UserName && clientConnection.Password == x.Password);
                                if (!both)
                                {
                                    var deviceMarched = this.ClientAccessList.Exists(x => x.Enabled && !string.IsNullOrEmpty(x.DeviceId) && string.IsNullOrEmpty(x.UserName) && clientConnection.ClientId == x.DeviceId);
                                    if (!deviceMarched)
                                    {
                                        var namePasswordMarched = this.ClientAccessList.Exists(x => x.Enabled && string.IsNullOrEmpty(x.DeviceId) && !string.IsNullOrEmpty(x.UserName) && !string.IsNullOrEmpty(x.Password) && clientConnection.Username == x.UserName && clientConnection.Password == x.Password);
                                        if (!namePasswordMarched)
                                        {
                                            clientConnection.ReasonCode = MqttConnectReasonCode.NotAuthorized;
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        clientConnection.ReasonCode = MqttConnectReasonCode.Success;
                    });
                }

                optionsBuilder.WithSubscriptionInterceptor(
                    clientConnection =>
                    {
                        clientConnection.AcceptSubscription = true;
                    }).WithApplicationMessageInterceptor(
                    clientConnection =>
                    {
                        clientConnection.AcceptPublish = true;
                    });

                this.mqttServer = new MqttFactory().CreateMqttServer() as MqttServer;
                this.mqttServer.StartedHandler = new MqttServerStartedHandlerDelegate(this.OnMqttServerStarted);
                this.mqttServer.StoppedHandler = new MqttServerStoppedHandlerDelegate(this.OnMqttServerStopped);

                this.mqttServer.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(this.OnMqttServerClientConnected);
                this.mqttServer.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(this.OnMqttServerClientDisconnected);
                this.mqttServer.ClientSubscribedTopicHandler = new MqttServerClientSubscribedHandlerDelegate(this.OnMqttServerClientSubscribedTopic);
                this.mqttServer.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(this.OnMqttServerClientUnsubscribedTopic);
                this.mqttServer.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(this.OnMqttServer_ApplicationMessageReceived);

                await this.mqttServer.StartAsync(optionsBuilder.Build());
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Server]", exp);
                Logger.LogTrace("[MQTT Server]", exp);
            }
        }

        private void OnMqttServerStarted(EventArgs e)
        {
            Logger.LogInfo("[MQTT Server]MQTT Server is started.");
        }

        private void OnMqttServerStopped(EventArgs e)
        {
            Logger.LogInfo("[MQTT Server]MQTT Server is stopped.");
        }

        private void OnMqttServerClientConnected(MqttServerClientConnectedEventArgs e)
        {
            Logger.LogInfo($"[MQTT Server]Client[{e.ClientId}] is connected.");
        }

        private void OnMqttServerClientDisconnected(MqttServerClientDisconnectedEventArgs e)
        {
            Logger.LogInfo($"[MQTT Server]Client[{e.ClientId}] is disconnected.");
        }

        private void OnMqttServerClientSubscribedTopic(MqttServerClientSubscribedTopicEventArgs e)
        {
            Logger.LogInfo($"[MQTT Server]Client[{e.ClientId}] subcribed topic[{e.TopicFilter}].");
        }

        private void OnMqttServerClientUnsubscribedTopic(MqttServerClientUnsubscribedTopicEventArgs e)
        {
            Logger.LogInfo($"[MQTT Server]Client[{e.ClientId}] unsubcribed topic[{e.TopicFilter}].");
        }

        private void OnMqttServer_ApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                string text = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
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
                        this.NotifyAll();
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.LogError("[MQTT Server]", exp);
                Logger.LogTrace("[MQTT Server]", exp);
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
