using System;
using System.Windows.Media;
using log4net.Core;

namespace Mechavian.NodeService.UI.ViewModels
{
    internal class LoggingEventViewModel
    {
        public LoggingEventViewModel(LoggingEvent loggingEvent)
        {
            Message = loggingEvent.RenderedMessage;
            Exception = loggingEvent.GetExceptionString();
            Timestamp = loggingEvent.TimeStamp;
            LevelText = loggingEvent.Level.DisplayName;
            LevelColor = GetLoggingColor(loggingEvent.Level);
        }

        private Color GetLoggingColor(Level level)
        {
            switch (level.Name)
            {
                case "EMERGENCY":
                case "FATAL":
                case "ALERT":
                case "CRITICAL":
                case "SEVERE":
                case "ERROR":
                    return Colors.Red;
                case "WARN":
                    return Colors.Yellow;
                case "NOTICE":
                case "INFO":
                    return Colors.LightBlue;
                case "DEBUG":
                case "FINE":
                case "log4net:DEBUG":
                    return Colors.Orange;
                case "TRACE":
                case "FINER":
                    return Colors.Green;
                case "VERBOSE":
                case "FINEST":
                    return Colors.LightGreen;
                default:
                    return Colors.LightGray;
            }
        }

        public Color LevelColor { get; private set; }
        public string LevelText { get; private set; }
        public string Message { get; private set; }
        public string Exception { get; private set; }
        public DateTime Timestamp { get; private set; }
    }
}