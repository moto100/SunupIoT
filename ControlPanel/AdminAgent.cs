// <copyright file="AdminAgent.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ControlPanel
{
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Sunup.Contract;

    /// <summary>
    /// IOServer.
    /// </summary>
    public class AdminAgent
    {
        /// <summary>
        /// Initialize agent.
        /// </summary>
        /// <param name="appPath">app path.</param>
        /// <returns>status of agent.</returns>
        public static bool Init(string appPath)
        {
            ////Diagnostics.Logger.LogTrace($"[Control Panel]Init >> try to start deployed app.");
            Task.Run(() =>
            {
                var con = new Connection("<<A virtual connection>>");
                con.StartDeployedProjects();
            });
            return true;
        }

        /// <summary>
        /// Initialize agent.
        /// </summary>
        /// <param name="sessionid">sessionid.</param>
        public static async void Close(string sessionid)
        {
            Diagnostics.Logger.LogTrace($"[ControlPanel]Closing SessionId: {sessionid}.");
            var connection = ConnectionManager.GetConnection(sessionid);
            if (connection != null && connection.ConnectionType == ConnectionType.Opened)
            {
                await connection.Close();
                Diagnostics.Logger.LogTrace($"[ControlPanel]Closed SessionId: {sessionid}.");
                ConnectionManager.RemoveConnection(sessionid);
            }
        }

        /// <summary>
        /// Receive request, handle and then response back.
        /// </summary>
        /// <param name="sessionId">session id.</param>
        /// <param name="request">request.</param>
        /// <returns>response.</returns>
        public static DataResponse Request(string sessionId, string request)
        {
            Diagnostics.Logger.LogTrace($"[ControlPanel]Request sessionId: {sessionId}, request: {request}");
            DataResponse dataResponse;

            if (string.IsNullOrEmpty(sessionId))
            {
                dataResponse = new DataResponse()
                {
                    Status = (int)ResultStatus.InvalidSessionId,
                    Message = "Invalid sessionId.",
                };
                return dataResponse;
            }

            var connection = ConnectionManager.GetConnection(sessionId);
            if (connection.ConnectionType == ConnectionType.Closed)
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
        /// <param name="sessionId">session id.</param>
        /// <param name="request">request.</param>
        /// <param name="serverCallback">serverCallback.</param>
        public static void Request(string sessionId, string request, ICallback serverCallback = null)
        {
            Diagnostics.Logger.LogTrace($"[ControlPanel]Request sessionId: {sessionId}, request: {request}");
            DataResponse dataResponse;

            if (string.IsNullOrEmpty(sessionId))
            {
                dataResponse = new DataResponse()
                {
                    Status = (int)ResultStatus.InvalidSessionId,
                    Message = "Invalid sessionId.",
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

            var connection = ConnectionManager.GetConnection(sessionId);
            if (connection.ConnectionType == ConnectionType.Closed)
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
                //// connection.ProcessAyncRequest(request);
                return;
            }
        }
    }
}
