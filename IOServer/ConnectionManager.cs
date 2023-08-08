// <copyright file="ConnectionManager.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Sunup.Contract;

    /// <summary>
    /// ConnectionManager.
    /// </summary>
    public class ConnectionManager
    {
        private static ConcurrentDictionary<string, Connection> connections = new ConcurrentDictionary<string, Connection>();

        private static Application application;

        /// <summary>
        /// Start agent.
        /// </summary>
        /// <param name="appPath">app path.</param>
        /// <param name="appId">appId.</param>
        /// <returns>status of agent.</returns>
        public static bool Start(string appPath, string appId)
        {
            application = new Application(appPath);
            if (application.LoadAppConfig())
            {
                Diagnostics.Logger.LogTrace($"[ConnectionManager]Init >>LoadAppConfig appPath :{appPath}.");
                application.Start(appId);
                Diagnostics.Logger.LogTrace($"[ConnectionManager]Init >>Application started.");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Start agent.
        /// </summary>
        /// <returns>status of agent.</returns>
        public static bool Stop()
        {
            return true;
        }

        /// <summary>
        /// remove connection.
        /// </summary>
        /// <param name="connectionId">connectionId.</param>
        public static void RemoveConnection(string connectionId)
        {
            if (connections.ContainsKey(connectionId))
            {
                Diagnostics.Logger.LogTrace($"[ConnectionManager]RemoveConnection >>ConnectionId : {connectionId}.");
                connections.Remove(connectionId, out Connection connection);
            }
        }

        /// <summary>
        /// Get existing Connection or create a new one.
        /// </summary>
        /// <param name="connectionId">connectionId.</param>
        /// <param name="createIfNot">create if not.</param>
        /// <returns>Connection.</returns>
        public static Connection GetConnection(string connectionId, bool createIfNot = true)
        {
            if (connections.ContainsKey(connectionId))
            {
                Diagnostics.Logger.LogTrace($"[ConnectionManager]GetConnection >> Return exixting connection, connectionId : {connectionId}.");
                return connections[connectionId];
            }
            else
            {
                ////if (connections.Count >= License.ConnectionNumber)
                ////{
                ////    Diagnostics.Logger.LogWarning($"[License] >>Current connection number reach license definition.");
                ////    return null;
                ////}
                ////else
                if (createIfNot)
                {
                    Diagnostics.Logger.LogTrace($"[ConnectionManager]GetConnection >> Create a new connection, connectionId : {connectionId}.");
                    var newConnection = new Connection(connectionId, application);
                    newConnection.Open();
                    connections[connectionId] = newConnection;
                    return newConnection;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
