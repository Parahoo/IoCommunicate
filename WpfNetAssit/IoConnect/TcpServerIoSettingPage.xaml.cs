using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace WpfNetAssit.IoConnect
{
    /// <summary>
    /// TcpServerIoSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class TcpServerIoSettingPage : UserControl
    {
        public TcpServerIoSettingPage()
        {
            InitializeComponent();
        }
        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            List<string> ipv4Ips = new List<string>();
            ipv4Ips.Add("127.0.0.1");

            //获取本机可用IP地址  
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    ipv4Ips.Add(ip.ToString());
            }

            LocalIpComboBox.ItemsSource = ipv4Ips;
        }

        private void LocalIpComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsBindLocalIpCheckBox.IsChecked = LocalIpComboBox.SelectedIndex != -1;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LocalPortTextBox.IsFocused)
                IsBindLocalPortCheckBox.IsChecked = (LocalPortTextBox.Text != "") && (LocalPortTextBox.Text != "0");
        }
    }
}
