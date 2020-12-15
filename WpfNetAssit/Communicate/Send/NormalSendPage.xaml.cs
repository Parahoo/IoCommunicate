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

namespace WpfNetAssit.Communicate.Send
{
    /// <summary>
    /// NormalSendPage.xaml 的交互逻辑
    /// </summary>
    public partial class NormalSendPage : UserControl
    {
        public NormalSendPage()
        {
            InitializeComponent();
        }


        public event SendDataEventHandler DataSend
        {
            add { AddHandler(DataSendEvent, value); }
            remove { RemoveHandler(DataSendEvent, value); }
        }

        public static readonly RoutedEvent DataSendEvent = EventManager.RegisterRoutedEvent(
        "DataSend", RoutingStrategy.Bubble, typeof(SendDataEventHandler), typeof(NormalSendPage));


        private void InputTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(IsRealtimeSendCheckBox.IsOn == true)
            {
                SendText(e.Text);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 根据编码发送文本
        /// </summary>
        /// <param name="text"></param>
        private void SendText(string text)
        {
            byte[] data = EncodingSelector.CurEncoding.GetBytes(text);
            SendData(data);
        }

        /// <summary>
        /// 将2进制数据发送
        /// </summary>
        /// <param name="data"></param>
        private void SendData(byte[] data)
        {
            var args = new SendDataEventArgs(data);
            args.RoutedEvent = DataSendEvent;
            args.Source = this;

            RaiseEvent(args);

        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            byte[] src = DataEditor.Data;
            byte[] data = src;
            if (IsAddEnterCheckBox.IsChecked == true)
            {
                data = new byte[src.Length + 1];
                src.CopyTo(data, 0);
                data[src.Length] = 13;
            }
            SendData(data);
        }
    }
}
