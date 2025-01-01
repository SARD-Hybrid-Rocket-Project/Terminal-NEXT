using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionController.Core
{
    public class WirelessModuleData
    {
        //変数
        public ushort NodeNumber { get; set; } = 0xFFFF;
        public byte RSSI { get; set; } = 0x00;
        public byte Type { get; set; }
        public byte[] Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        //Packet32のコンストラクタ
        public WirelessModuleData(byte dataType, byte[] content)
        {
            Type = dataType;
            Data = content;
        }
        public WirelessModuleData(ushort nodeNumber, byte rssi, byte type, byte[] data)
        {
            NodeNumber = nodeNumber;
            RSSI = rssi;
            Type = type;
            Data = data;
        }

        internal static string Format(DataType dataType, string data)//フォーマット
        {
            return string.Empty;
        }
        
    }
    public class SendData
    {
        public ushort Node { get; set; } = 0xFFFF;
        public string Data { get; set; }
        public SendData(ushort nodeNumber, string data)
        {
            Node = nodeNumber;
            Data = data;
        }
    }
    public static class Packet32Serializer
    {
        static string WMEditCommand = "WMEdit";
        /// <summary>
        /// Packet32形式をシリアライズします。
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static SendData Serialize(string command)
        {
            ushort target = 0xFFFF;
            string commandData = "";
            //コマンドを分解
            var splitedCommand = command
                .Split(' ')
                .Select(value => value.Trim())
                .ToArray();

            //無線モジュール設定コマンドの場合は、別処理
            if (splitedCommand[0] == WMEditCommand)
            {
                Debug.WriteLine(WMEditCommand);
                //ターゲットを自身に設定
                target = 0xFFFF;
                //残りのコマンドをつなげる
                for (int i = 1; i < splitedCommand.Length; i++)
                {
                    commandData += splitedCommand[i] + " ";
                }
            }
            else if (Enum.TryParse(typeof(DataType), splitedCommand[0], true, out var parsedType))
            {
                target = Convert.ToUInt16(splitedCommand[1], 16);
                var type = (DataType)parsedType;

                commandData = $"{(byte)type:X2},";
                for (int i = 2; i < splitedCommand.Length; i++)
                {
                    commandData += splitedCommand[i] + " ";
                }
            }

            return new SendData(target, commandData);
        }
        /// <summary>
        /// 文字列をPacket32形式にデコードします。
        /// </summary>
        /// <param name="data">byte形式の配列</param>
        /// <returns></returns>
        public static WirelessModuleData Deserialize(string data)
        {
            //データの分解
            //ノード番号である、文字列の4文字目から4文字取得
            ushort node = Convert.ToUInt16(data.Substring(3, 4), 16);
            //RSSI値を取得
            byte rssi = Convert.ToByte(data.Substring(8, 2), 16);
            //DataTypeを取得
            byte type = Convert.ToByte(data.Substring(11, 2), 16);

            //ユーザーデータを取得
            byte[] userData = data.Substring(14)
                .Trim()
                .Split(',')
                .Select(hex => Convert.ToByte(hex, 16))
                .ToArray();
            //Packet32を生成して返す
            return new WirelessModuleData(node, rssi, type, userData);
        }
    }
    public enum DataType
    {
        DEBUG = 0, INFO = 1, WARN = 2, ERROR = 3, FATAL = 4,
        COMMAND = 16,
        NOTHING = 255
    }
}
