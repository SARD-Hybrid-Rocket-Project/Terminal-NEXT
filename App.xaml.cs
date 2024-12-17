using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using log4net;
using MissionController.Core;
using MissionController.Views;

namespace MissionController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //ロガー
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        //メインウィンドウ
        internal MainWindow? mainWindow;
        //無線接続関連
        internal WirelessModule wirelessModule { get; private set; }
        public ushort NodeNumber { get; private set; }
        private int _rssi;
        public int RSSI
        {
            get { return _rssi; }
            set
            {
                _rssi = value;
                mainWindow?.UpdateSignalStrength(value);
            }
        }

        //環境変数・プロファイルのクラス
        public ApplicationProfile Profile { get; private set; }
        public EnvironmentConfiguration Config { get; private set; }
        public Packet32Serializer PacketHandler { get; private set; }

        public App()
        {
            //log4netの初期化
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
            log.Info("Initialized log4net");

            //環境設定ファイル読み込み
            Config = EnvironmentConfiguration.ReadConfiguration();
            log.Info("Read configuration file");

            //アプリのインスタンス情報を新規作成
            Profile = new ApplicationProfile();
            log.Debug("Created application profile");

            //ワイヤレスモジュールの制御クラスをインスタンス化し、イベントを設定
            wirelessModule = new WirelessModule()
            {
                DataReceivedEvent = WModuleDataReceived,
                CommandResponceEvent = CommandResponceEventHandler
            };
            log.Debug("WirelessModuleクラスのインスタンスwirelessModuleを初期化");

            //コンストラクタの最後でMainWindowを初期化する。表示はしない。
            mainWindow = new MainWindow();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            mainWindow?.Show();
        }

        //メソッドとか
        public void Send(Packet32 packet)
        {
            Packet32Serializer.Serialize(packet);
        }








        /// <summary>
        /// シリアルポート受信時のイベント
        /// </summary>
        /// <param name="sender"></param>
        private void WModuleDataReceived(string data)//シリアルポート受信時のイベント
        {
            Packet32 packet = Packet32Serializer.Deserialize(data);
        }
        /// <summary>
        /// コマンドレスポンスのイベントハンドラ
        /// </summary>
        /// <param name="responce"></param>
        private void CommandResponceEventHandler(string responce)
        {
        }
        private void PacketReceivedEvent(Packet32 packet)
        {
            switch (packet.Type)
            {
                case DataType.Log:
                    Debug.WriteLine($"DebugLog: {packet.Data}");
                    break;
                case DataType.DebugLog:
                    Debug.WriteLine($"DebugNotification: {packet.Data}");
                    break;
                case DataType.Nothing:
                    break;
            }
            //受信したパケットの処理
        }
        //internal void LogAddEvent(LogData data)
        //{
        //    //ログ追加時の処理
        //    Paragraph log = new Paragraph();//ログの段落
        //    log.FontFamily = new FontFamily("Consolas");

        //    Run logAttribute = new Run(" " + data.Type.ToString() + " ");
        //    Run time = new Run(data.Time.ToString(" HH:mm:ss:fff "));//時間
        //    switch (data.Type)
        //    {
        //        case LogType.LOG:
        //            logAttribute.Background = Brushes.White;
        //            break;
        //        case LogType.DEBUG:
        //            logAttribute.Background = Brushes.Green;
        //            break;
        //    }
        //    log.Inlines.Add(logAttribute);
        //    log.Inlines.Add(time);
        //    log.Inlines.Add(data.Content);
        //    mainWindow.TextBox_DebugLog.Document.Blocks.Add(log);
        //    mainWindow.TextBox_DebugLog.ScrollToEnd();
        //}
    }

}
