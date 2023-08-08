// <copyright file="ProcessHelper.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.ControlPanel
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Sunup.Diagnostics;

    /// <summary>
    /// Config.
    /// </summary>
    public static class ProcessHelper
    {
        private static readonly string ClientName = "Sunup.IOServerHost";
        private static readonly string ClientPath;

        /// <summary>
        /// Initializes static members of the <see cref="ProcessHelper"/> class.
        /// </summary>
        static ProcessHelper()
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            var basePath = Path.GetDirectoryName(exePath);
            ClientPath = Path.Combine(basePath, ClientName + ".exe");
        }

        /// <summary>
        /// StartClient.
        /// </summary>
        /// <param name="args">args.</param>
        /// <returns>true.</returns>
        public static int StartProcess(string args)
        {
            try
            {
                var appPath = ClientPath;
                var procInfo = new ProcessStartInfo()
                {
                    Arguments = args,
                    FileName = appPath,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                };
                Process proc = Process.Start(procInfo);
                proc.EnableRaisingEvents = true;
                proc.OutputDataReceived += (sender, data) =>
                {
                    Logger.LogInfo("[ProcessHelper] After start process, OutputDataReceived " + data.Data);
                };

                proc.StartInfo.RedirectStandardError = true;

                proc.ErrorDataReceived += (sender, data) =>
                {
                    Logger.LogInfo("[ProcessHelper] After start process, ErrorDataReceived " + data.Data);
                };

                if (proc == null)
                {
                    Logger.LogInfo("[ProcessHelper] Failed to run app.\r\n app args is " + args);
                    return -1;
                }
                else
                {
                    Logger.LogInfo("[ProcessHelper] Succeed to run app.\r\n app args is " + args);

                    return proc.Id;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[ProcessHelper] Failed to run app.\r\n app args is " + args, ex);
                return -1;
            }
        }

        /// <summary>
        /// StartClient.
        /// </summary>
        /// <param name="args">args.</param>
        /// <returns>true.</returns>
        public static Process GetProcess(string args)
        {
            try
            {
                Process proc = null;
                var procs = Process.GetProcessesByName(ClientName);
                if (procs.Length > 0)
                {
                    // do nothing if the process is running
                    return procs[0];
                }

                return proc;
            }
            catch (Exception ex)
            {
                Logger.LogError("[ProcessHelper] Failed to get the running app, args is " + args, ex);
                return null;
            }
        }

        /// <summary>
        /// StartClient.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>true.</returns>
        public static Process GetProcess(int id)
        {
            try
            {
                var proc = Process.GetProcessById(id);
                return proc;
            }
            catch (Exception ex)
            {
                Logger.LogError("[ProcessHelper] Failed to get the running app, process id is " + id, ex);
                return null;
            }
        }

        /// <summary>
        /// StartClient.
        /// </summary>
        public static void KillProcess()
        {
            try
            {
                var procs = Process.GetProcessesByName(ClientName);
                foreach (var process in procs)
                {
                    var id = process.Id;
                    if (!process.HasExited)
                    {
                        process.EnableRaisingEvents = true;
                        process.Exited += (object sender, EventArgs e) =>
                        {
                            Logger.LogInfo("[ProcessHelper] Succeed to stop the running app, process id is " + id);
                        };
                        process.Kill();
                        Logger.LogInfo("[ProcessHelper] Stopping the running app, process id is " + id);
                    }
                    else
                    {
                        Logger.LogInfo("[ProcessHelper] There is not an App running or is closing, process id is " + id);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[ProcessHelper] Failed to stop the running apps.", ex);
            }
        }

        /// <summary>
        /// StartClient.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>true.</returns>
        public static bool KillProcess(int id)
        {
            Process process = GetProcess(id);
            if (process != null && !process.HasExited)
            {
                try
                {
                    process.EnableRaisingEvents = true;
                    process.Exited += (object sender, EventArgs e) =>
                    {
                        Logger.LogInfo("[ProcessHelper] Succeed to stop the running app, process id is " + id);
                    };
                    process.Kill();
                    Logger.LogInfo("[ProcessHelper] Stopping the running app, process id is " + id);

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError("[ProcessHelper] Failed to stop the running app, process id is " + id, ex);
                    return false;
                }
            }
            else
            {
                Logger.LogInfo("[ProcessHelper] There is not an App running or is closing, process id is " + id);
                return true;
            }
        }

        /// <summary>
        /// StartClient.
        /// </summary>
        /// <param name="args">args.</param>
        /// <returns>true.</returns>
        public static bool KillProcess(string args)
        {
            args = ClientName;
            Process process = GetProcess(ClientName);
            if (process != null && !process.HasExited)
            {
                try
                {
                    process.EnableRaisingEvents = true;
                    process.Exited += (object sender, EventArgs e) =>
                    {
                        Logger.LogInfo("[ProcessHelper] Succeed to stop the running app, args is " + args);
                    };
                    process.Kill();
                    Logger.LogInfo("[ProcessHelper] Stopping the running app, args is " + args);

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError("[ProcessHelper] Failed to stop the running app, args is " + args, ex);
                    return false;
                }
            }
            else
            {
                Logger.LogInfo("[ProcessHelper] There is not an App running or is closing, args is " + args);
                return true;
            }
        }

        private static void Process_Exited(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
