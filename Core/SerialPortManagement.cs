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
    internal class SerialPortManagement
    {
        public SerialPort serialPort;
        public SerialPortManagement()
        {
            serialPort = new SerialPort();
        }
        public SerialPortManagement(SerialPort port)
        {
            serialPort = port;
        }

        public SerialPort GetSerialPort() {  return serialPort; }

        public static SerialPortInformation GetSerialPortInformation()
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
        public void Connect(SerialPortInformation serialPortInformation)
        {
            if (serialPortInformation.PortName == string.Empty)
            {
                MessageBox.Show("ポートを指定してください！");
                return;
            };
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
                MessageBox.Show("接続しました", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show($"Failed to connect port {e.Message}");
            }
            catch (IOException e)
            {

            }
            catch (ArgumentException e)
            {
                MessageBox.Show($"Failed to connect port {e.Message}");
            }
            catch (Exception e)
            {

            }
        }
    }
    public class SerialPortInformation
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
