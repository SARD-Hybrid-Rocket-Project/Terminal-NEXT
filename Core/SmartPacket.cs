using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionController.Core
{
    public class SmartPacket
    {
        //変数
        public ushort NodeNumber { get; set; } = 0xFFFF;
        public byte RSSI { get; set; } = 0x00;
        public DataType Type { get; set; }
        public byte[] Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        //Packet32のコンストラクタ
        public SmartPacket(DataType dataType, byte[] content)
        {
            Type = dataType;
            Data = content;
        }
        public SmartPacket(ushort nodeNumber, byte rssi, DataType dataType, byte[] data)
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
    public static class Packet32Serializer
    {
        /// <summary>
        /// Packet32形式をシリアライズします。
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static string Serialize(SmartPacket packet)
        {
            //packet.Typeを16進数に変換して、Contentをカンマ区切りの文字列に変換して返す
            return ((byte)packet.Type).ToString("X2") + BitConverter.ToString((byte[])packet.Data).Replace("-", ",");
        }
        /// <summary>
        /// 文字列をPacket32形式にデコードします。
        /// </summary>
        /// <param name="data">byte形式の配列</param>
        /// <returns></returns>
        public static SmartPacket Deserialize(string data)
        {
            //データの分解
            //ノード番号である、文字列の4文字目から4文字取得
            ushort node = Convert.ToUInt16(data.Substring(3, 4), 16);
            //RSSI値を取得
            byte rssi = Convert.ToByte(data.Substring(8, 2), 16);
            //DataTypeを取得
            byte type = Convert.ToByte(data.Substring(11, 2), 16);
            if (!Enum.IsDefined(typeof(DataType), (int)type)) type = 255;//DataTypeが定義されていない場合は255に設定

            //ユーザーデータを取得
            byte[] userData = data
                .Substring(13)
                .Split(',')
                .Select(hex => hex.Trim())
                .Select(hex => Convert.ToByte(hex, 16))
                .ToArray();
            //Packet32を生成して返す
            return new SmartPacket(node, rssi, (DataType)type, userData);
        }
    }
    public enum DataType
    {
        DEBUG = 0, INFO = 1, WARN = 2, ERROR = 3, FATAL = 4,
        COMMAND = 16,
        NOTHING = 255
    }
}
