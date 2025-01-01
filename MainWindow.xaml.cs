using SARD.Views;
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
using SARD.Core;
using System.Diagnostics;

namespace SARD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //アプリケーションクラスのインスタンス
        private App app = (App)App.Current;
        //時計用のタイマー
        private Timer timer;
        public MainWindow()
        {
            InitializeComponent();


            RichTextBox_Timeline.Document = app.timeline.TimelineDocument;

            //ウィンドウのタイトルを設定
            this.Title = SARD.Resources.Resource.ApplicationName;
            //時計の更新
            timer = new Timer((state) =>
            {
                string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                Application.Current.Dispatcher.Invoke(() => { TextBlock_CurrentDate.Text = currentTime; });
            },
            null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));

            RefreshWindow();
        }
        protected override void OnClosed(EventArgs e)
        {
            //タイマーの破棄
            timer.Dispose();
            //参照を切る
            RichTextBox_Timeline.Document = new FlowDocument();
            base.OnClosed(e);
        }

        private void UpdateCommandInput()
        {

        }
        internal void UpdateSignalStrength(byte rssi)
        {
            if (rssi >= Convert.ToByte(255))
            {
                Icon_SignalStrength.Symbol = Wpf.Ui.Controls.SymbolRegular.CellularData120;
            }
            else if (rssi >= Convert.ToByte(100))
            {
                Icon_SignalStrength.Symbol = Wpf.Ui.Controls.SymbolRegular.CellularData220;
            }
            else
            {
                Icon_SignalStrength.Symbol = Wpf.Ui.Controls.SymbolRegular.CellularOff24;
            }
            TextBlock_SignalStrength.Text = $"信号強度 {rssi} dBm";
        }


        /// <summary>
        /// ウィンドウの更新
        /// </summary>
        private void RefreshWindow()
        {
            if (app.wirelessModule.Port.IsOpen)
            {
                Button_NewConnection.IsEnabled = false;
                Button_Disconnect.IsEnabled = true;

                AutoSuggestBox_Command.IsEnabled = true;
                Button_InputBox_CommandSend.IsEnabled = true;
            }
            else
            {
                Button_NewConnection.IsEnabled = true;
                Button_Disconnect.IsEnabled = false;

                AutoSuggestBox_Command.IsEnabled = false;
                Button_InputBox_CommandSend.IsEnabled = false;
            }
        }
        /// <summary>
        /// ログ表示
        /// </summary>
        /// <param name="paragraph"></param>
        












        //ボタンのクリックイベント
        private void E_PortConnect(object sender, RoutedEventArgs e)
        {
            
            app.SerialPortConnect();
            RefreshWindow();
        }
        private void E_PortDisConnect(object sender, RoutedEventArgs e)
        {
            app.SerialPortDisconnect();
            RefreshWindow();
        }
        private void E_Send(object sender, RoutedEventArgs e)
        {
            if(AutoSuggestBox_Command.Text != string.Empty) app.Send(AutoSuggestBox_Command.Text);
            AutoSuggestBox_Command.Text = string.Empty;
        }
        private void E_AboutApplication(object sender, RoutedEventArgs e) { new About { Owner = this }.ShowDialog(); }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("未実装です。ごめんなさい");
        }

        private void Button_EnvironmentalSetting_Click(object sender, RoutedEventArgs e)
        {
            EnvironmentSettingsDialog dialog = new EnvironmentSettingsDialog(app.Config);
            dialog.Owner = this;
            bool? result = dialog.ShowDialog();
        }

        

        private void InputBox_CommandInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) E_Send(sender, e);
        }

        private void Button_InputBox_CommandSend_Click(object sender, RoutedEventArgs e)
        {
        }
        private void SerialCommandSend(string s)
        {
        }
    }
}