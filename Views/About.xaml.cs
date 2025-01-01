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
    /// About.xaml の相互作用ロジック
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            this.Title = MissionController.Resources.Resource.ApplicationName;
            TextBlock_ApplicationTitle.Text = MissionController.Resources.Resource.ApplicationName;
        }
        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
