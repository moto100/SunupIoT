using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sunup.WatchDogService
{
    /// <summary>
    /// 系统日志
    /// </summary>
    public class WinEventLog
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        private static string ErrorInfo { get; set; }
        /// <summary>
        /// 创建系统事件日志分类
        /// </summary>
        /// <param name="eventSourceName">注册事件源(比如说这个日志来源于某一个应用程序)</param>
        /// <param name="logName">日志名称(事件列表显示的名称)</param>
        /// <returns></returns>
        public static bool CreateSystemEventLogCategory(string eventSourceName, string logName)
        {
            bool createResult = false;
            try
            {
                if (!EventLog.SourceExists(eventSourceName))
                {
                    EventLog.CreateEventSource(eventSourceName, logName);
                }
                createResult = true;
            }
            catch (Exception ex)
            {
                createResult = false;
                ErrorInfo = ex.Message;
            }
            return createResult;
        }
        /// <summary>
        /// 删除系统事件日志分类
        /// </summary>
        /// <param name="eventSource">EventName事件源</param>
        /// <returns></returns>
        public static bool RemoveSystemEventSourceCategory(string eventSource)
        {
            bool createResult = false;
            try
            {
                if (EventLog.SourceExists(eventSource))
                {
                    EventLog.DeleteEventSource(eventSource, ".");
                }
                createResult = true;
            }
            catch (Exception ex)
            {
                createResult = false;
                ErrorInfo = ex.Message;
            }
            return createResult;
        }
        /// <summary>
        /// 向系统日志中写入日志
        /// </summary>
        /// <param name="eventSource">事件源</param>
        /// <param name="msg">写入日志信息</param>
        /// <param name="type">日志文本分类(警告、信息、错误)</param>
        /// <returns></returns>
        public static bool WriteSystemEventLog(string eventSource, string msg, EventLogEntryType type)
        {
            bool writeResult = false;
            try
            {
                if (!EventLog.SourceExists(eventSource))
                {
                    writeResult = false;
                    ErrorInfo = "日志分类不存在!";
                }
                else
                {
                    EventLog.WriteEntry(eventSource, msg, type);
                    writeResult = true;
                }
            }
            catch (Exception ex)
            {
                writeResult = false;
                ErrorInfo = ex.Message;
            }
            return writeResult;
        }
        /// <summary>
        /// 删除事件源中logName(好像删除了所有的该分类的日志)
        /// </summary>
        /// <param name="eventSource"></param>
        /// <param name="logName"></param>
        /// <returns></returns>
        public static bool RemoveSystemEventLog(string eventSource, string logName)
        {
            bool removeResult = false;
            try
            {
                if (!EventLog.SourceExists(eventSource))
                {
                    removeResult = false;
                    ErrorInfo = "日志分类不存在!";
                }
                else
                {
                    EventLog.Delete(logName);
                    removeResult = true;
                }
            }
            catch (Exception ex)
            {
                removeResult = false;
                ErrorInfo = ex.Message;
            }
            return removeResult;
        }
        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public static string GetErrorMessage()
        {
            return ErrorInfo;
        }
    }
}

