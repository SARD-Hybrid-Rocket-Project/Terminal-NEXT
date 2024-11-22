using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlightController.Core
{
    internal class SerialPortManagement
    {
        private SerialPort serialPort;

        internal WirelessModuleType wirelessModuleType = WirelessModuleType.IM920SL;

        internal enum WirelessModuleType
        {
            IM920, IM920SL, Xbee//IM920とXbeeは実装未定
        }
        public SerialPortManagement()
        {
            serialPort = new SerialPort();
        }

        internal SerialPort GetSerialPort() {  return serialPort; }

        public static SerialPortInformation GetSerialPortInformation() //シリアルポート
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
        public void Connect(SerialPortInformation serialPortInformation)//指定されたシリアルポートに接続するメソッド
        {
            if (serialPortInformation.PortName == string.Empty || serialPortInformation.BaudLate == 0) return;//ポート名が空だったら返す

            try
            {
                serialPort = new SerialPort(
                serialPortInformation.PortName,
                serialPortInformation.BaudLate,
                serialPortInformation.Parity,
                serialPortInformation.DataBits,
                serialPortInformation.StopBits
                );

                serialPort.Open();//ポートを開く
                MessageBox.Show($"{serialPortInformation.PortName}に接続しました", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to connect port {e.Message}");
            }
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
    }
    public struct SerialPortInformation
    {
        public string PortName;
        public int BaudLate;

        public byte DataBits = 8;
        public Parity Parity = Parity.None;
        public StopBits StopBits = StopBits.One;
        Handshake Handshake = Handshake.None;

        public SerialPortInformation(string portName, int baudLate)
        {
            this.PortName = portName;
            BaudLate = baudLate;
        }
    }
}
