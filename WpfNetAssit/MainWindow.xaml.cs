using MahApps.Metro.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfNetAssit
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void IoSelectPage_IoOpened(object sender, EventArgs e)
        {
            FunctionTab.SelectedIndex = 1;
            communicatePage.StartCommunicate(ioSelectPage.CurIo);
        }

        private void IoSelectPage_IoClosed(object sender, EventArgs e)
        {
            communicatePage.StopCommunicate();
        }
    }
}
