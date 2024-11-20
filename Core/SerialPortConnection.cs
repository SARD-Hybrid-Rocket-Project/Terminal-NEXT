using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlightController
{
    static class SerialPortConnection
    {
        public static SerialPortInformation GetSerialPortInformation()
        {
            PortSelectionDialog dialog = new PortSelectionDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                SerialPortInformation serialPortInformation = new SerialPortInformation(
                    dialog.ComboBox_PortList.SelectedValue?.ToString() ?? string.Empty,
                    Convert.ToInt32(dialog.ComboBox_PortList.SelectedItem));

                return serialPortInformation;
            }
            return new SerialPortInformation(string.Empty, 0);
        }
        public static SerialPort SerialPortConnect(SerialPortInformation serialPortInformation)//シリアルポート接続用クラス
        {
            try
            {
                SerialPort port = new SerialPort(
                serialPortInformation.PortName,
                serialPortInformation.BaudLate,
                serialPortInformation.Parity,
                serialPortInformation.DataBits,
                serialPortInformation.StopBits
                );

                port.Open();//ポートを開く
                MessageBox.Show("接続しました", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);

                return port;
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show($"Failed to connect port {e.Message}");
            }
            catch(IOException e)
            {

            }
            catch (ArgumentException e)
            {

            }
            catch(Exception e)
            {

            }

            return null;
        }
    }
    class SerialPortInformation
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
