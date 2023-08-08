// <copyright file="WebSocketService.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServerHost
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// WebSocketService.
    /// </summary>
    public class WebSocketService
    {
        /// <summary>
        /// Map request to web socket.
        /// </summary>
        /// <param name="app">IApplicationBuilder.</param>
        public static void Map(IApplicationBuilder app)
        {
            Diagnostics.Logger.LogInfo($"[WebSocket Service]Inject WebSocketHandler.");
            app.UseWebSockets();
            ////app.UseWebSockets(new WebSocketOptions()
            ////{
            ////    ReceiveBufferSize = 1024 * 1024,
            ////});
            app.Use(WebSocketService.WebSocketHandler);
        }

        private static async Task WebSocketHandler(Microsoft.AspNetCore.Http.HttpContext httpContext, Func<Task> n)
        {
            if (!httpContext.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            var socket = await httpContext.WebSockets.AcceptWebSocketAsync();

            var webSocket = WebSocketManager.CreateWebSocketProxy(socket, httpContext);
            Diagnostics.Logger.LogInfo($"[WebSocket Service]Create WebSocketProxy.");
            await webSocket.RecvAsync(CancellationToken.None);
        }
    }
}
