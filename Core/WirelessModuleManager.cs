using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MissionController.Core
{
    public delegate void WMCommandResponceEventHandler(string data);
    public enum WirelessModuleType
    {
        IM920, IM920SL, Xbee//IM920とXbeeは実装未定
    }
    internal class WirelessModuleManager
    {
        //ロガー
        private static readonly ILog log = LogManager.GetLogger(typeof(WirelessModuleManager));
        //変数
        internal bool IsSending { get; private set; }
        internal byte RSSI { get; private set; }
        internal SerialPort serialPort { get; private set; }

        //イベント
        internal WirelessModuleType wirelessModuleType = WirelessModuleType.IM920SL;//要修正
        //バッファ
        internal string[] ReceivedDataBuffer { get; private set; } = new string[0];

        internal enum SendType
        {
            Command = 0, Broadcast = 1, Unicast = 2
        }
        internal WirelessModuleManager()
        {
            IsSending = false;
            serialPort = new SerialPort();
        }
        /// <summary>
        /// 指定されたシリアルポートに接続するメソッド
        /// </summary>
        /// <param name="portSettings"></param>
        /// <returns></returns>
        internal bool Connect(SerialPortSettings portSettings)//指定されたシリアルポートに接続するメソッド
        {
            //ポート名が空もしくはボーレートが0の場合は接続しない
            if (portSettings.PortName == string.Empty || portSettings.BaudRate == 0) return false;

            try
            {
                serialPort = new SerialPort()
                {
                    PortName = portSettings.PortName,
                    BaudRate = portSettings.BaudRate,
                    DataBits = portSettings.DataBits,
                    Parity = portSettings.Parity,
                    StopBits = portSettings.StopBits,
                    Handshake = portSettings.Handshake,
                };
                serialPort.DataReceived += DataReceivedEventHandler;


                serialPort.Open();//ポートを開く

                log.Info($"Connected to {portSettings.PortName}");
            }
            catch (Exception e)
            {
                throw;
            }
            return true;
        }
        /// <summary>
        /// シリアルポートを切断するメソッド
        /// </summary>
        internal void Disconnect()
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Failed to disconnect port {e.Message}");
                }
            }
        }
        /// <summary>
        /// IM920SLの設定コマンドを送信するメソッド
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <returns></returns>
        public async Task SendCommand(string command)//コマンドを送信するメソッド
        {
            if (!serialPort.IsOpen || IsSending) return;
            await Task.Run(() =>
            {
                IsSending = true;
                try
                {
                    serialPort.WriteLine(command);
                }
                catch (Exception e)
                {
                    log.Error($"Failed to send command: {e}");
                }
                IsSending = false;
            });
        }
        /// <summary>
        /// IM920SLのブロードキャスト（全員に送信する）モードで無線送信するメソッド
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendBroadCast(byte[] data)
        {
            if (!serialPort.IsOpen || IsSending) return;
            await Task.Run(() =>
            {
                IsSending = true;
                try
                {
                    serialPort.WriteLine($"TXDA{BitConverter.ToString(data).Replace("-", ",")}");
                }
                catch (Exception e)
                {
                    log.Error($"Failed to send command: {e}");
                }
                IsSending = false;
            });
        }
        /// <summary>
        /// IM920SLのユニキャスト（相手を指定する）モードで無線送信するメソッド
        /// </summary>
        /// <param name="targetNode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendUniCast(ushort targetNode, byte[] data)
        {
            if (!serialPort.IsOpen || IsSending) return;
            await Task.Run(() =>
            {
                IsSending = true;
                try
                {
                    //TXDU [相手ノード番号],[data]<CR><LF>
                    serialPort.WriteLine($"TXDU {targetNode:X4},{BitConverter.ToString(data).Replace("-", ",")}");
                }
                catch (Exception e)
                {
                    log.Error($"Failed to send command: {e}");
                }
                IsSending = false;
            });
        }

        /// <summary>
        /// シリアルポートのデータ受信イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //受信データを読み取る
            var r = serialPort.ReadExisting();
            //受信データを改行で分割してバッファに格納

            //IM920SLの場合、受信データはaa,bbbb,dd:userDataもしくは、コマンドレスポンスのOK,NG、設定読み取りコマンド時の値が返ってくる。
            //そこで、受信データがaa,bbbb,dd:で始まる場合はデータ受信イベントを発火したい
            //処理負荷を軽減するため、aa,bbbb,dd:のカンマとコロンの位置で判定する。
            if (r.StartsWith("00,"))
            {
                SmartPacket packet = Packet32Serializer.Deserialize(r);
                RSSI = packet.RSSI;
                //アビオニクスからのデータ受信イベントを発火
                //DataReceivedEvent?.Invoke(packet);
            }
            else
            {
                //コマンドレスポンスの場合
            }
        }

        internal static SerialPortSettings GetSerialPortInformation()//シリアルポート情報を取得するメソッド
        {
            PortSelectionDialog dialog = new PortSelectionDialog();
            bool? result = dialog.ShowDialog();
            Debug.WriteLine(result);

            if (result == true)
            {
                SerialPortSettings serialPortInformation = new SerialPortSettings(
                    dialog.ComboBox_PortList.SelectedValue?.ToString() ?? string.Empty,
                    Convert.ToInt32(dialog.ComboBox_Baudlate.SelectedValue));
                Debug.WriteLine($"取得したポート情報\n{dialog.ComboBox_PortList.SelectedValue?.ToString()}\n{dialog.ComboBox_PortList.SelectedItem}");
                return serialPortInformation;
            }
            return new SerialPortSettings(string.Empty, 0);
        }

        public struct SerialPortSettings
        {
            public string PortName;
            public int BaudRate;

            public byte DataBits = 8;
            public Parity Parity = Parity.None;
            public StopBits StopBits = StopBits.One;
            public Handshake Handshake = Handshake.None;

            public SerialPortSettings(string portName, int baudRate)
            {
                this.PortName = portName;
                BaudRate = baudRate;
            }
        }
    }
}
