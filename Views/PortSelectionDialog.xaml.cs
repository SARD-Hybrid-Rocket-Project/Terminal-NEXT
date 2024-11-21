using System;
using System.Collections.Generic;
using System.Management;
using System.Linq;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;

namespace FlightController
{
    /// <summary>
    /// PortSelectionDialogでやってること
    ///     ・初期化された時に、PnPデバイス（COMポート）を全検索してリストアップし、ComboBoxに表示する。
    ///     ・ComboBoxを開くたびに内容を更新する。
    ///     ・接続ボタンを押したら DialogResult = true でダイアログを閉じる。閉じるボタンとキャンセルボタンは同じ動作。
    /// </summary>
    public partial class PortSelectionDialog : Window
    {
        string[] BaudlateIndex = { "1200", "2400", "4800", "9600", "14400", "19200", "38400", "56000", "57600", "115200" };
        public PortSelectionDialog()
        {
            InitializeComponent();//Componentを初期化

            InitializePortList();//ComboBox_PortListを初期化
            InitializeBaudlateList();//ComboBox_Baudlateを初期化
        }
        private void InitializePortList() //ComboBox_PortListの初期化
        {
            ComboBox_PortList.IsEnabled = true;
            ComboBox_PortList.ItemsSource = null;
            ComboBox_PortList.ItemsSource = SerialPort_Enumeration();
            ComboBox_PortList.SelectedValuePath = "DeviceID";
            ComboBox_PortList.DisplayMemberPath = "Description";
            ComboBox_PortList.SelectedIndex = 0;

            if (ComboBox_PortList.Items.Count == 0) ComboBox_PortList.IsEnabled = false;
            else ComboBox_PortList.IsEnabled = true;
        }
        private void InitializeBaudlateList()//ComboBox_Baudlateを初期化
        {
            ComboBox_Baudlate.Items.Clear();//ComboBox_BaudLateを初期化するために中身を削除
            foreach (var i in BaudlateIndex)//BaudlateIndexをComboBox_Baudlateに追加する
            {
                ComboBox_Baudlate.Items.Add(i);
            }
            ComboBox_Baudlate.SelectedValue = "19200";//初期選択値を設定
        }
        private ObservableCollection<COMPort> SerialPort_Enumeration() //シリアルポートをObservableCollectionに列挙して返す
        {
            var SerialPortList = new ObservableCollection<COMPort>();

            var check = new System.Text.RegularExpressions.Regex("(COM[0-9]{1,3})");

            foreach(ManagementObject m in new ManagementClass("Win32_PnPEntity").GetInstances())//全てのPnPデバイスを調べ、シリアル通信を行うデバイスを追加
            {
                string name = m.GetPropertyValue("name")?.ToString() ?? string.Empty; //null条件演算子・null合体演算子

                if (check.IsMatch(name))//nameプロパティがcheckで指定した正規表現に部分一致した場合リストに追加
                {
                    string caption = m.GetPropertyValue("Caption")?.ToString() ?? string.Empty;
                    string id = System.Text.RegularExpressions.Regex.Matches(caption, "COM[0-9]{1,3}")[0].Value;

                    SerialPortList.Add(new COMPort { DeviceID = id, Description = caption });
                }
            }
            return SerialPortList;
        }
        
        private void ComboBox_PortList_DropDownOpened(object sender, EventArgs e)
        {
            InitializePortList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
    public class COMPort
    {
        public required string DeviceID { get; set; }
        public required string Description { get; set; }
    }
}
