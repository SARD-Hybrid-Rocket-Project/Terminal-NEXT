using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace MissionController.Core
{
    //クソコードですまん
    public class Timeline
    {
        //ロガー
        private static readonly ILog log = LogManager.GetLogger("TimelineLogger");
        public FlowDocument TimelineDocument { get; private set; }
        //イベント
        public event EventHandler? TimelineUpdateEvent;

        public Timeline()
        {
            TimelineDocument = new FlowDocument()
            {
                Background = System.Windows.Media.Brushes.Black,
                Foreground = System.Windows.Media.Brushes.White,
                FontFamily = new System.Windows.Media.FontFamily("consolas"),
                LineStackingStrategy = LineStackingStrategy.MaxHeight,
                FontSize = 16,
                PagePadding = new Thickness(0),
                
            };
        }
        public void Debug(string message)
        {
            Logging(Timeline.Level.DEBUG, DateTime.Now, message);
        }
        public void Debug(DateTime date, string message)
        {
            Logging(Timeline.Level.DEBUG, date, message);
        }
        public void Info(string message)
        {
            Logging(Timeline.Level.INFO, DateTime.Now, message);
        }
        public void Info(DateTime date, string message)
        {
            Logging(Timeline.Level.INFO, date, message);
        }
        public void Warn(string message)
        {
            Logging(Timeline.Level.WARN, DateTime.Now, message);
        }
        public void Warn(DateTime date, string message)
        {
            Logging(Timeline.Level.WARN, date, message);
        }
        public void Error(string message)
        {
            Logging(Timeline.Level.ERROR, DateTime.Now, message);
        }
        public void Error(DateTime date, string message)
        {
            Logging(Timeline.Level.ERROR, date, message);
        }
        public void Fatal(string message)
        {
            Logging(Timeline.Level.FATAL, DateTime.Now, message);
        }
        public void Fatal(DateTime date, string message)
        {
            Logging(Timeline.Level.FATAL, date, message);
        }
        public void LineBreak()
        {
            TimelineDocument.Blocks.Add(new Paragraph());
        }
        public enum Level
        {
            DEBUG = 0, INFO = 1, WARN = 2, ERROR = 3, FATAL = 4
        }
        public void Logging(Timeline.Level level, DateTime time ,string message)
        {
            Paragraph paragraph = new Paragraph();

            //ログメッセージをフォーマット
            // 改行コードを検知して、その後に指定した数の空白を挿入
            string spaceString = new string(' ', 19); // 指定した数の空白文字を生成
            message = message.Replace("\n", "\n" + spaceString); // 改行後に空白を挿入
            Run m = new Run(message);

            //時間をフォーマット
            string date = time.ToString("HH:mm:ss:fff");
            Run t = new Run(date + " ");
            //ログの属性によって色分けを設定
            Run logAttribute = new Run(level.ToString().PadRight(5) + " ");
            switch (level)
            {
                case Level.DEBUG:
                    log.Debug(date + " " + message);
                    logAttribute.Foreground = System.Windows.Media.Brushes.LightGreen;
                    break;
                case Level.INFO:
                    log.Info(date + " " + message);
                    logAttribute.Foreground = System.Windows.Media.Brushes.White;
                    break;
                case Level.WARN:
                    log.Warn(date + " " + message);
                    logAttribute.Foreground = System.Windows.Media.Brushes.Orange;
                    break;
                case Level.ERROR:
                    log.Error(date + " " + message);
                    logAttribute.Foreground = System.Windows.Media.Brushes.Red;
                    break;
                case Level.FATAL:
                    log.Fatal(date + " " + message);
                    logAttribute.Foreground = System.Windows.Media.Brushes.IndianRed;
                    break;
                default:
                    break;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(t);
                paragraph.Inlines.Add(logAttribute);
                paragraph.Inlines.Add(m);
                
                paragraph.Margin = new Thickness(0);
                paragraph.TextAlignment = TextAlignment.Left;
                TimelineDocument.Blocks.Add(paragraph);

                TimelineUpdateEvent?.Invoke(this, new EventArgs());
            });
        }
    }
}
