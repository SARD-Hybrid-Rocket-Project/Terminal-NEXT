using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionController.Core
{
    public class Command
    {
        //デリゲートの定義
        public delegate void LogHandler(DataSet e);
        public delegate void CommandHandler(DataSet e);
        public delegate void CommandResponceHandler(DataSet e);

        //デリゲートの宣言
        public required LogHandler dLogHandler;
        public required CommandHandler dCommandHandler;
        public required CommandResponceHandler dCommandResponceHandler;

        public static byte[] Format(string data, WirelessModuleType type)//
        {
            switch (type)
            {
                //IM920とIM920SLは文法が同じなので、同一処理でいい
                case WirelessModuleType.IM920:
                case WirelessModuleType.IM920SL:
                    //aa,bb,dddd:00,00,00,00,というIM920SLのデータ形式から、aa,bb,dddd:を消して、16進数配列にする。
                    string userData = data.Split(',').Last();
                    byte[] userData16Array = userData.Split(',').Select(x => Convert.ToByte(x, 16)).ToArray();
                    return userData16Array;
                default:
                    return new byte[0];
            }
        }
        public void Commander(DataSet data)
        {
            byte h = data.HeadByte[0]; //ヘッダバイトの１バイト目を取得する
            switch (h)
            {
                case 0x00: //ログハンドラを呼び出す
                    dLogHandler(data);
                    break;
                case 0x01:
                    dCommandHandler(data);
                    break;
                case 0x02:
                    dCommandResponceHandler(data);
                    break;
                default:
                    break;
            }
        }
    }
    public class DataSet
    {
        public Byte[] HeadByte;
        public Byte[] DataValue;
        public DataSet()
        {
            Byte[] data = new Byte[0];
            Byte[] DataValue = new byte[0];
        }
    }
    public class SerialPortLog
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public SerialPortLog(DateTime date, string Message)
        {
            Timestamp = DateTime.Now;

        }
    }
}
