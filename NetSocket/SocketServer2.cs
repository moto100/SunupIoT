// <copyright file="SocketServer2.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.NetSocket
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Sunup.Contract;
    using Sunup.Diagnostics;

    /// <summary>
    /// SocketServer.
    /// </summary>
    public class SocketServer2 : DataSource
    {
        private Socket server;
        private AsyncCallback serverAcceptCallback;
        private ConcurrentDictionary<string, SocketServerClientHandler> socketServerClientHandlers = new ConcurrentDictionary<string, SocketServerClientHandler>();

        private int port;
        private string serverAddress;
        private string password;
        private string user;
        ////private bool retained;
        private string[] items;
        ////private int quality;
        private object lockObject = new object();
        ////private Dictionary<string, DataItem> itemsDictionary;
        ////private Dictionary<string, string[]> publishedItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServer2"/> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="options">options.</param>
        /// <param name="items">items.</param>
        public SocketServer2(string name, SocketOptions options, params string[] items)
            : base(name)
        {
            this.SourceType = DataSourceType.Socket;
            this.serverAddress = options.Server;
            this.port = options.Port;
            this.user = options.User;
            this.password = options.Password;
            this.items = items;
            this.serverAcceptCallback = new AsyncCallback(this.ServerAcceptCallback);
        }

        /// <summary>
        /// Run data source.
        /// </summary>
        public override void Run()
        {
            try
            {
                this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ip = IPAddress.Parse(this.serverAddress);
                var endPoint = new IPEndPoint(ip, this.port);
                this.server.Bind(endPoint);
                this.server.Listen(1000);
                this.server.BeginAccept(this.serverAcceptCallback, null);
            }
            catch (Exception ex)
            {
                this.Stop();

                Logger.LogError($"[Socket Server]Socket Server fail to stop.", ex);
                Logger.LogTrace($"[Socket Server]Socket Server fail to stop.", ex);
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public override async void Stop()
        {
            if (this.server == null)
            {
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    this.server.Close();
                    this.server = null;
                });
                Logger.LogInfo("[Socket Server]Socket Server is stopped.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Socket Server]Socket Server fail to stop.", ex);
                Logger.LogTrace($"[Socket Server]Socket Server fail to stop.", ex);
            }
        }

        /// <summary>
        /// WriteItem.
        /// </summary>
        /// <param name="dataToWrite">dataToWrite.</param>
        /// <param name="item">item.</param>
        public override void Publish(Dictionary<string, dynamic> dataToWrite, WriteItem item)
        {
           ////var publishedItem = this.publishedItems.FirstOrDefault(topic => topic.Value.Contains(item.BoundField));
           ////if (!string.IsNullOrEmpty(publishedItem.Key))
           ////{
           ////     var json = string.Empty;
           ////     try
           ////     {
           ////         json = JsonSerializer.Serialize(dataToWrite);
           ////     }
           ////     catch (Exception ex)
           ////     {
           ////         Logger.LogError("[MQTT Server] Write >>Fail to serialize published data object.", ex);
           ////         Logger.LogTrace("[MQTT Server] Write >>Fail to serialize published data object.", ex);
           ////     }

           ////     if (!string.IsNullOrEmpty(json))
           ////     {
           ////         ////this.Publish(publishedItem.Key, json);
           ////     }
           //// }
           ////else
           ////{
           ////     Logger.LogWarning("[MQTT Server] Write >>Didn't find matched topic.");
           //// }
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

        /////// <summary>
        /////// Publish data.
        /////// </summary>
        /////// <param name="topic">Topic.</param>
        /////// <param name="message">Message.</param>
        ////public void Publish(string topic, string message)
        ////{
        ////    try
        ////    {
        ////        if (this.mqttServer == null || !this.mqttServer.IsStarted)
        ////        {
        ////            return;
        ////        }

        ////        Logger.LogTrace("[MQTT Server]Publish >>Topic: " + topic + "; QoS: " + this.quality + "; Retained: " + this.retained + ";");
        ////        Logger.LogTrace("[MQTT Server]Publish >>Message: " + message);
        ////        MqttApplicationMessageBuilder mamb = new MqttApplicationMessageBuilder()
        ////         .WithTopic(topic)
        ////         .WithPayload(message).WithRetainFlag(this.retained);
        ////        if (this.quality == 0)
        ////        {
        ////            mamb = mamb.WithAtMostOnceQoS();
        ////        }
        ////        else if (this.quality == 1)
        ////        {
        ////            mamb = mamb.WithAtLeastOnceQoS();
        ////        }
        ////        else if (this.quality == 2)
        ////        {
        ////            mamb = mamb.WithExactlyOnceQoS();
        ////        }

        ////        this.mqttServer.PublishAsync(mamb.Build());
        ////    }
        ////    catch (Exception exp)
        ////    {
        ////        Logger.LogError("[MQTT Server] Fail to publish.", exp);
        ////        Logger.LogTrace("[MQTT Server] Fail to publish.", exp);
        ////    }
        ////}

        private void ServerAcceptCallback(IAsyncResult ar)
        {
            Socket client = null;
            try
            {
                client = this.server.EndAccept(ar);
            }
            catch (Exception ex)
            {
                Logger.LogError("[Socket Server] ServerAcceptSocketCallback >>End Accept socket err. ", ex);
                Logger.LogTrace("[Socket Server] ServerAcceptSocketCallback >>End Accept socket err. ", ex);
                client = null;
            }

            try
            {
                this.server.BeginAccept(this.serverAcceptCallback, null);
            }
            catch (Exception ex)
            {
                Logger.LogError("[Socket Server] ServerAcceptSocketCallback >>After accept client socket,Re beginAccept Socket fail. ", ex);
                Logger.LogTrace("[Socket Server] ServerAcceptSocketCallback >>After accept client socket,Re beginAccept Socket fail.", ex);
            }

            if (client != null)
            {
                SocketServerClientHandler c = new SocketServerClientHandler(client);
                this.socketServerClientHandlers.TryAdd(c.ClientId, c);

                Logger.LogInfo($"[Socket Server] ServerAcceptSocketCallback >>Accept client.{c.RemoteIP} ");

                if (c.BeginReceive() == true)
                {
                    c.OnDisconnected += (SocketServerClientHandler client) =>
                    {
                        this.socketServerClientHandlers.TryRemove(client.ClientId, out SocketServerClientHandler socketServerClientHandler);
                        Logger.LogInfo($"[Socket Server] ServerAcceptSocketCallback >>Client {client.RemoteIP} disconected.");
                    };
                }
                else
                {
                    c.Stop();
                    this.socketServerClientHandlers.TryRemove(c.ClientId, out SocketServerClientHandler socketServerClientHandler);
                    Logger.LogInfo($"[Socket Server] ServerAcceptSocketCallback >>Accepted client {c.RemoteIP} removed.cannot begin reveive data");
                }
            }
        }

        ////private string ToJsonString(string topic, string payload)
        ////{
        ////    //var item = this.itemsDictionary[topic];

        ////    //if (item == null || string.IsNullOrEmpty(payload))
        ////    //{
        ////    //    return null;
        ////    //}
        ////    //else
        ////    //{
        ////    //    Logger.LogTrace(payload);
        ////    //    return payload;
        ////    //}
        ////}
    }
}
