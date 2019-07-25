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

namespace WpfNetAssit.Communicate.Recive
{
    /// <summary>
    /// RecivePage.xaml 的交互逻辑
    /// </summary>
    public partial class RecivePage : UserControl
    {
        public RecivePage()
        {
            InitializeComponent();
        }

        Task recvTask = null;
        bool IsRun = false;
        public void StartRecive(IoConnect.CommunicateIo io)
        {
            recvTask = Task.Run(() => {
                byte[] buf = new byte[0x1000];
                int size = 0;
                IsRun = true;
                while (IsRun)
                {
                    bool ret = io.Read(buf, ref size);
                    if (ret && size > 0)
                    {
                        byte[] data = buf.Take(size).ToArray();
                        ReciveShowPage.RecvData(data);
                    }
                }
            });
        }

        public void StopRecive()
        {
            if (recvTask != null && !recvTask.IsCompleted)
            {
                IsRun = false;
                recvTask.Wait();
            }
        }
    }
}
