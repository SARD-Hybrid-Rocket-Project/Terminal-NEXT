using FlightController.Views;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using FlightController.Core;

namespace FlightController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private App app;


        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.SystemIdle);
        private SerialPort _serialPort = new SerialPort();
        public MainWindow()
        {
            InitializeComponent();

            app = (App)App.Current;

            this.Title = Constants.ApplicationName;

            MainWindowInitialize();
        }
        private void MainWindowInitialize()
        {
            if (isSerialPortOpen())
            {
                Button_Disconnect.IsEnabled = true;
                Button_InputBox_CommandSend.IsEnabled = true;
            }
            else
            {
                Button_Disconnect.IsEnabled = false;
                Button_InputBox_CommandSend.IsEnabled = false;
            }

            _InitializeClock();//時計を初期化

            if (isSerialPortOpen())
            {
                InputBox_CommandInput.IsEnabled = true;

                _IinitializeCommandInput();
            }
            else
            {
                InputBox_CommandInput.IsEnabled = false;
            }
        }
        private void _InitializeClock()
        {
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (sender, e) =>
            {
                TextBlock_CurrentDate.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            };
            timer.Start();
        }
        private void _InitializeVisualLog()
        {
        }
        private void _IinitializeCommandInput()
        {

        }
        private bool isSerialPortOpen()//ポートが開いていたら入力ボックスを有効化する
        {
            var app = (App)App.Current;
            return app.serialPortManagement.GetSerialPort().IsOpen;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_NewConnection_Click(object sender, RoutedEventArgs e)
        {
            //serialPortManagement.ConnectにGetSerialPortInformationで取得したシリアルポート情報を渡す
            var app = (App)App.Current;
            app.serialPortManagement.Connect(SerialPortManagement.GetSerialPortInformation());

            MainWindowInitialize();
        }

        private void Button_EnvironmentalSetting_Click(object sender, RoutedEventArgs e)
        {
            EnvironmentSettingsDialog dialog = new EnvironmentSettingsDialog(app.Config);
            dialog.Owner = this;
            bool? result = dialog.ShowDialog();
        }

        private void Button_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            var app = (App)App.Current;
            app.serialPortManagement.Disconnect();

            MainWindowInitialize();
        }

        private void InputBox_CommandInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SerialCommandSend(InputBox_CommandInput.Text);
            }
        }

        private void Button_InputBox_CommandSend_Click(object sender, RoutedEventArgs e)
        {
            SerialCommandSend(InputBox_CommandInput.Text);
        }
        private void SerialCommandSend(string s)
        {
            app.serialPortManagement.GetSerialPort().WriteLine(s);
        }
    }
}