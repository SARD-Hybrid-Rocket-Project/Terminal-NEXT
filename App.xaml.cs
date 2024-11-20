using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using FlightController.Core;

namespace FlightController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ApplicationProfile Profile { get; private set; }
        public EnvironmentConfiguration Config { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
        public App()
        {
            Config = EnvironmentConfiguration.ReadConfiguration()

            Profile = new ApplicationProfile(); //新しいApplicationProfileインスタンスを作成
        }
    }

}
