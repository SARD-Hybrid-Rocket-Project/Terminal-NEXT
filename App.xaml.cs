using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using FlightController.Core;
using FlightController.Views;

namespace FlightController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //無線接続関連
        internal SerialPortManagement serialPortManagement { get; private set; }
        //環境変数・プロファイルのクラス
        public ApplicationProfile Profile { get; private set; }
        public EnvironmentConfiguration Config { get; private set; }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
        public App()
        {

            Config = EnvironmentConfiguration.ReadConfiguration();

            Profile = new ApplicationProfile(); //新しいApplicationProfileインスタンスを作成

            serialPortManagement = new SerialPortManagement();
            serialPortManagement.GetSerialPort().DataReceived += SerialDataReceived;


        }
        private void SerialDataReceived(Object sender,SerialDataReceivedEventArgs e)//シリアルポート受診時の処理
        {
            try
            {
                string receivedData = serialPortManagement.GetSerialPort().ReadExisting();
                Debug.WriteLine($"Received: {receivedData}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Received: {ex}");
                throw;
            }
        }
    }

}
