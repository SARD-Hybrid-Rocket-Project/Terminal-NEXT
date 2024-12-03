using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using MissionController.Core;
using MissionController.Views;

namespace MissionController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal MainWindow mainWindow;//メインウィンドウ
        //無線接続関連
        internal SerialPortManagement serialPortManagement { get; private set; }
        //環境変数・プロファイルのクラス
        public ApplicationProfile Profile { get; private set; }
        public EnvironmentConfiguration Config { get; private set; }
        
        public App()
        {
            //設定ファイル読み込み
            Config = EnvironmentConfiguration.ReadConfiguration();

            //アプリのインスタンス情報を新規作成
            Profile = new ApplicationProfile();

            //シリアル通信用のシリアルポートインスタンスを宣言
            serialPortManagement = new SerialPortManagement();
            serialPortManagement.dataReceivedHandler += SerialDataReceived;//シリアルポート受信時のイベント

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            mainWindow = new MainWindow();
            mainWindow.Show();
        }
        private void SerialDataReceived(Object sender,SerialDataReceivedEventArgs e)//シリアルポート受診時の処理
        {
            try
            {
                string receivedData = serialPortManagement.GetSerialPort().ReadExisting();
                Debug.WriteLine($"Received: {receivedData}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mainWindow.RichTextBox_Log.AppendText(receivedData);
                    mainWindow.RichTextBox_Log.ScrollToEnd();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Received: {ex}");
                throw;
            }
        }
    }

}
