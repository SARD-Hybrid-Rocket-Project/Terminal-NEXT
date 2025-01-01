using System.Collections;
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
using SARD.Core;
using SARD.Resources;
using SARD.Views;

namespace SARD
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
        internal IM920SL wirelessModule { get; private set; }

        //環境変数のクラス・プロパティ
        public EnvironmentConfiguration Config { get; private set; }
        public bool IsShowHexValue { get; set; } = true; //受信データの生の値を表示するかどうか
        public bool IsLocalEcho { get; set; } = true; //ローカルエコーを有効にするかどうか

        public App()
        {
            //log4netの初期化
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
            log.Debug("Initialized log4net");
            //Timelineのイベントを設定
            timeline.TimelineUpdateEvent += (sender, e) => mainWindow?.RichTextBox_Timeline.ScrollToEnd();

            //環境設定ファイル読み込み
            try
            {
                timeline.Debug("環境ファイルを読み込んでいます...");
                Config = EnvironmentConfiguration.ReadConfiguration();
                timeline.Debug("環境ファイルを正常に読み込めました");
            }
            catch (Exception e)
            {
                timeline.Error("環境ファイルの読み込み時にエラーが発生しました\n" + e.ToString());
                throw;
            }

            timeline.Debug("インスタンスを初期化しています...");
            //ワイヤレスモジュールの制御クラスをインスタンス化し、イベントを設定
            wirelessModule = new IM920SL();
            wirelessModule.DataReceivedEvent += DataReceivedEventHandler;
            log.Debug("WirelessModuleクラスのインスタンスwirelessModuleを初期化");
            //コンストラクタの最後でMainWindowを初期化する。表示はしない。
            mainWindow = new MainWindow();

            timeline.Debug("初期化が完了しました");
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            mainWindow?.Show();
            timeline.Info($"{Resource.ApplicationName}へようこそ！");
        }



        //メソッドとか
        /// <summary>
        /// シリアルポートに接続するメソッド
        /// </summary>
        internal void SerialPortConnect()
        {
            PortSelectionDialog dialog = new PortSelectionDialog() { Owner = mainWindow ?? null };
            bool? result = dialog.ShowDialog();
            Debug.WriteLine(result);

            if(result == true)
            {
                timeline.Debug("以下のポートに接続を試みます\n" +
                    $"ポート名：{dialog.ComboBox_PortList.SelectedValue?.ToString()}\n" +
                    $"ボーレート：{Convert.ToInt32(dialog.ComboBox_Baudlate.SelectedValue).ToString()}");
                try
                {
                    wirelessModule.Open(
                        dialog.ComboBox_PortList.SelectedValue?.ToString() ?? string.Empty,
                        Convert.ToInt32(dialog.ComboBox_Baudlate.SelectedValue));
                    timeline.Info("接続しました");
                }
                catch (Exception e)
                {
                    timeline.Error("接続に失敗しました\n" + e.ToString());
                }
            }
            else timeline.Debug("接続をキャンセルしました");
        }
        /// <summary>
        /// シリアルポートから切断するメソッド
        /// </summary>
        internal void SerialPortDisconnect()
        {
            try
            {
                wirelessModule.Close();
                timeline.Info("シリアルポートを切断しました");
            }
            catch (Exception e)
            {
                timeline.Debug("シリアルポートの切断に失敗しました\n" + e.ToString());
                throw;
            }
        }
        /// <summary>
        /// データを送信するメソッド
        /// </summary>
        /// <param name="mode">送信モード</param>
        /// <param name="commannd"></param>
        public void Send(string commannd)
        {
            timeline.Info(commannd);
            //受け取ったdataの形式は
            //[コマンド] [ターゲット] [値１] [値２]
            //[コマンド] [値2] [値1]
            //の形式であることを想定している
            //Serialize後のデータアレイの中身は
            //[0] ターゲットノード（16進数4桁）
            //[1] データ（16進数2桁コンマ「,」区切り）
            //[2] データ（[1]の中身をテキスト情報にしたもの）
            //の予定
            try
            {
                var data = Packet32Serializer.Serialize(commannd);
                wirelessModule.Send(data.Node, data.Data);
                //ローカルエコーが有効な場合はデータを表示
                if (IsLocalEcho) timeline.Debug($"データを送信しました\nターゲット：{data.Node}\nデータ：{data.Data}");
            }
            catch (Exception e) { timeline.Error("送信中にエラーが発生しました" + e.ToString()); }
        }

        //イベントハンドラ
        /// <summary>
        /// シリアルポート受信時のイベント
        /// </summary>
        private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Debug.WriteLine("データを受信しました");
            var r = ((SerialPort)sender).ReadExisting();
            r.Split("\r\n");
            Debug.WriteLine(r);

            ////IsShowHexValueがtrueの場合は受信データをそのまま表示
            //if (IsShowHexValue) Application.Current.Dispatcher.Invoke(() => timeline.Debug($"受信データ：{r.Trim()}"));

            if (r.StartsWith("00,"))
            {
                //受信データの場合
                WirelessModuleData packet = Packet32Serializer.Deserialize(r);

                //Typeが0～4の範囲内の場合はログとして表示
                if (0 <= packet.Type && packet.Type <= 4)
                {
                    var message = System.Text.Encoding.ASCII.GetString(packet.Data);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        mainWindow?.UpdateSignalStrength(packet.RSSI);
                        timeline.Logging((Timeline.Level)packet.Type, packet.Timestamp, message);
                    });
                }
                //Typeが5の場合はエンコードデータとして表示
                //else if (packet.Type == 5)
                //    Application.Current.Dispatcher.Invoke(() => timeline.Debug($"エンコードデータ：{packet.Data}"));
            }
            //受信データではない場合はコマンドレスポンスとして扱う
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    timeline.Debug($"コマンドレスポンス：{r.Trim()}");
                });
            }
        }
    }

}
