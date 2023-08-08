// <copyright file="WebSocketProxy.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOGateway
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
        }

        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public string Id { get; set; }

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
            do
            {
                var sessionid = this.httpContext.Session.Id;
                var ms = new MemoryStream();
                var buffer = new ArraySegment<byte>(new byte[1024 * 8]);
                result = await this.webSocket.ReceiveAsync(buffer, cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Diagnostics.Logger.LogTrace($"[WebSocketProxy]RecvAsync >>Closing WebSocket: {sessionid}.");
                    ////Sunup.IOServer.IOServerAgent.Close(sessionid);
                    Diagnostics.Logger.LogTrace($"[WebSocketProxy]RecvAsync >>Closed WebSocket: {sessionid}.");
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
                    var content = request.ToString();
                    Diagnostics.Logger.LogTrace($"[WebSocketProxy]RecvAsync >>Request: {sessionid}, Content: {content}.");
                    ////Sunup.IOServer.IOServerAgent.Request(sessionid, content, this);
                    ////var responseString = JsonConvert.SerializeObject(response);
                    ////await this.SendAsync(responseString);
                }
            }
            while (result.EndOfMessage);

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
                    var sessionid = this.httpContext.Session.Id;
                    CancellationToken cancellation = default(CancellationToken);
                    var buf = Encoding.UTF8.GetBytes(msg);
                    var segment = new ArraySegment<byte>(buf);
                    Diagnostics.Logger.LogTrace($"[WebSocketProxy]SendAsync >>Request: {sessionid},Response: {msg}.");
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
