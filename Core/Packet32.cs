using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionController.Core
{
    public class Packet32
    {
        //変数
        public DataType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public object Content { get; set; }

        public Packet32(DataType dataType, object content)
        {
            //タイムスタンプはコンストラクタが呼ばれた時点での日時とする
            DateTime timestamp = DateTime.Now;

            Type = dataType;
            Content = content;
        }

        internal static string Format(DataType dataType, string data)//フォーマット
        {
            return string.Empty;
        }
        
    }
    public class Packet32Serializer
    {
        public PacketReceivedEventHandler PacketReceived;
        public Packet32Serializer(PacketReceivedEventHandler eventHandler)
        {
            PacketReceived += eventHandler;
        }
        /// <summary>
        /// Packet32形式の文字列をデコードします。
        /// </summary>
        /// <param name="data">byte形式の配列</param>
        /// <returns></returns>
        public static Packet32 Decode(byte[] data)
        {
            Packet32 packet = new Packet32(DataType.Nothing, 0);
            switch (data[0])
            {
                case 0x00:
                    packet = new Packet32(
                        DataType.Log,
                        Encoding.ASCII.GetString(data.Skip(1).ToArray()));
                    break;
                case 0x01:
                    packet = new Packet32(
                        DataType.DebugLog,
                        Encoding.ASCII.GetString(data.Skip(1).ToArray()));
                    break;
                case 0x02:
                    packet = new Packet32(
                        DataType.DebugNotification,
                        Encoding.ASCII.GetString(data.Skip(1).ToArray()));
                    break;
                case 0x03:
                    packet = new Packet32(
                        DataType.DebugWarning,
                        Encoding.ASCII.GetString(data.Skip(1).ToArray()));
                    break;
                case 0x04:
                    packet = new Packet32(
                        DataType.DebugError,
                        Encoding.ASCII.GetString(data.Skip(1).ToArray()));
                    break;
                case 0x05:
                    packet = new Packet32(
                        DataType.DebugCriticalError,
                        Encoding.ASCII.GetString(data.Skip(1).ToArray()));
                    break;
                case 0x10:
                    packet = new Packet32(
                        DataType.Command,
                        data.Skip(1).ToArray());
                    break;
                default:
                    break;
            }
            return packet;
        }
        //public static byte[] Encode(DataType dataType, string str)
        //{
        //}
    }
    public delegate void PacketReceivedEventHandler(Packet32 packet);
    public enum DataType
    {
        Log = 0x00, DebugLog = 0x01, DebugNotification = 0x02, DebugWarning = 0x03, DebugError = 0x04, DebugCriticalError = 0x05,
        Command = 0x10,
        Nothing = 0xFF
    }
}
