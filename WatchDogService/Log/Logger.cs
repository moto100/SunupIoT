using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunup.WatchDogService
{
    public class Logger
    {
        private string LogEvenSource = "Sunup IO Server Platform";

        public Logger()
        {
            WinEventLog.CreateSystemEventLogCategory(LogEvenSource, "Application");
        }
        public Logger(string logEvenSource)
        {
            this.LogEvenSource = logEvenSource;
            WinEventLog.CreateSystemEventLogCategory(LogEvenSource, "Application");
        }
        public void LogInfo(string message)
        {
            WinEventLog.WriteSystemEventLog(LogEvenSource, message, System.Diagnostics.EventLogEntryType.Information);
        }

        public void LogError(string message)
        {
            WinEventLog.WriteSystemEventLog(LogEvenSource, message, System.Diagnostics.EventLogEntryType.Error);
        }
    }
}
