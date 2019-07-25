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
    /// SendPage.xaml 的交互逻辑
    /// </summary>
    public partial class SendPage : UserControl
    {
        public SendPage()
        {
            InitializeComponent();
        }

        public IoConnect.CommunicateIo Io = null;

        private void SendPage_DataSend(object sender, SendDataEventArgs e)
        {
            if (Io != null)
            {
                int size = e.Data.Length;
                Io.Write(e.Data, ref size);
            }
        }
    }
}
