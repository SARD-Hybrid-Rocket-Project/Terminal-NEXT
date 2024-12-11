using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionController.Core
{
    public class VisualLog
    {
        public delegate void LoggingEventHandler(LogData data);
        public LoggingEventHandler? LogAddEvent;
        public LoggingEventHandler? LogClearEvent;
        List<LogData> logList;
        public VisualLog()
        {
            LogAddEvent = null;
            logList = new List<LogData>();
        }

        public void Add(DateTime date, LogType type, string content)
        {
            var log = new LogData(date, type, content);
            logList.Add(log);
            if (LogAddEvent != null) LogAddEvent(log);
        }
        public void Clear()
        {
            logList.Clear();
            //if (LogClearEvent != null) LogClearEvent(log);
        }

    }
    public class LogData
    {
        public DateTime Time { get; set; }
        public LogType Type { get; set; }
        public string Content { get; set; }
        public LogData(DateTime time, LogType type, string content)
        {
            Time = time;
            Type = type;
            Content = content;
        }
    }
    public enum LogType
    {
        Log,
        DebugLog,
        DebugNotification,
        Error,
        Warning,
        Information
    }
}
