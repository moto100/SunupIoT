// <copyright file="WebSocketManager.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServerHost
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net.WebSockets;

    /// <summary>
    /// WebSocketManager.
    /// </summary>
    public class WebSocketManager
    {
        private static ConcurrentDictionary<string, WebSocketProxy> webSockets = new ConcurrentDictionary<string, WebSocketProxy>();

        /// <summary>
        /// Create web socket.
        /// </summary>
        /// <param name="webSocket">web socket.</param>
        /// <param name="httpContext">httpContext.</param>
        /// <returns>WebSocketProxy.</returns>
        public static WebSocketProxy CreateWebSocketProxy(WebSocket webSocket, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            WebSocketProxy webSocketProxy = new WebSocketProxy(webSocket, httpContext);
            webSockets[webSocketProxy.Id] = webSocketProxy;
            return webSocketProxy;
        }
    }
}
