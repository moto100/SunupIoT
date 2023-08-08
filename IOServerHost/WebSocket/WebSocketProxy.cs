// <copyright file="WebSocketProxy.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServerHost
{
    using System;
    using System.IO;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Sunup.Contract;
    using Sunup.Diagnostics;

    /// <summary>
    /// WebSocketProxy.
    /// </summary>
    public class WebSocketProxy : ICallback
    {
        private WebSocket webSocket;
        private HttpContext httpContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketProxy"/> class.
        /// </summary>
        /// <param name="socket">web socket.</param>
        /// <param name="httpContext">httpContext.</param>
        public WebSocketProxy(WebSocket socket, HttpContext httpContext)
        {
            this.webSocket = socket;
            this.httpContext = httpContext;
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// MessageCallback.
        /// </summary>
        /// <param name="result">result.</param>
        public async void MessageCallback(string result)
        {
            await this.SendAsync(result);
        }

        /// <summary>
        /// Receive data.
        /// </summary>
        /// <param name="cancellationToken">cancellation object.</param>
        /// <returns>task.</returns>
        public async Task<string> RecvAsync(CancellationToken cancellationToken)
        {
            WebSocketReceiveResult result;
            ////StringBuilder content = new StringBuilder();
            do
            {
                StringBuilder content = new StringBuilder();
                do
                {
                    var ms = new MemoryStream();
                    var buffer = new ArraySegment<byte>(new byte[1024 * 8]);
                    result = await this.webSocket.ReceiveAsync(buffer, cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Diagnostics.Logger.LogTrace($"[WebSocketProxy]RecvAsync >>Closing WebSocket: {this.Id}.");
                        await Sunup.IOServer.IOServerAgent.Close(this.Id);
                        Diagnostics.Logger.LogTrace($"[WebSocketProxy]RecvAsync >>Closed WebSocket: {this.Id}.");
                        break;
                    }

                    ms.Write(buffer.Array, buffer.Offset, result.Count - buffer.Offset);
                    ms.Seek(0, SeekOrigin.Begin);
                    var reader = new StreamReader(ms);
                    var request = reader.ReadToEnd();
                    reader.Dispose();
                    ms.Dispose();
                    if (request.Length > 0)
                    {
                        content.Append(request);
                        ////Diagnostics.Logger.LogTrace($"[WebSocketProxy]RecvAsync >>Request: {this.Id}, Content: {request}.");
                        ////Sunup.IOServer.IOServerAgent.Request(this.Id, request, this);
                        ////var responseString = JsonConvert.SerializeObject(response);
                        ////await this.SendAsync(responseString);
                    }
                }
                while (!result.EndOfMessage);
                if (this.webSocket.State == WebSocketState.Open && content.Length > 0)
                {
                    Diagnostics.Logger.LogTrace($"[WebSocketProxy]RecvAsync >>Request: {this.Id}, Content: {content}.");
                    await Sunup.IOServer.IOServerAgent.Request(this.Id, content.ToString(), this);
                }
            }
            while (this.webSocket.State == WebSocketState.Open);

            ////if (content.Length > 0)
            ////{
            ////    Diagnostics.Logger.LogTrace($"[WebSocketProxy]RecvAsync >>Request: {this.Id}, Content: {content}.");
            ////    Sunup.IOServer.IOServerAgent.Request(this.Id, content.ToString(), this);
            ////}

            return string.Empty;
        }

        /// <summary>
        /// Send data back.
        /// </summary>
        /// <param name="msg">request data.</param>
        /// <returns>task.</returns>
        public async Task SendAsync(string msg)
        {
            try
            {
                if (this.webSocket.State == WebSocketState.Open)
                {
                    CancellationToken cancellation = default(CancellationToken);
                    var buf = Encoding.UTF8.GetBytes(msg);
                    var segment = new ArraySegment<byte>(buf);
                    Diagnostics.Logger.LogTrace($"[WebSocketProxy]SendAsync >>Request: {this.Id},Response: {msg}.");
                    await this.webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellation);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"[WebSocket Proxy]SendAsync >>Failed to send message.", ex);
                Logger.LogTrace($"[WebSocket Proxy]SendAsync >>Failed to send message.", ex);
            }
        }
    }
}
