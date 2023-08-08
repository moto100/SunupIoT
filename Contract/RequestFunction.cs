// <copyright file="RequestFunction.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Contract
{
    /// <summary>
    /// Request function type.
    /// </summary>
    public enum RequestFunction : byte
    {
        /// <summary>
        /// UnKnown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Subscribe
        /// </summary>
        Subscribe = 1,

        /// <summary>
        /// Unsubscribe
        /// </summary>
        Unsubscribe = 2,

        /// <summary>
        /// GetSubscribedData
        /// </summary>
        GetSubscribedData = 3,

        /// <summary>
        /// WriteNode
        /// </summary>
        WriteNode = 4,

        /// <summary>
        /// GetProjectInfos
        /// </summary>
        GetProjectInfos = 5,

        /// <summary>
        /// UpdateProjectInfos
        /// </summary>
        UpdateProjectInfos = 6,

        /// <summary>
        /// GetProject
        /// </summary>
        GetProject = 7,

        /// <summary>
        /// UpdateProject
        /// </summary>
        UpdateProject = 8,

        /// <summary>
        /// GetProjectInfo
        /// </summary>
        GetProjectInfo = 9,

        /// <summary>
        /// DeployProject
        /// </summary>
        DeployProject = 10,

        /// <summary>
        /// KeepAlive
        /// </summary>
        KeepAlive = 11,

        /// <summary>
        /// UndeployProject
        /// </summary>
        UndeployProject = 12,

        /// <summary>
        /// DownloadProject
        /// </summary>
        DownloadProject = 13,

        /// <summary>
        /// UploadProject
        /// </summary>
        UploadProject = 14,

        /// <summary>
        /// Login
        /// </summary>
        Login = 15,

        /// <summary>
        /// StartDeployedProjects
        /// </summary>
        StartDeployedProjects = 16,

        /// <summary>
        /// RefreshToken
        /// </summary>
        RefreshToken = 17,

        /// <summary>
        /// StopDeployedProjects
        /// </summary>
        StopDeployedProjects = 18,

        /// <summary>
        /// GetRuntimeLog
        /// </summary>
        GetRuntimeLog = 19,
    }
}
