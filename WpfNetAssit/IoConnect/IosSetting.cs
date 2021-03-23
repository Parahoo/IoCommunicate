using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.IoConnect
{
    [Serializable]
    public class IosSetting : ObservableObject
    {
        public int IoSel { get; set; } = 0;

        public ComIoParam ComIoParam { get; set; } = new ComIoParam();

        public NetIoParam UdpIoParam { get; set; } = new NetIoParam();

        public NetIoParam TcpServerIoParam { get; set; } = new NetIoParam();

        public NetIoParam TcpClientIoParam { get; set; } = new NetIoParam();
    }
}
