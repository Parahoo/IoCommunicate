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
using System.Windows.Threading;

namespace WpfNetAssit.Communicate.Recive
{
    /// <summary>
    /// ReciveShowPage.xaml 的交互逻辑
    /// </summary>
    public partial class ReciveShowPage : UserControl
    {
        public ReciveShowPage()
        {
            InitializeComponent();
            StartUiUpdateTimer();
        }


        private void StartUiUpdateTimer()
        {
            DispatcherTimer uiUpdateTimer = new DispatcherTimer();
            uiUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            uiUpdateTimer.Tick += UiUpdate;
            uiUpdateTimer.Start();
        }

        private void UiUpdate(object sender, EventArgs e)
        {
            TotalCountLabel.Content = TotalCount.ToString();
            CurCountLabel.Content = CurCount.ToString();
        }

        public int TotalCount { get; set; } = 0;
        public int CurCount { get; set; } = 0;
        private void CountAdd()
        {
            CurCount++;
            TotalCount++;
        }

        public bool IsShowText { get; set; } = true;
        private void IsShowTextCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IsShowText = true;
        }

        private void IsShowTextCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsShowText = false;
        }

        public bool IsHex { get; set; } = false;

        private void IsHexCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IsHex = true;
        }

        private void IsHexCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsHex = false;
        }


        private void ClearMonitorBtn_Click(object sender, RoutedEventArgs e)
        {
            MonitorTextBox.Text = "";
        }

        private void ClearCount_Click(object sender, RoutedEventArgs e)
        {
            CurCount = 0;
        }


        /// <summary>
        /// 显示接收到的数据
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="size"></param>
        private void MonitorData(byte[] data)
        {

            string t = TransToString(data);
            Dispatcher.Invoke(() => {
                MonitorTextBox.AppendText(t);
            });
        }



        private string EncodingToString(byte[] buf)
        {
            return EncodingSelector.EncodingToString(buf);
        }


        /// <summary>
        /// 将数据转化为显示文本
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        private string TransToString(byte[] buf)
        {
            if (IsHex)
                return HexStringConvertor.HexToString(buf);
            else
                return EncodingToString(buf);
        }

        public void RecvData(byte[] data)
        {
            CountAdd();
            if (IsShowText)
                MonitorData(data);
        }
    }
}
