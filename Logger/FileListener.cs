// <copyright file="FileListener.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Timers;
    using Sunup.Contract;

    /// <summary>
    /// ConsoleListener.
    /// </summary>
    public class FileListener : IListener
    {
        private const int MessageCapacity = 10000;
        private const string LogFileNameFormate = "yyyy_MM_dd__hh_mm_ss";
        private Queue<string> messageQueue = new Queue<string>();
        private StringBuilder buffter;
        private int totalStringLength = 0;
        private string logFile;
        private Timer executionTimer;
        private string logFolderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileListener"/> class.
        /// </summary>
        /// <param name="logFolderPath">logFolderPath.</param>
        public FileListener(string logFolderPath)
        {
            this.logFolderPath = logFolderPath;
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableLogError.
        /// </summary>
        public bool EnableLogError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enableLogWarning.
        /// </summary>
        public bool EnableLogWarning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enableLogInfo.
        /// </summary>
        public bool EnableLogInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enableLogTrace.
        /// </summary>
        public bool EnableLogTrace { get; set; }

        /// <summary>
        /// Start.
        /// </summary>
        public void Start()
        {
            this.StartExecutionTimer();
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            this.StopExecutionTimer();
        }

        /// <summary>
        /// Logs an error message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        public void LogError(Func<string> getMessage, Exception exception = null)
        {
            if (!this.EnableLogError)
            {
                return;
            }

            var message = getMessage();
            var date = DateTime.Now;
            string messageString;
            if (exception != null)
            {
                messageString = date + " >>> [Error] " + message + " : " + exception.Message;
            }
            else
            {
                messageString = date + " >>> [Error] " + message;
            }

            this.Append(messageString);
        }

        /// <summary>
        /// Logs a warning message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        public void LogWarning(Func<string> getMessage, Exception exception = null)
        {
            if (!this.EnableLogWarning)
            {
                return;
            }

            var message = getMessage();
            var date = DateTime.Now;
            string messageString;
            if (exception != null)
            {
                messageString = date + " >>> [Warning] " + message + " : " + exception.Message;
            }
            else
            {
                messageString = date + " >>> [Warning] " + message;
            }

            this.Append(messageString);
        }

        /// <summary>
        /// Logs an info message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        public void LogInfo(Func<string> getMessage)
        {
            if (!this.EnableLogInfo)
            {
                return;
            }

            var message = getMessage();
            var date = DateTime.Now;
            string messageString;
            messageString = date + " >>> [Info] " + message;
            this.Append(messageString);
        }

        /// <summary>
        /// Logs a trace message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        public void LogTrace(Func<string> getMessage, Exception exception = null)
        {
            if (!this.EnableLogTrace)
            {
                return;
            }

            var message = getMessage();
            var date = DateTime.Now;
            string messageString;
            if (exception != null)
            {
                messageString = date + " >>> [Trace] " + message + " : " + exception.StackTrace;
            }
            else
            {
                messageString = date + " >>> [Trace] " + message;
            }

            this.Append(messageString);
        }

        private void Append(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.messageQueue.Enqueue(message + "\r\n");
            }
        }

        private void Execute()
        {
            if (this.messageQueue.Count > 0)
            {
                this.buffter = new StringBuilder();
            }
            else
            {
                return;
            }

            TimeoutCounter tc = new TimeoutCounter(50);
            while (!tc.HasElapsed() && this.totalStringLength < MessageCapacity)
            {
                if (this.messageQueue.Count == 0)
                {
                    break;
                }

                var msg = this.messageQueue.Dequeue();
                if (!string.IsNullOrEmpty(msg))
                {
                    this.buffter.Append(msg);
                    this.totalStringLength++;
                }
            }

            if (string.IsNullOrEmpty(this.logFile))
            {
                this.logFile = Path.Combine(this.logFolderPath, "Log", DateTime.Now.ToString(LogFileNameFormate));
            }

            File.AppendAllText(this.logFile, this.buffter.ToString());
            if (this.totalStringLength == MessageCapacity)
            {
                this.logFile = Path.Combine(this.logFolderPath, "Log", DateTime.Now.ToString(LogFileNameFormate));
                this.totalStringLength = 0;
            }
        }

        private void StartExecutionTimer()
        {
            if (this.executionTimer == null)
            {
                var interval = 100;
                this.executionTimer = new Timer(interval);
                this.executionTimer.Elapsed += (sender, e) =>
                {
                    this.executionTimer.Enabled = false;
                    try
                    {
                        this.Execute();
                    }
                    catch
                    {
                    }

                    this.executionTimer.Enabled = true;
                };

                this.executionTimer.Start();
            }
        }

        private void StopExecutionTimer()
        {
            if (this.executionTimer != null)
            {
                this.executionTimer.Stop();
                this.executionTimer.Dispose();
                this.executionTimer = null;
            }
        }
    }
}
