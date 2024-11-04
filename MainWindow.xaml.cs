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

namespace Terminal_XP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.SystemIdle);
        private SerialPort _serialPort = new SerialPort();
        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Terminal-XP" + " " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?. ToString() ?? string.Empty;
            InitializeClock();
        }
        private void InitializeClock()
        {
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (sender, e) =>
            {
                TextBlock_CurrentDate.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            };
            timer.Start();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string[]? portInfo = SerialPortConnection.GetSerialPortInformation();
            if (portInfo != null)
            {

            }
            else return;
        }
    }
}