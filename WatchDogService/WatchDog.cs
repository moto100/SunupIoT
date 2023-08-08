using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Sunup.WatchDogService
{
    public partial class WatchDog : ServiceBase
    {
        int AvailableCheckInterval = 5000;
        string ClientName = "Sunup.IOServerHost";
        string ClientPath;
        string LogEvenSource = "Sunup IO Server Platform";
        CancellationTokenSource cancelTokenSource;
        Logger Logger;
        public WatchDog()
        {
            InitializeComponent();
            Initial();
            if (string.IsNullOrEmpty(ClientPath))
            {
                var exePath = Assembly.GetEntryAssembly().Location;
                var basePath = Path.GetDirectoryName(exePath);
                ClientPath = basePath + "\\" + ClientName + ".exe";
            }

            Logger = new Logger(LogEvenSource);
           
            //WinEventLog.CreateSystemEventLogCategory(LogEvenSource, "Application");
        }

        protected override void OnStart(string[] args)
        {
            cancelTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(DoWork, cancelTokenSource.Token);
        }

        private void DoWork(object arg)
        {
            while (!cancelTokenSource.IsCancellationRequested)
            {
                try
                {
                    Process process = GetClientProcess();
                    if (process == null || process.HasExited)
                    {
                        process = StartClient();
                        if (process == null)
                        {
                            //WinEventLog.WriteSystemEventLog(LogEvenSource, "Failed to start client.", EventLogEntryType.Information);
                            Logger.LogInfo("Failed to start service.");
                        }
                        else
                        {
                            //WinEventLog.WriteSystemEventLog(LogEvenSource, "Succeed to start client.", EventLogEntryType.Information);
                            Logger.LogInfo("Succeed to start service.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // WinEventLog.WriteSystemEventLog(LogEvenSource, "Failed to run client.\r\n" + ex.Message+ "\r\n" +ex.StackTrace, EventLogEntryType.Error);
                    Logger.LogError("Failed to run service.\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                }
                // report to gateway
                Thread.Sleep(AvailableCheckInterval);
            }
        }

        protected override void OnStop()
        {
            try
            {
                Process process = GetClientProcess();
                if (process != null && !process.HasExited)
                {
                    process.Kill();
                    Logger.LogInfo("Succeed to close service.");
                    //WinEventLog.WriteSystemEventLog(LogEvenSource, "Succeed to start client.", EventLogEntryType.Information);
                }
            }
            catch(Exception ex)
            {
                //WinEventLog.WriteSystemEventLog(LogEvenSource, "Failed to close client.", EventLogEntryType.Error);
                Logger.LogError("Failed to close service." + ex.StackTrace);
            }
            if (cancelTokenSource != null)
            {
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
            }
        }

        private Process StartClient()
        {
            var appPath = ClientPath;

            var procInfo = new ProcessStartInfo()
            {
                Arguments = @"",
                FileName = appPath,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };
            Process proc = Process.Start(procInfo);
            if (proc == null)
            {
                //WinEventLog.WriteSystemEventLog(LogEvenSource, "[Service] Failed to run client.\r\n appPath is " + appPath, EventLogEntryType.Information);
                Logger.LogInfo("[Service] Failed to run service.\r\n appPath is " + appPath);
                return null;
            }

            return proc;
        }

        private Process GetClientProcess()
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

        private void Initial()
        {
            ClientPath = System.Configuration.ConfigurationSettings.AppSettings["ClientPath"];
            ClientName = System.Configuration.ConfigurationSettings.AppSettings["ClientName"];
            LogEvenSource = System.Configuration.ConfigurationSettings.AppSettings["LogEvenSource"];
            var interval = System.Configuration.ConfigurationSettings.AppSettings["AvailableCheckInterval"];
            int.TryParse(interval, out AvailableCheckInterval);
        }
    }
}
