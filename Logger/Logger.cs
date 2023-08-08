// <copyright file="Logger.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Log infor.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Gets or sets a value indicating whether EnableLogError.
        /// </summary>
        public static bool EnableLogError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enableLogWarning.
        /// </summary>
        public static bool EnableLogWarning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enableLogInfo.
        /// </summary>
        public static bool EnableLogInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enableLogTrace.
        /// </summary>
        public static bool EnableLogTrace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enableLogTrace.
        /// </summary>
        public static ILogger MSLogger { get; set; }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        public static void LogError(string message, Exception exception = null)
        {
            if (MSLogger != null)
            {
                MSLogger.LogError(exception, message);
            }
            else
            {
                LogError(() => message, exception);
            }
        }

        /// <summary>
        /// Logs an warning message.
        /// </summary>
        /// <param name="message">a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        public static void LogWarning(string message, Exception exception = null)
        {
            if (MSLogger != null)
            {
                MSLogger.LogWarning(exception, message);
            }
            else
            {
                LogWarning(() => message, exception);
            }
        }

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        public static void LogInfo(string message)
        {
            if (MSLogger != null)
            {
                MSLogger.LogInformation(message);
            }
            else
            {
                LogInfo(() => message);
            }
        }

        /// <summary>
        /// Logs an trace message.
        /// </summary>
        /// <param name="message">a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        public static void LogTrace(string message, Exception exception = null)
        {
            if (MSLogger != null)
            {
                MSLogger.LogTrace(exception, message);
            }
            else
            {
                LogTrace(() => message, exception);
            }
        }

        /// <summary>
        /// Logs an error message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        public static void LogError(Func<string> getMessage, Exception exception = null)
        {
            if (!EnableLogError)
            {
                return;
            }

            try
            {
                var listeners = ListenerManager.Instance.Listeners.Values;
                foreach (var listener in listeners)
                {
                    if (listener != null)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            listener.LogError(getMessage, exception);
                        });
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Logs a warning message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        public static void LogWarning(Func<string> getMessage, Exception exception = null)
        {
            if (!EnableLogWarning)
            {
                return;
            }

            try
            {
                var listeners = ListenerManager.Instance.Listeners.Values;
                foreach (var listener in listeners)
                {
                    if (listener != null)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            listener.LogWarning(getMessage, exception);
                        });
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Logs an info message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        public static void LogInfo(Func<string> getMessage)
        {
            if (!EnableLogInfo)
            {
                return;
            }

            try
            {
                var listeners = ListenerManager.Instance.Listeners.Values;
                foreach (var listener in listeners)
                {
                    if (listener != null)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            listener.LogInfo(getMessage);
                        });
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Logs a trace message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        public static void LogTrace(Func<string> getMessage, Exception exception = null)
        {
            if (!EnableLogTrace)
            {
                return;
            }

            try
            {
                var listeners = ListenerManager.Instance.Listeners.Values;
                foreach (var listener in listeners)
                {
                    if (listener != null)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            listener.LogTrace(getMessage, exception);
                        });
                    }
                }
            }
            catch
            {
            }
        }
    }
}
