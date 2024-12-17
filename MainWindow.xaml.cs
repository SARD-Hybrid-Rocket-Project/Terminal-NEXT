using MissionController.Views;
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
using MissionController.Core;

namespace MissionController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //アプリケーションクラスのインスタンス
        private App app;
        //時計用のタイマー
        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.SystemIdle);
        //RSSI値
        public MainWindow()
        {
            InitializeComponent();

            app = (App)App.Current;

            this.Title = Constants.ApplicationName;

            UpdateMainWindow();
        }
        private void UpdateMainWindow()
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

            UpdateClock();//時計を初期化

            if (isSerialPortOpen())
            {
                InputBox_CommandInput.IsEnabled = true;

                UpdateCommandInput();
            }
            else
            {
                InputBox_CommandInput.IsEnabled = false;
            }
            UpdateSignalStrength(0);
        }
        private void UpdateClock()
        {
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (sender, e) =>
            {
                TextBlock_CurrentDate.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            };
            timer.Start();
        }
        private void UpdateCommandInput()
        {

        }
        internal void UpdateSignalStrength(int Strength)
        {
            //後で処理追加
            if(Strength >= Convert.ToByte(255))
            {
                Icon_SignalStrength.Symbol = Wpf.Ui.Controls.SymbolRegular.CellularData120;
            }
            else if(Strength >= Convert.ToByte(100))
            {
                Icon_SignalStrength.Symbol = Wpf.Ui.Controls.SymbolRegular.CellularData220;
            }
            else
            {
                Icon_SignalStrength.Symbol = Wpf.Ui.Controls.SymbolRegular.CellularOff24;
            }
            TextBlock_SignalStrength.Text = $"信号強度 {Strength} dBm";
        }
        private bool isSerialPortOpen()//ポートが開いていたら入力ボックスを有効化する
        {
            var app = (App)App.Current;
            return app.wirelessModule.serialPort.IsOpen;
        }



















        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_NewConnection_Click(object sender, RoutedEventArgs e)
        {
            //serialPortManagement.ConnectにGetSerialPortInformationで取得したシリアルポート情報を渡す
            var app = (App)App.Current;
            app.wirelessModule.Connect(WirelessModule.GetSerialPortInformation());
            UpdateMainWindow();
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
            app.wirelessModule.Disconnect();

            UpdateMainWindow();
        }

        private void InputBox_CommandInput_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Button_InputBox_CommandSend_Click(object sender, RoutedEventArgs e)
        {
        }
        private void SerialCommandSend(string s)
        {
        }
    }
}