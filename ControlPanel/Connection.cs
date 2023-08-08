// <copyright file="Connection.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ControlPanel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Sunup.Contract;
    using Sunup.Diagnostics;
    using Sunup.Utility;

    /// <summary>
    /// Connection.
    /// </summary>
    public partial class Connection
    {
        private const string DeployedAppsStr = "deployedApps";
        private const string AppsStr = "apps";
        private static string currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
        private static string deployedApps = Path.Combine(currentPath, DeployedAppsStr);
        private static Dictionary<string, JObject> cachedProjectApps = new Dictionary<string, JObject>();
        private static JArray cachedProjects;
        private static object projectsInfoLock = new object();
        private string connectionId;
        private Session session;

        /// <summary>
        /// Initializes static members of the <see cref="Connection"/> class.
        /// </summary>
        static Connection()
        {
            var appsPath = Path.Combine(currentPath, AppsStr);
            if (!Directory.Exists(appsPath))
            {
                Directory.CreateDirectory(appsPath);
            }

            ////var deployedApps = Path.Combine(currentPath, DeployedAppsStr);
            if (!Directory.Exists(deployedApps))
            {
                Directory.CreateDirectory(deployedApps);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="connectionId">sessionId.</param>
        public Connection(string connectionId)
        {
            this.connectionId = connectionId;
            this.session = new Session(connectionId);
        }

        /// <summary>
        /// Gets or sets Callback.
        /// </summary>
        public ICallback Callback { get; set; }

        /// <summary>
        /// Gets or sets BusinessLogic.
        /// </summary>
        public BusinessLogic BusinessLogic { get; set; }

        /// <summary>
        /// Gets count of ConnectionType.
        /// </summary>
        public ConnectionType ConnectionType { get; private set; }

        /// <summary>
        /// Open connection.
        /// </summary>
        public void Open()
        {
            this.ConnectionType = ConnectionType.Opened;
            Diagnostics.Logger.LogInfo($"[Connection]Open >> ConnectionId : {this.connectionId}.");
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        /// <returns>task.</returns>
        public Task Close()
        {
            var task = Task.Run(() =>
            {
                this.ConnectionType = ConnectionType.Closed;
                Diagnostics.Logger.LogInfo($"[Connection]Closed >> ConnectionId : {this.connectionId}.");
            });

            return task;
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <param name="request">request.</param>
        /// <returns>response.</returns>
        public DataResponse ProcessRequest(string request)
        {
            DataResponse dataResponse = null;
            JObject jsonDocument = null;
            RequestFunction requestFunction = RequestFunction.Unknown;
            var requestStatus = this.VerifyRequest(request, out jsonDocument, out requestFunction);
            if (requestStatus != ResultStatus.Ok)
            {
                return this.ResponseError(requestStatus);
            }

            if (requestFunction == RequestFunction.GetProjectInfos)
            {
                dataResponse = this.GetProjectInfos(jsonDocument);
            }
            else if (requestFunction == RequestFunction.UpdateProjectInfos)
            {
                dataResponse = this.UpdateProjectInfos(jsonDocument);
            }
            else if (requestFunction == RequestFunction.GetProject)
            {
                dataResponse = this.GetProject(jsonDocument);
            }
            else if (requestFunction == RequestFunction.UpdateProject)
            {
                dataResponse = this.UpdateProject(jsonDocument);
            }
            else if (requestFunction == RequestFunction.GetProjectInfo)
            {
                dataResponse = this.GetProjectInfo(jsonDocument);
            }
            else if (requestFunction == RequestFunction.DeployProject)
            {
                dataResponse = this.DeployProject(jsonDocument);
            }
            else if (requestFunction == RequestFunction.UndeployProject)
            {
                dataResponse = this.UndeployProject(jsonDocument);
            }
            else if (requestFunction == RequestFunction.DownloadProject)
            {
                dataResponse = this.DownloadProject(jsonDocument);
            }
            else if (requestFunction == RequestFunction.UploadProject)
            {
                dataResponse = this.UploadProject(jsonDocument);
            }
            else if (requestFunction == RequestFunction.Login)
            {
                dataResponse = this.Login(jsonDocument);
            }
            else if (requestFunction == RequestFunction.StartDeployedProjects)
            {
                dataResponse = this.StartDeployedProjects(jsonDocument);
            }
            else if (requestFunction == RequestFunction.StopDeployedProjects)
            {
                dataResponse = this.StopDeployedProjects(jsonDocument);
            }
            else if (requestFunction == RequestFunction.RefreshToken)
            {
                dataResponse = this.RefreshToken(jsonDocument);
            }
            else if (requestFunction == RequestFunction.GetRuntimeLog)
            {
                dataResponse = this.GetRuntimeLog(jsonDocument);
            }

            return dataResponse;
        }

        /// <summary>
        /// StartDeployedProjects.
        /// </summary>
        public void StartDeployedProjects()
        {
            this.ReadProjectInfos();
            if (cachedProjects == null)
            {
                return;
            }

            var hasChange = false;
            foreach (var proj in cachedProjects)
            {
                if (proj["Deployed"] != null && proj.Value<bool>("Deployed"))
                {
                    var projectId = proj.Value<string>("Id");
                    var processId = proj["ProcessId"] != null && proj["ProcessId"].Type == JTokenType.Integer ? proj.Value<int>("ProcessId") : -1;
                    Diagnostics.Logger.LogInfo($"[Connection]StartDeployedProjects >> try to start deployed app, ConnectionId : {this.connectionId}.");
                    var process = processId > 0 ? ProcessHelper.GetProcess(processId) : null;
                    if (process == null)
                    {
                        var deplopedPath = Path.Combine(deployedApps, projectId);
                        var url = proj.Value<string>("ApiAddress");
                        var appId = proj.Value<string>("Id");
                        var logLevel = proj["LogLevel"] != null ? proj.Value<string>("LogLevel") : "Warning";
                        var args = $"Urls:'{url}' AppId:'{appId}' BasePath:'{deplopedPath}' LogLevel:'{logLevel}'";
                        var proId = ProcessHelper.StartProcess(args);
                        if (proId < 0)
                        {
                            var name = proj.Value<string>("Name");
                            Diagnostics.Logger.LogWarning($"[Connection]StartDeployedProjects >> Failed to run a deployed app, ConnectionId : {this.connectionId}. App name:{name}");
                        }
                        else
                        {
                            proj["ProcessId"] = proId;
                            ////proj["Deployed"] = true;
                            hasChange = true;
                        }
                    }
                    else
                    {
                        Diagnostics.Logger.LogInfo($"[Connection]StartDeployedProjects >> the deployed app is runing and ignore to start new one, ConnectionId : {this.connectionId}.ProcessId : {processId}.");
                    }

                    Diagnostics.Logger.LogInfo($"[Connection]StartDeployedProjects >> Started to run a deployed app, ConnectionId : {this.connectionId}.");
                }
            }

            if (hasChange)
            {
                JObject jsonDocument = new JObject();
                jsonDocument["data"] = cachedProjects;
                this.UpdateProjectInfos(jsonDocument);
            }
        }

        /// <summary>
        /// StopDeployedProjects.
        /// </summary>
        public void StopDeployedProjects()
        {
            this.ReadProjectInfos();
            if (cachedProjects == null)
            {
                return;
            }

            var hasChange = false;
            foreach (var proj in cachedProjects)
            {
                if (proj["Deployed"] != null && proj.Value<bool>("Deployed"))
                {
                    var projectId = proj.Value<string>("Id");
                    var processId = proj["ProcessId"] != null && proj["ProcessId"].Type == JTokenType.Integer ? proj.Value<int>("ProcessId") : -1;
                    if (processId > 0)
                    {
                        proj["ProcessId"] = -1;
                        ////proj["Deployed"] = false;
                        hasChange = true;
                        ////Diagnostics.Logger.LogInfo($"[Connection]StopDeployedProjects >> try to stop a running app, ConnectionId : {this.connectionId}, ProcessId : {processId}.");
                        ////if (!ProcessHelper.KillProcess(processId))
                        ////{
                        ////    Diagnostics.Logger.LogWarning($"[Connection]StopDeployedProjects >> Failed to stop a running app, ConnectionId : {this.connectionId}, ProcessId : {processId}.");
                        ////}
                        ////else
                        ////{
                        ////    proj["ProcessId"] = null;
                        ////    ////proj["Deployed"] = false;
                        ////    hasChange = true;
                        ////}
                    }
                }
            }

            if (hasChange)
            {
                JObject jsonDocument = new JObject();
                jsonDocument["data"] = cachedProjects;
                this.UpdateProjectInfos(jsonDocument);
            }

            ProcessHelper.KillProcess();
        }

        private DataResponse StartDeployedProjects(JObject jsonDocument)
        {
            var dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.StartDeployedProjects;
            this.StartDeployedProjects();
            return dataResponse;
        }

        private DataResponse StopDeployedProjects(JObject jsonDocument)
        {
            var dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.StopDeployedProjects;
            this.StopDeployedProjects();
            return dataResponse;
        }

        private DataResponse Login(JObject jsonDocument)
        {
            var userName = jsonDocument.Value<string>("userName");
            var password = jsonDocument.Value<string>("password");
            Diagnostics.Logger.LogInfo($"[Connection]Login >> Verifying username and password, userName : {userName}, password : {password}.");
            var dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.Login;

            if (userName == "sunup" && password == "temptemp")
            {
                dataResponse.Data = new
                {
                    token = Guid.NewGuid().ToString(),
                    name = userName,
                    email = string.Empty,
                    id = 10000,
                    time = DateTime.Now,
                };
                Diagnostics.Logger.LogInfo($"[Connection]Login >> Verified username and password, userName : {userName}, password : {password}.");
            }
            else
            {
                dataResponse.Status = (byte)ResultStatus.InvalidUsernamePassword;
                dataResponse.Message = "Invalid username or password";
                Diagnostics.Logger.LogInfo($"[Connection]Login >> Invalid username or password, userName : {userName}, password : {password}.");
            }

            return dataResponse;
        }

        private DataResponse RefreshToken(JObject jsonDocument)
        {
            Diagnostics.Logger.LogInfo($"[Connection]RefreshToken >>.");
            var dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.RefreshToken;
            dataResponse.Data = new
            {
                msg = "ok",
                token = Guid.NewGuid().ToString(),
            };

            return dataResponse;
        }

        private DataResponse UndeployProject(JObject jsonDocument)
        {
            var projectId = jsonDocument.Value<string>("projectId");
            DataResponse dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.UndeployProject;
            var proj = this.GetProjectInfo(projectId);
            var processId = proj["ProcessId"] != null && proj["ProcessId"].Type == JTokenType.Integer ? proj.Value<int>("ProcessId") : -1;
            if (processId > 0)
            {
                Diagnostics.Logger.LogInfo($"[Connection]UndeployProject >> Stopping a running app, ConnectionId : {this.connectionId}, ProcessId : {processId}.");
                if (!ProcessHelper.KillProcess(processId))
                {
                    Diagnostics.Logger.LogWarning($"[Connection]UndeployProject >> Failed to stop a running app, ConnectionId : {this.connectionId}, ProcessId : {processId}.");

                    dataResponse.Status = (int)ResultStatus.FailedToStopRunningApp;
                    dataResponse.Message = "Failed to stop the running app.";
                    return dataResponse;
                }
            }

            proj["ProcessId"] = null;
            proj["Deployed"] = false;
            this.UpdateProjectInfo(proj);
            dataResponse.Message = "Succeed to stop the app process.";

            return dataResponse;
        }

        private DataResponse GetRuntimeLog(JObject jsonDocument)
        {
            var level = jsonDocument.Value<string>("level");
            var days = jsonDocument.Value<int>("days");
            DataResponse dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.GetRuntimeLog;
            dataResponse.Data = this.BusinessLogic.GetRuntimeLog(level, days);

            return dataResponse;
        }

        private DataResponse DeployProject(JObject jsonDocument)
        {
            var projectId = jsonDocument.Value<string>("projectId");
            DataResponse dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.DeployProject;
            var appPath = Path.Combine(currentPath, AppsStr, projectId);
            var projectPath = Path.Combine(appPath, "project.json");
            ////var deployedApps = Path.Combine(currentPath, DeployedAppsStr);
            var proj = this.GetProjectInfo(projectId);
            Diagnostics.Logger.LogInfo($"[Connection]DeployProject >> Try to run a delopyed app, ConnectionId : {this.connectionId}.");

            if (proj == null)
            {
                Diagnostics.Logger.LogWarning($"[Connection]DeployProject >> Project info is not existing, ConnectionId : {this.connectionId}.");

                dataResponse.Status = (int)ResultStatus.ProjectInfoIsNotExisting;
                dataResponse.Message = "Project info is not existing.";
                return dataResponse;
            }

            if (!Directory.Exists(appPath) || !File.Exists(projectPath))
            {
                Diagnostics.Logger.LogWarning($"[Connection]DeployProject >> Project file is not existing, ConnectionId : {this.connectionId}.");

                dataResponse.Status = (int)ResultStatus.FileIsNotExisting;
                dataResponse.Message = "Project config file is not existing.";
                return dataResponse;
            }

            var processId = proj["ProcessId"] != null && proj["ProcessId"].Type == JTokenType.Integer ? proj.Value<int>("ProcessId") : -1;
            if (processId > 0)
            {
                if (!ProcessHelper.KillProcess(processId))
                {
                    Diagnostics.Logger.LogWarning($"[Connection]DeployProject >> Failed to stop the running app, ConnectionId : {this.connectionId}.");

                    dataResponse.Status = (int)ResultStatus.FailedToStopRunningApp;
                    dataResponse.Message = "Failed to stop the running app.";
                    return dataResponse;
                }
            }

            FileProcess fileProcess = new FileProcess();
            var deplopedPath = Path.Combine(deployedApps, projectId);
            bool copy = fileProcess.CopyDirectory(appPath, deplopedPath, true);
            if (!copy)
            {
                Diagnostics.Logger.LogWarning($"[Connection]DeployProject >> Failed to copy project files to deployment folder, ConnectionId : {this.connectionId}.");

                dataResponse.Status = (int)ResultStatus.FailedToCopyFile;
                dataResponse.Message = "Failed to copy file.";
                return dataResponse;
            }

            var url = proj.Value<string>("ApiAddress");
            var appId = proj.Value<string>("Id");
            var logLevel = proj["LogLevel"] != null ? proj.Value<string>("LogLevel") : "Warning";
            var args = $"Urls:'{url}' AppId:'{appId}' BasePath:'{deplopedPath}' LogLevel:'{logLevel}'";
            var proId = ProcessHelper.StartProcess(args);
            if (proId < 0)
                {
                var name = proj.Value<string>("Name");
                Diagnostics.Logger.LogWarning($"[Connection]StartDeployedProject >> Failed to run a deployed app, ConnectionId : {this.connectionId}. App name:{name}");

                dataResponse.Status = (int)ResultStatus.FailedToStartAppProcess;
                dataResponse.Message = "Failed to start app process.";
                return dataResponse;
                }

            proj["ProcessId"] = proId;
            proj["Deployed"] = true;
            this.UpdateProjectInfo(proj);
            dataResponse.Message = "Succeed to start app process.";
            Diagnostics.Logger.LogInfo($"[Connection]DeployProject >> Succeeded to start app process, ConnectionId : {this.connectionId}.");

            return dataResponse;
        }

        private JToken GetProjectInfo(string projectId)
        {
            this.ReadProjectInfos();
            if (cachedProjects == null)
            {
                return null;
            }

            foreach (var item in cachedProjects)
            {
                if (item.Value<string>("Id") == projectId)
                {
                    return item;
                }
            }

            return null;
        }

        private void UpdateProjectInfo(JToken updeteditem)
        {
            this.ReadProjectInfos();
            if (cachedProjects == null)
            {
                return;
            }

            var count = cachedProjects.Count;
            var hasChange = false;
            for (var i = 0; i < count; i++)
            {
                var item = cachedProjects[i];
                if (item.Value<string>("Id") == updeteditem.Value<string>("Id"))
                {
                    cachedProjects[i] = updeteditem;
                    hasChange = true;
                }
                ////else
                ////{
                ////    item["Deployed"] = false;
                ////}
            }

            if (hasChange)
            {
                JObject jsonDocument = new JObject();
                jsonDocument["data"] = cachedProjects;
                this.UpdateProjectInfos(jsonDocument);
            }
        }

        private void ReadProjectInfos()
        {
            var projectsPath = Path.Combine(currentPath, "projects.json");
            if (cachedProjects == null && File.Exists(projectsPath))
            {
                lock (projectsInfoLock)
                {
                    if (cachedProjects == null && File.Exists(projectsPath))
                    {
                        FileProcess fileProcess = new FileProcess();
                        if (fileProcess.ReadFile(projectsPath, out string data))
                        {
                            try
                            {
                                cachedProjects = JArray.Parse(data);
                                Diagnostics.Logger.LogInfo($"[Connection]ReadProjectInfos >> Read all projects info, ConnectionId : {this.connectionId}.");
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("[Connection] ReadProjects >> ", ex);
                            }
                        }
                    }
                }
            }
        }

        private void ReadProject(string projectId)
        {
            ////var currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
            ////var appsPath = Path.Combine(currentPath, AppsStr);
            ////if (!Directory.Exists(appsPath))
            ////{
            ////    Directory.CreateDirectory(appsPath);
            ////}
            if (cachedProjectApps.ContainsKey(projectId) && cachedProjectApps[projectId] != null)
            {
                return;
            }

            var appPath = Path.Combine(currentPath, AppsStr, projectId);
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
                return;
            }

            var projectPath = Path.Combine(appPath, "project.json");
            if ((!cachedProjectApps.ContainsKey(projectId) || cachedProjectApps[projectId] == null) && File.Exists(projectPath))
            {
                FileProcess fileProcess = new FileProcess();
                if (fileProcess.ReadFile(projectPath, out string data))
                {
                    try
                    {
                        cachedProjectApps[projectId] = JObject.Parse(data);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("[Connection] ReadProject >> ", ex);
                    }
                }
            }
        }

        private DataResponse GetProjectInfos(JObject jsonDocument)
        {
            Diagnostics.Logger.LogInfo($"[Connection]GetProjectInfos >> Getting all projects info, ConnectionId : {this.connectionId}.");

            DataResponse dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.GetProjectInfos;
            this.ReadProjectInfos();
            dataResponse.Data = cachedProjects;
            Diagnostics.Logger.LogInfo($"[Connection]GetProjectInfos >> Got all projects info, ConnectionId : {this.connectionId}.");

            return dataResponse;
        }

        private DataResponse GetProjectInfo(JObject jsonDocument)
        {
            var projectId = jsonDocument.Value<string>("projectId");
            Diagnostics.Logger.LogInfo($"[Connection]GetProjectInfo >> Reading project info, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");

            DataResponse dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.GetProjectInfos;
            this.ReadProjectInfos();
            if (cachedProjects != null)
            {
                foreach (var item in cachedProjects)
                {
                    if (item.Value<string>("Id") == projectId)
                    {
                        dataResponse.Data = item;
                        Diagnostics.Logger.LogInfo($"[Connection]GetProjectInfo >> Read project info, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");

                        break;
                    }
                }
            }

            return dataResponse;
        }

        private DataResponse DownloadProject(JObject jsonDocument)
        {
            var projectId = jsonDocument.Value<string>("projectId");
            Diagnostics.Logger.LogInfo($"[Connection]DownloadProject >> Reading project info, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");
            DataResponse dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.DownloadProject;
            this.ReadProject(projectId);
            var proj = cachedProjectApps.ContainsKey(projectId) ? cachedProjectApps[projectId] : null;
            ////var content = Convert.ToString(proj);
            ////var stream = new MemoryStream();
            ////var writer = new StreamWriter(stream);
            ////writer.Write(content);
            ////writer.FlushAsync();
            ////stream.Position = 0;
            ////dataResponse.Data = stream;
            dataResponse.Data = proj;
            Diagnostics.Logger.LogInfo($"[Connection]DownloadProject >> Read project configuration, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");
            return dataResponse;
        }

        private DataResponse GetProject(JObject jsonDocument)
        {
            var projectId = jsonDocument.Value<string>("projectId");
            Diagnostics.Logger.LogInfo($"[Connection]GetProject >> Reading project configuration, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");

            DataResponse dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.GetProject;
            this.ReadProject(projectId);
            dataResponse.Data = cachedProjectApps.ContainsKey(projectId) ? cachedProjectApps[projectId] : null;
            Diagnostics.Logger.LogInfo($"[Connection]GetProject >> Read project configuration, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");

            return dataResponse;
        }

        private DataResponse UpdateProject(JObject jsonDocument)
        {
            var projectId = jsonDocument.Value<string>("projectId");
            Diagnostics.Logger.LogInfo($"[Connection]UpdateProject >> Saving project configuration, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");

            DataResponse dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.UpdateProject;
            ////var currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var appPath = Path.Combine(currentPath, AppsStr, projectId);
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            var appFilePath = Path.Combine(appPath, "App.xml");
            var projectPath = Path.Combine(appPath, "project.json");
            var data = jsonDocument["data"].ToString();
            FileProcess fileProcess = new FileProcess();
            if (!File.Exists(appFilePath))
            {
                var appContent = "<App><PlatformModel Path = \"project.json\" /></App>";
                fileProcess.SaveFile(appFilePath, appContent);
                Diagnostics.Logger.LogInfo($"[Connection]UpdateProject >> Saved app.xml configuration, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");
            }

            if (!fileProcess.SaveFile(projectPath, data))
            {
                Diagnostics.Logger.LogWarning($"[Connection]UpdateProject >> Failed to save project configuration, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");

                dataResponse.Status = (int)ResultStatus.FailedToSaveFile;
            }
            else
            {
                cachedProjectApps[projectId] = null;
                Diagnostics.Logger.LogInfo($"[Connection]UpdateProject >> Saved project configuration, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");
            }

            return dataResponse;
        }

        private DataResponse UploadProject(JObject jsonDocument)
        {
            var projectId = jsonDocument.Value<string>("projectId");
            Diagnostics.Logger.LogInfo($"[Connection]UploadProject >> Uploading project configuration, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");
            DataResponse dataResponse = this.UpdateProject(jsonDocument);
            dataResponse.FunctionType = (int)RequestFunction.UploadProject;
            Diagnostics.Logger.LogInfo($"[Connection]UploadProject >> Uploaded project configuration, ConnectionId : {this.connectionId}, ProjectId : {projectId}.");

            return dataResponse;
        }

        private DataResponse UpdateProjectInfos(JObject jsonDocument)
        {
            Diagnostics.Logger.LogInfo($"[Connection]UpdateProjectInfos >> Saving all projects info, ConnectionId : {this.connectionId}.");

            DataResponse dataResponse = new DataResponse();
            dataResponse.FunctionType = (int)RequestFunction.UpdateProjectInfos;
            ////var currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
            lock (projectsInfoLock)
            {
                var projectsPath = Path.Combine(currentPath, "projects.json");
                var data = jsonDocument["data"].ToString();
                FileProcess fileProcess = new FileProcess();
                if (!fileProcess.SaveFile(projectsPath, data))
                {
                    Diagnostics.Logger.LogWarning($"[Connection]UpdateProjectInfos >> Failed to save all projects info, ConnectionId : {this.connectionId}.");

                    dataResponse.Status = (int)ResultStatus.FailedToSaveFile;
                }
                else
                {
                    cachedProjects = null;
                    Diagnostics.Logger.LogInfo($"[Connection]UpdateProjectInfos >> Saved all projects info, ConnectionId : {this.connectionId}.");
                }
            }

            return dataResponse;
        }

        private ResultStatus VerifyRequest(string request, out JObject jsonDocument, out RequestFunction requestFunction)
        {
            jsonDocument = null;
            requestFunction = RequestFunction.Unknown;
            if (string.IsNullOrEmpty(request))
            {
                return ResultStatus.InvalidJson;
            }

            ResultStatus resultStatus = ResultStatus.Ok;
            JObject document = null;
            try
            {
                document = JObject.Parse(request);
            }
            catch (Exception ex)
            {
                Logger.LogError("[Connection] VerifyRequest >> JObject.Parse Exception ", ex);
                return ResultStatus.InvalidJson;
            }

            if (document != null)
            {
                jsonDocument = document;
                string function = "Unknown";
                var found = document["function"] != null && document["function"].Type == JTokenType.String;
                if (!found)
                {
                    return ResultStatus.MissingFunction;
                }
                else
                {
                    function = document.Value<string>("function");
                    if (!Enum.TryParse(function, true, out requestFunction))
                    {
                        return ResultStatus.InvalidFunction;
                    }
                }

                if (requestFunction == RequestFunction.UpdateProjectInfos)
                {
                    var hasData = document["data"] != null && document["data"].Type == JTokenType.Array;
                    if (!hasData)
                    {
                        return ResultStatus.MissingData;
                    }
                }
                else if (requestFunction == RequestFunction.GetProject)
                {
                    var hasData = document["projectId"] != null && document["projectId"].Type == JTokenType.String;
                    if (!hasData)
                    {
                        return ResultStatus.MissingProjectId;
                    }
                }
                else if (requestFunction == RequestFunction.GetProjectInfo)
                {
                    var hasData = document["projectId"] != null && document["projectId"].Type == JTokenType.String;
                    if (!hasData)
                    {
                        return ResultStatus.MissingProjectId;
                    }
                }
                else if (requestFunction == RequestFunction.DeployProject)
                {
                    var hasData = document["projectId"] != null && document["projectId"].Type == JTokenType.String;
                    if (!hasData)
                    {
                        return ResultStatus.MissingProjectId;
                    }
                }
                else if (requestFunction == RequestFunction.UndeployProject)
                {
                    var hasData = document["projectId"] != null && document["projectId"].Type == JTokenType.String;
                    if (!hasData)
                    {
                        return ResultStatus.MissingProjectId;
                    }
                }
                else if (requestFunction == RequestFunction.DownloadProject)
                {
                    var hasData = document["projectId"] != null && document["projectId"].Type == JTokenType.String;
                    if (!hasData)
                    {
                        return ResultStatus.MissingProjectId;
                    }
                }
                else if (requestFunction == RequestFunction.UpdateProject)
                {
                    var hasData = document["projectId"] != null && document["projectId"].Type == JTokenType.String;
                    if (!hasData)
                    {
                        return ResultStatus.MissingProjectId;
                    }

                    hasData = document["data"] != null && document["data"].Type == JTokenType.Object;
                    if (!hasData)
                    {
                        return ResultStatus.MissingData;
                    }
                }
                else if (requestFunction == RequestFunction.UploadProject)
                {
                    var hasData = document["projectId"] != null && document["projectId"].Type == JTokenType.String;
                    if (!hasData)
                    {
                        return ResultStatus.MissingProjectId;
                    }

                    hasData = document["data"] != null && document["data"].Type == JTokenType.Object;
                    if (!hasData)
                    {
                        return ResultStatus.MissingData;
                    }
                }
                else if (requestFunction == RequestFunction.Login)
                {
                    var hasData = document["userName"] != null && document["userName"].Type == JTokenType.String;
                    if (!hasData)
                    {
                        return ResultStatus.MissingUsernamePassword;
                    }

                    hasData = document["password"] != null && document["password"].Type == JTokenType.String;
                    if (!hasData)
                    {
                        return ResultStatus.MissingUsernamePassword;
                    }
                }
            }

            return resultStatus;
        }

        private DataResponse ResponseError(ResultStatus resultStatusType)
        {
            DataResponse dataResponse = null;
            switch (resultStatusType)
            {
                case ResultStatus.InvalidJson:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.InvalidJson,
                        Message = "Invalid json.",
                    };
                    break;
                case ResultStatus.MissingFunction:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.MissingFunction,
                        Message = "Missing function.",
                    };
                    break;
                case ResultStatus.InvalidFunction:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.InvalidFunction,
                        Message = "Invalid function.",
                    };
                    break;
                case ResultStatus.InvalidJsonString:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.InvalidJson,
                        Message = "Invalid json string.",
                    };
                    break;
                case ResultStatus.MissingData:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.InvalidJson,
                        Message = "Missing data or invalid data.",
                    };
                    break;
                case ResultStatus.MissingProjectId:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.MissingProjectId,
                        Message = "Missing project id.",
                    };
                    break;
                case ResultStatus.MissingUsernamePassword:
                    dataResponse = new DataResponse()
                    {
                        Status = (int)ResultStatus.MissingUsernamePassword,
                        Message = "Missing username or password.",
                    };
                    break;
            }

            return dataResponse;
        }
    }
}
