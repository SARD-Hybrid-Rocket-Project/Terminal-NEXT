﻿using System.Configuration;
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
        private string receivedDataBuffer = string.Empty;

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

            //ワイヤレスモジュールの制御クラスをインスタンス化
            wirelessModule = new WirelessModule();
            wirelessModule.DataReceivedEventHandler += SerialDataReceived;//シリアルポート受信時のイベント
            log.Debug("WirelessModuleクラスのインスタンスwirelessModuleを初期化");

            //謎
            PacketHandler = new Packet32Serializer(PacketReceivedEvent);


            //コンストラクタの最後でMainWindowを初期化する。表示はしない。
            mainWindow = new MainWindow();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            mainWindow?.Show();
        }
        private void SerialDataReceived(Object sender,SerialDataReceivedEventArgs e)//シリアルポート受信時のイベント
        {
            try
            {
                //受け取ったデータをバッファ用変数に追加する。
                receivedDataBuffer += wirelessModule.serialPort.ReadExisting();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Received: {ex}");
                throw;
            }
            
            //改行コード（CRLF）が見つかったらデータを出力する
            if (receivedDataBuffer.Contains("\r\n"))//データはaa,bb,dddd:の形式で送られてくる
            {
                //IM920の固有コマンドレスポンスのチェック
                if (receivedDataBuffer.Contains("OK") || receivedDataBuffer.Contains("NG"))
                {   
                    return;
                }

                //データの分解
                string nodeNumber = receivedDataBuffer.Substring(4,4);//ノード番号である、文字列の4文字目から4文字取得
                int rssi = int.Parse(receivedDataBuffer.Substring(9, 2), System.Globalization.NumberStyles.HexNumber);//RSSI値を取得
                byte[] userData = receivedDataBuffer//ユーザーデータを取得
                    .Substring(11)
                    .Split(',')
                    .Select(hex => Convert.ToByte(hex, 16))
                    .ToArray();
                var packet = Packet32Serializer.Decode(userData);//デコード

                switch (packet.Type)
                {
                    case DataType.Log:

                        break;
                    case DataType.DebugLog:
                        break;
                    case DataType.DebugNotification:
                        break;
                    case DataType.DebugWarning:
                        break;
                    case DataType.DebugError:
                        break;
                    case DataType.DebugCriticalError:
                        break;
                    case DataType.Command:
                        break;
                    case DataType.Nothing:
                        break;
                    default:
                        break;
                }

                //一時的措置
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mainWindow?.RichTextBox_Log.AppendText(receivedDataBuffer);
                    mainWindow?.RichTextBox_Log.ScrollToEnd();
                });

                //バッファをクリア
                receivedDataBuffer = string.Empty;
            }
        }
        private void PacketReceivedEvent(Packet32 packet)
        {
            switch (packet.Type)
            {
                case DataType.Log:
                    Debug.WriteLine($"DebugLog: {packet.Content}");
                    break;
                case DataType.DebugLog:
                    Debug.WriteLine($"DebugNotification: {packet.Content}");
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
