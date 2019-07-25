using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfNetAssit.Communicate.Send
{
    public class SendDataEventArgs : RoutedEventArgs
    {
        public SendDataEventArgs(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; }
    }

    public delegate void SendDataEventHandler(object sender, SendDataEventArgs e);
}
