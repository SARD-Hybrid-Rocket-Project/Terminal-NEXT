using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MissionController.Views
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemLogWindow : Window
    {
        App app = (App)App.Current;
        public SystemLogWindow()
        {
            InitializeComponent();

            this.Title = Core.Constants.ApplicationName;

            RichTextBox_SystenLog.Document = app.SystemLogDocument.FlowDocument;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
            RichTextBox_SystenLog.Document = new FlowDocument();
        }
    }
}
