using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MissionController.Core.Logging
{
    internal class FlowDocumentManager
    {
        internal FlowDocument FlowDocument { get; private set; } = new FlowDocument()
        {
            Background = Brushes.Black,
            Foreground = Brushes.White
        };
        internal FlowDocumentManager()
        {
        }
        internal void AddLog(Packet32 packet)
        {
            Paragraph paragraph = new Paragraph();
            //ログの属性によって色分けを設定
            Run logAttribute = new Run(" " + packet.Type.ToString().PadRight(10) + " ");
            switch (packet.Type)
            {
                case DataType.DEBUG:
                    logAttribute.Foreground = Brushes.Black;
                    logAttribute.Background = Brushes.White;
                    break;
                case DataType.INFO:
                    logAttribute.Background = Brushes.Green;
                    break;
                case DataType.WARN:
                    logAttribute.Background = Brushes.Orange;
                    break;
                case DataType.ERROR:
                    logAttribute.Background = Brushes.Red;
                    break;
                case DataType.FATAL:
                    logAttribute.Background = Brushes.IndianRed;
                    break;
                default:
                    break;
            }
            Run time = new Run(packet.Timestamp.ToString(" HH:mm:ss:fff "));//時間
            Run content = new Run(packet.Data.ToString());//内容
            paragraph.Inlines.Add(logAttribute);
            paragraph.Inlines.Add(time);
            paragraph.Inlines.Add(content);
            FlowDocument.Blocks.Add(paragraph);
        }
        internal void AppendText(string text)
        {
            Paragraph paragraph = new Paragraph(new Run(text));
            FlowDocument.Blocks.Add(paragraph);
        }
    }
}
