using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MissionController.Core
{
    public enum WirelessModuleType
    {
        IM920, IM920SL, Xbee//IM920とXbeeは実装未定
    }
    internal class WirelessModule
    {
        //ロガー
        private static readonly ILog log = LogManager.GetLogger(typeof(WirelessModule));
        //変数
        internal bool IsSending { get; private set; }
        internal SerialPort serialPort { get; private set; }
        internal SerialDataReceivedEventHandler DataReceivedEventHandler;
        internal WirelessModuleType wirelessModuleType = WirelessModuleType.IM920SL;//要修正

        internal enum SendType
        {
            Command = 0, Broadcast = 1, 
        }
        internal WirelessModule()
        {
            IsSending = false;
            serialPort = new SerialPort();
        }

        internal static SerialPortInformation GetSerialPortInformation()//シリアルポート情報を取得するメソッド
        {
            PortSelectionDialog dialog = new PortSelectionDialog();
            bool? result = dialog.ShowDialog();
            Debug.WriteLine(result);

            if (result == true)
            {
                SerialPortInformation serialPortInformation = new SerialPortInformation(
                    dialog.ComboBox_PortList.SelectedValue?.ToString() ?? string.Empty,
                    Convert.ToInt32(dialog.ComboBox_Baudlate.SelectedValue));
                Debug.WriteLine($"取得したポート情報\n{dialog.ComboBox_PortList.SelectedValue?.ToString()}\n{dialog.ComboBox_PortList.SelectedItem}");
                return serialPortInformation;
            }
            return new SerialPortInformation(string.Empty, 0);
        }
        internal bool Connect(SerialPortInformation serialPortInformation)//指定されたシリアルポートに接続するメソッド
        {
            if (serialPortInformation.PortName == string.Empty || serialPortInformation.BaudLate == 0) return false;//ポート名が空だったら返す

            try
            {
                serialPort = new SerialPort()
                {
                    PortName = serialPortInformation.PortName,
                    BaudRate = serialPortInformation.BaudLate,
                    DataBits = serialPortInformation.DataBits,
                    Parity = serialPortInformation.Parity,
                    StopBits = serialPortInformation.StopBits,
                    Handshake = serialPortInformation.Handshake,
                };
                serialPort.DataReceived += DataReceivedEventHandler;


                serialPort.Open();//ポートを開く

                MessageBox.Show($"{serialPortInformation.PortName}に接続しました", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to connect port {e.Message}");
                return false;
            }
            return true;
        }
        public void Disconnect()
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
        public async Task<bool> Send(SendType type, byte[] data)//データを送信するメソッド
        {
            if (!serialPort.IsOpen || IsSending) return false;

            return await Task.Run(() =>
            {
                switch (type)
                {
                    case SendType.Command:
                        try
                        {
                            //無線モジュール用のコマンド
                            IsSending = true;
                            serialPort.WriteLine(BitConverter.ToString(data));
                            IsSending = false;

                            //OKorNGを受信するまで待機
                            if (serialPort.ReadLine().Contains("OK"))
                            {
                                log.Info("送信成功");
                            }
                            else if (serialPort.ReadLine().Contains("NG"))
                            {
                                log.Info("送信失敗");
                            }
                            else
                            {
                                log.Info(serialPort.ReadLine());
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        break;
                    case SendType.Broadcast:
                        //IM920SLのブロードキャスト送信形式"TXDA[data]"にする
                        //WriteLineを使って<CR><LF>付きで送信する
                        try
                        {
                            //送信中フラグを立てる
                            IsSending = true;
                            serialPort.WriteLine("TXDA" + BitConverter.ToString(data).Replace("-", ","));

                            //OKorNGを受信するまで待機
                            if(serialPort.ReadLine().Contains("OK"))
                            {
                                log.Info("送信成功");
                            }
                            else if(serialPort.ReadLine().Contains("OK"))
                            {
                                log.Info("送信失敗");
                            }
                            //送信中フラグを下げる
                            IsSending = false;
                        }
                        catch (TimeoutException e)
                        {
                            log.Debug($"Timeout: {e}");
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        finally
                        {
                            IsSending = false;
                        }
                        break;
                    default:
                        break;
                }
                return true;
            });
        }

    }
    public struct SerialPortInformation
    {
        public string PortName;
        public int BaudLate;

        public byte DataBits = 8;
        public Parity Parity = Parity.None;
        public StopBits StopBits = StopBits.One;
        public Handshake Handshake = Handshake.None;

        public SerialPortInformation(string portName, int baudLate)
        {
            this.PortName = portName;
            BaudLate = baudLate;
        }
    }
}
