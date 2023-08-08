// <copyright file="IOServerAgent.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Sunup.Contract;

    /// <summary>
    /// IOServer.
    /// </summary>
    public class IOServerAgent
    {
        private static bool initialized = false;

        /// <summary>
        /// Initialize agent.
        /// </summary>
        /// <param name="appPath">app path.</param>
        /// <param name="appId">appId.</param>
        /// <returns>status of agent.</returns>
        public static bool Init(string appPath, string appId)
        {
            Diagnostics.Logger.LogTrace($"[IOServer]Init >> appPath :{appPath}.");
            initialized = ConnectionManager.Start(appPath, appId);
            return initialized;
        }

        /// <summary>
        /// Stop agent.
        /// </summary>
        /// <returns>status of agent.</returns>
        public static bool Stop()
        {
            Diagnostics.Logger.LogTrace($"[IOServer]Close >> app is closing.");
            return ConnectionManager.Stop();
        }

        /// <summary>
        /// Initialize agent.
        /// </summary>
        /// <param name="connectionId">connectionId.</param>
        /// <returns>
        ///     A <see cref="Task"/> return true;.
        /// </returns>
        public static async Task Close(string connectionId)
        {
            var connection = ConnectionManager.GetConnection(connectionId, false);
            if (connection != null)
            {
                if (connection.ConnectionType == ConnectionType.Opened)
                {
                    Diagnostics.Logger.LogTrace($"[IOServer]Closing connnection, ConnectionId: {connectionId}.");
                    await connection.Close();
                    Diagnostics.Logger.LogTrace($"[IOServer]Closed connnection, ConnectionId: {connectionId}.");
                }

                Diagnostics.Logger.LogTrace($"[IOServer]Removing connnection, ConnectionId: {connectionId}.");
                ConnectionManager.RemoveConnection(connectionId);
                Diagnostics.Logger.LogTrace($"[IOServer]Removed connnection, ConnectionId: {connectionId}.");
            }
        }

        /// <summary>
        /// Receive request, handle and then response back.
        /// </summary>
        /// <param name="connectionId">connection id.</param>
        /// <param name="request">request.</param>
        /// <returns>response.</returns>
        public static DataResponse Request(string connectionId, string request)
        {
            Diagnostics.Logger.LogTrace($"[IOServer]Request connectionId: {connectionId}, request: {request}");
            DataResponse dataResponse;
            if (!initialized)
            {
                dataResponse = new DataResponse()
                {
                    Status = (int)ResultStatus.FailedToInitailzieApp,
                    Message = "Fail to initialize application.",
                };
                return dataResponse;
            }

            if (string.IsNullOrEmpty(connectionId))
            {
                dataResponse = new DataResponse()
                {
                    Status = (int)ResultStatus.InvalidSessionId,
                    Message = "Invalid connectionId.",
                };
                return dataResponse;
            }

            var connection = ConnectionManager.GetConnection(connectionId);
            if (connection == null || connection.ConnectionType == ConnectionType.Closed)
            {
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.ConnectionClosed,
                        Message = "Connection closed.",
                    };
                    return dataResponse;
            }

            return connection.ProcessRequest(request);
        }

        /// <summary>
        /// Receive request, handle and then response back.
        /// </summary>
        /// <param name="connectionId">connection id.</param>
        /// <param name="request">request.</param>
        /// <param name="serverCallback">serverCallback.</param>
        /// <returns><see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Request(string connectionId, string request, ICallback serverCallback = null)
        {
            Diagnostics.Logger.LogTrace($"[IOServer]Request connectionId: {connectionId}, request: {request}");
            DataResponse dataResponse;
            if (!initialized)
            {
                dataResponse = new DataResponse()
                {
                    Status = (int)ResultStatus.FailedToInitailzieApp,
                    Message = "Fail to initialize application.",
                };
                if (serverCallback != null)
                {
                    serverCallback.MessageCallback(JsonConvert.SerializeObject(dataResponse));
                    return;
                }
                else
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(connectionId))
            {
                dataResponse = new DataResponse()
                {
                    Status = (int)ResultStatus.InvalidSessionId,
                    Message = "Invalid connectionId.",
                };
                if (serverCallback != null)
                {
                    serverCallback.MessageCallback(JsonConvert.SerializeObject(dataResponse));
                    return;
                }
                else
                {
                    return;
                }
            }

            var connection = ConnectionManager.GetConnection(connectionId);
            if (connection == null || connection.ConnectionType == ConnectionType.Closed)
            {
                dataResponse = new DataResponse()
                {
                    Status = (int)ResultStatus.ConnectionClosed,
                    Message = "Connection closed.",
                };
                if (serverCallback != null)
                {
                    serverCallback.MessageCallback(JsonConvert.SerializeObject(dataResponse));
                    return;
                }
                else
                {
                    return;
                }
            }

            if (serverCallback != null)
            {
                connection.Callback = serverCallback;
                await connection.ProcessAyncRequest(request);
                return;
            }
        }
    }
}
