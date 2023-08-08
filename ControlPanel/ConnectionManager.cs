// <copyright file="ConnectionManager.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ControlPanel
{
    using System.Collections.Generic;

    /// <summary>
    /// ConnectionManager.
    /// </summary>
    public class ConnectionManager
    {
        private static Dictionary<string, Connection> connections = new Dictionary<string, Connection>();

        /// <summary>
        /// Initialize agent.
        /// </summary>
        /// <param name="appPath">app path.</param>
        /// <returns>status of agent.</returns>
        public static bool Init(string appPath)
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
                connections.Remove(connectionId);
            }
        }

        /// <summary>
        /// Get existing Connection or create a new one.
        /// </summary>
        /// <param name="connectionId">connectionId.</param>
        /// <returns>Connection.</returns>
        public static Connection GetConnection(string connectionId)
        {
            if (connections.ContainsKey(connectionId))
            {
                Diagnostics.Logger.LogTrace($"[ConnectionManager]GetConnection >> Return exixting connection, connectionId : {connectionId}.");
                return connections[connectionId];
            }
            else
            {
                Diagnostics.Logger.LogTrace($"[ConnectionManager]GetConnection >> Create a new connection, connectionId : {connectionId}.");
                var newConnection = new Connection(connectionId);
                var dbProxy = new DBConnection.MSSQLProxy(Config.DbConnectionString);
                newConnection.BusinessLogic = new BusinessLogic(dbProxy);
                newConnection.Open();
                connections.TryAdd(connectionId, newConnection);
                return newConnection;
            }
        }
    }
}
