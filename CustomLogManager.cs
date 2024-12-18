using log4net;
using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace MissionController
{
    internal class CustomLogManager : AppenderSkeleton
    {
        private Paragraph[] paragraphs = new Paragraph[0];
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            if (loggingEvent == null) return;

            Paragraph p = new Paragraph();
            p.FontFamily = new FontFamily("Consolas");

            Run logAttribute = new Run(" " + (loggingEvent.Level?.ToString() ?? "Unknown") + " ");
            Run time = new Run(loggingEvent.TimeStamp.ToString(" HH:mm:ss:fff "));//時間
            switch (loggingEvent.Level?.Name)
            {
                case "LOG":
                    logAttribute.Background = Brushes.White;
                    break;
                case "DEBUG":
                    logAttribute.Background = Brushes.Green;
                    break;
            }
            p.Inlines.Add(logAttribute);
            p.Inlines.Add(time);
            p.Inlines.Add(new Run(loggingEvent.RenderedMessage));
            string logMessage = RenderLoggingEvent(loggingEvent);
        }
    }
}
