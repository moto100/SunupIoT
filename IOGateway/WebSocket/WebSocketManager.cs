// <copyright file="WebSocketManager.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOGateway
{
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;

    /// <summary>
    /// WebSocketManager.
    /// </summary>
    public class WebSocketManager
    {
        private static Dictionary<string, WebSocketProxy> webSockets = new Dictionary<string, WebSocketProxy>();

        /// <summary>
        /// Create web socket.
        /// </summary>
        /// <param name="webSocket">web socket.</param>
        /// <param name="httpContext">httpContext.</param>
        /// <returns>WebSocketProxy.</returns>
        public static WebSocketProxy CreateWebSocketProxy(WebSocket webSocket, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            WebSocketProxy webSocketProxy = new WebSocketProxy(webSocket, httpContext);
            var id = httpContext.Session.Id;
            webSocketProxy.Id = id;
            webSockets[id] = webSocketProxy;
            return webSocketProxy;
        }
    }
}
