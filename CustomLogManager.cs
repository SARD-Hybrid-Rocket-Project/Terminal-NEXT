using log4net;
using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace SARD
{
    internal class TimelineAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            // ログメッセージをフォーマット
            string logMessage = this.RenderLoggingEvent(loggingEvent);

            // 独自の出力処理（例: コンソールに色付きで出力）
            Console.ForegroundColor = ConsoleColor.Cyan; // メッセージの色を変更
            Console.WriteLine($"[CustomAppender] {logMessage}");
            Console.ResetColor(); // 色をリセット
        }
    }
}
