using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using log4net;
using MissionController.Core;
using MissionController.Resources;
using MissionController.Views;
using static MissionController.Core.WirelessModuleManager;

namespace MissionController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //ロガー
        private static readonly ILog log = LogManager.GetLogger("TimelineLogger");
        internal Timeline timeline { get; private set; } = new Timeline();

        //UI関連
        internal MainWindow? mainWindow;
        //無線接続関連
        internal WirelessModuleManager wirelessModule { get; private set; }
        internal IM920SL im920sl { get; private set; }

        //環境変数・プロファイルのクラス
        public EnvironmentConfiguration Config { get; private set; }

        public App()
        {
            //log4netの初期化
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
            log.Debug("Initialized log4net");

            //環境設定ファイル読み込み
            Config = EnvironmentConfiguration.ReadConfiguration();
            log.Info("Read configuration file");
            timeline.Debug("Read configuration file");

            //ワイヤレスモジュールの制御クラスをインスタンス化し、イベントを設定
            wirelessModule = new WirelessModuleManager()
            {
                //DataReceivedEvent = WirelessModuleDataReceived,
                //CommandResponceEvent = CommandResponceEventHandler
            };
            im920sl = new IM920SL();
            im920sl.DataReceivedEvent += DataReceivedEventHandler;
            log.Debug("WirelessModuleクラスのインスタンスwirelessModuleを初期化");


            //コンストラクタの最後でMainWindowを初期化する。表示はしない。
            mainWindow = new MainWindow();

            timeline.Info($"{Resource.ApplicationName}へようこそ");
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            mainWindow?.Show();
        }

        

        //メソッドとか
        public void Connect()
        {
            PortSelectionDialog dialog = new PortSelectionDialog() { Owner = mainWindow ?? null };
            bool? result = dialog.ShowDialog();
            Debug.WriteLine(result);

            if (result == true)
            {
                SerialPortSettings serialPortInformation = new SerialPortSettings(
                    dialog.ComboBox_PortList.SelectedValue?.ToString() ?? string.Empty,
                    Convert.ToInt32(dialog.ComboBox_Baudlate.SelectedValue));
                timeline.Info($"以下のポートに接続を試みます\n{dialog.ComboBox_PortList.SelectedValue?.ToString()}\n{dialog.ComboBox_PortList.SelectedItem}");

                try
                {
                    wirelessModule.Connect(serialPortInformation);
                    timeline.Info("接続しました");
                }
                catch (Exception e)
                {
                    timeline.Error($"接続に失敗しました\n{e.ToString()}");
                }
            }
        }
        public void Send(SmartPacket packet)
        {
            Packet32Serializer.Serialize(packet);
        }

        //イベントハンドラ
        /// <summary>
        /// シリアルポート受信時のイベント
        /// </summary>
        private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var r = ((SerialPort)sender).ReadExisting();
            if (r.StartsWith("00,"))
            {
                SmartPacket packet = Packet32Serializer.Deserialize(r);
            }
            //受信データではない場合はコマンドレスポンスとして扱う
            else
            {
            }
        }
    }

}
