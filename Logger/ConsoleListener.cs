// <copyright file="ConsoleListener.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Diagnostics
{
    using System;

    /// <summary>
    /// ConsoleListener.
    /// </summary>
    public class ConsoleListener : IListener
    {
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
            if (exception != null)
            {
                Console.WriteLine(date + " >>> [Error] " + message + " : " + exception.Message);
            }
            else
            {
                Console.WriteLine(date + " >>> [Error] " + message);
            }
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
            if (exception != null)
            {
                Console.WriteLine(date + " >>> [Warning] " + message + " : " + exception.Message);
            }
            else
            {
                Console.WriteLine(date + " >>> [Warning] " + message);
            }
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
            Console.WriteLine(date + " >>> [Info] " + message);
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
            if (exception != null)
            {
                Console.WriteLine(date + " >>> [Trace] " + message + " : " + exception.StackTrace);
            }
            else
            {
                Console.WriteLine(date + " >>> [Trace] " + message);
            }
        }

        /// <summary>
        /// Start.
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
        }
    }
}
