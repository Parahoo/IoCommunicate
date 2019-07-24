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
            InitEncodingComboBox();
            StartUiUpdateTimer();
        }

        private void InitEncodingComboBox()
        {
            EncodingComboBox.ItemsSource = new Encoding[]{
                Encoding.Default, Encoding.UTF8, Encoding.Unicode, Encoding.ASCII,
                Encoding.GetEncoding("GB2312"), Encoding.GetEncoding("Big5")
            };
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

        public bool IsShowText { get; set; } = true;

        private void CountAdd()
        {
            CurCount++;
            TotalCount++;
        }
        private void IsShowTextCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IsShowText = true;
        }

        private void IsShowTextCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsShowText = false;
        }

        private void ClearMonitorBtn_Click(object sender, RoutedEventArgs e)
        {
            MonitorTextBox.Text = "";
        }

        private void ClearCount_Click(object sender, RoutedEventArgs e)
        {
            CurCount = 0;
        }

        private void EncodingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurEncoding = EncodingComboBox.SelectedItem as Encoding;
        }

        private void MonitorData(byte[] buf, int size)
        {
            byte[] data = buf.Take(size).ToArray();

            string t = TransToString(data);
            Dispatcher.Invoke(() => {
                MonitorTextBox.AppendText(t);
            });
        }

        // 根据当前编码转换为string
        public Encoding CurEncoding { get; set; } = Encoding.Default;
        private string TransToString(byte[] buf)
        {
            return CurEncoding.GetString(buf);
        }

        public void RecvData(byte[] buf, int size)
        {
            CountAdd();
            if (IsShowText)
                MonitorData(buf, size);
        }

    }
}
