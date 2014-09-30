using System;
using log4net.Appender;
using log4net.Core;

namespace Mechavian.NodeService.UI.ViewModels
{
    internal class NodeServiceLogAppender : AppenderSkeleton
    {
        public event Action<NodeServiceLogAppender, LoggingEvent> LogAppended;

        protected override void Append(LoggingEvent loggingEvent)
        {
            var handler = LogAppended;
            if (handler != null)
            {
                handler(this, loggingEvent);
            }
        }
    }
}