using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionController.Core
{
    public class Packet32
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }

        public Packet32()
        {
            DateTime timestamp = DateTime.Now;
        }

        internal static void Format(DataPrefix prefix, string data)//フォーマット
        {
        }
        public static Packet32 Format(string data, WirelessModuleType type)//
        {
            switch (type)
            {
                //IM920とIM920SLは文法が同じなので、同一処理でいい
                case WirelessModuleType.IM920:
                    break;

                case WirelessModuleType.IM920SL:
                    //ノード番号は受信文字列の4~7番目なので、該当箇所をbyte型変数に変換する
                    ushort nodeNumber =Convert.ToUInt16($"{data[4]}{data[5]}{data[6]}{data[7]}", 16);
                    //RSSI値は受信文字列の9~10番目なので、該当箇所をbyte型変数に変換して受信電力とする
                    byte signalLevel = Convert.ToByte($"{data[9]}{data[10]}",16);

                    //aa,bb,dddd:00,00,00,00,というIM920SLのデータ形式から、aa,bb,dddd:を消して、16進数配列にする。
                    string userData = data.Split(':').Last();
                    byte[] userData16Array = userData.Split(',').Select(x => Convert.ToByte(x, 16)).ToArray();
                    return userData16Array;

                default:
                    return new byte[0];
            }
        }
        //public void Commander(DataSet data)
        //{
        //    byte h = data.HeadByte[0]; //ヘッダバイトの１バイト目を取得する
        //    switch (h)
        //    {
        //        case 0x00: //ログハンドラを呼び出す
        //            dLogHandler(data);
        //            break;
        //        case 0x01:
        //            dCommandHandler(data);
        //            break;
        //        case 0x02:
        //            dCommandResponceHandler(data);
        //            break;
        //        default:
        //            break;
        //    }
        //}
        internal enum DataPrefix
        {
            Debug = 0x00, Command = 0x01, CommandResponce = 0x02, Telemetry = 0x10, 
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
}
