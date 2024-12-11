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
    internal class SerialPortManagement
    {
        //変数
        internal bool IsSending { get; private set; }
        internal SerialPort SerialPort { get; private set; }
        internal SerialDataReceivedEventHandler DataReceivedEventHandler;
        internal WirelessModuleType wirelessModuleType = WirelessModuleType.IM920SL;//要修正

        internal enum SendType
        {
            Command = 0, Broadcast = 1, 
        }
        internal SerialPortManagement()
        {
            IsSending = false;
            SerialPort = new SerialPort();
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
                SerialPort = new SerialPort()
                {
                    PortName = serialPortInformation.PortName,
                    BaudRate = serialPortInformation.BaudLate,
                    DataBits = serialPortInformation.DataBits,
                    Parity = serialPortInformation.Parity,
                    StopBits = serialPortInformation.StopBits,
                    Handshake = serialPortInformation.Handshake,
                };
                SerialPort.DataReceived += DataReceivedEventHandler;


                SerialPort.Open();//ポートを開く

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
            if (SerialPort.IsOpen)
            {
                try
                {
                    SerialPort.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Failed to disconnect port {e.Message}");
                }
            }
        }
        public void Send(SendType type, string data)//データを送信するメソッド
        {
            string command = string.Empty;
            switch (type)
            {
                case SendType.Command:
                    command = data;
                    break;
                case SendType.Broadcast:
                    command = "TXDA " + data;
                    break;
            }
            if (SerialPort.IsOpen && IsSending == false)
            {
                IsSending = true;
                try
                {
                    SerialPort.Write(command);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Failed to send data {e.Message}");
                }
            }
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
