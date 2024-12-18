using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionController.Core
{
    public class Packet32
    {
        //変数
        public ushort NodeNumber { get; set; } = 0xFFFF;
        public byte RSSI { get; set; } = 0x00;
        public DataType Type { get; set; }
        public byte[] Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        //Packet32のコンストラクタ
        public Packet32(DataType dataType, byte[] content)
        {
            Type = dataType;
            Data = content;
        }
        public Packet32(ushort nodeNumber, byte rssi, DataType dataType, byte[] data)
        {
            NodeNumber = nodeNumber;
            RSSI = rssi;
            Type = dataType;
            Data = data;
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
        /// Packet32形式をシリアライズします。
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static string Serialize(Packet32 packet)
        {
            //packet.Typeを16進数に変換して、Contentをカンマ区切りの文字列に変換して返す
            return ((byte)packet.Type).ToString("X2") + BitConverter.ToString((byte[])packet.Data).Replace("-", ",");
        }
        /// <summary>
        /// 文字列をPacket32形式にデコードします。
        /// </summary>
        /// <param name="data">byte形式の配列</param>
        /// <returns></returns>
        public static Packet32 Deserialize(string data)
        {
            //データの分解
            //ノード番号である、文字列の4文字目から4文字取得
            ushort node = Convert.ToUInt16(data.Substring(3, 4), 16);
            //RSSI値を取得
            byte rssi = Convert.ToByte(data.Substring(8, 2), 16);
            //ユーザーデータを取得
            byte[] userData = data
                .Substring(11)
                .Split(',')
                .Select(hex => hex.Trim())
                .Select(hex => Convert.ToByte(hex, 16))
                .ToArray();

            //デフォルトはDataType.Nothing
            DataType type = DataType.NOTHING;
            //data[0]がDataTypeに含まれているか確認し、含まれていればtypeにキャストする
            if (Enum.IsDefined(typeof(DataType), (int)userData[0])) type = (DataType)(int)userData[0];
            //Packet32を生成して返す
            return new Packet32(node, rssi, type, userData.Skip(1).ToArray());
        }
    }
    public delegate void PacketReceivedEventHandler(Packet32 packet);
    public enum DataType
    {
        DEBUG = 0x00, INFO = 0x01, WARN = 0x02, ERROR = 0x03, FATAL = 0x04,
        COMMAND = 0x10,
        NOTHING = 0xFF
    }
}
