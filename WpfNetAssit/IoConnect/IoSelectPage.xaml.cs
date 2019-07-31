using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WpfNetAssit.IoConnect
{
    /// <summary>
    /// IoSelectPage.xaml 的交互逻辑
    /// </summary>
    public partial class IoSelectPage : UserControl
    {
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(IoSelectPage), new FrameworkPropertyMetadata(false) { BindsTwoWayByDefault = true});


        [Bindable(true)]
        public int CurSel
        {
            get { return (int)GetValue(CurSelProperty); }
            set { SetValue(CurSelProperty, value); }
        }
        public static readonly DependencyProperty CurSelProperty =
            DependencyProperty.Register("CurSel", typeof(int), typeof(IoSelectPage), new FrameworkPropertyMetadata(0) { BindsTwoWayByDefault = true });


        public event EventHandler IoOpened
        {
            add { AddHandler(IoOpenedEvent, value); }
            remove { RemoveHandler(IoOpenedEvent, value); }
        }

        public static readonly RoutedEvent IoOpenedEvent = EventManager.RegisterRoutedEvent(
        "IoOpened", RoutingStrategy.Bubble, typeof(EventHandler), typeof(IoSelectPage));


        public event EventHandler IoClosed
        {
            add { AddHandler(IoClosedEvent, value); }
            remove { RemoveHandler(IoClosedEvent, value); }
        }

        public static readonly RoutedEvent IoClosedEvent = EventManager.RegisterRoutedEvent(
        "IoClosed", RoutingStrategy.Bubble, typeof(EventHandler), typeof(IoSelectPage));


        /// <summary>
        /// 串口参数
        /// </summary>
        public ComIoParam ComIoParam
        {
            get { return (ComIoParam)GetValue(ComIoParamProperty); }
            set { SetValue(ComIoParamProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComIoParam.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComIoParamProperty =
            DependencyProperty.Register("ComIoParam", typeof(ComIoParam), typeof(IoSelectPage), new FrameworkPropertyMetadata(new ComIoParam()) { BindsTwoWayByDefault = true });


        /// <summary>
        /// udp io param
        /// </summary>
        public NetIoParam UdpIoParam
        {
            get { return (NetIoParam)GetValue(UdpIoParamProperty); }
            set { SetValue(UdpIoParamProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UdpIoParam.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UdpIoParamProperty =
            DependencyProperty.Register("UdpIoParam", typeof(NetIoParam), typeof(IoSelectPage), new FrameworkPropertyMetadata(new NetIoParam()) { BindsTwoWayByDefault = true });


        /// <summary>
        /// tcp server io param
        /// </summary>
        public NetIoParam TcpServerIoParam
        {
            get { return (NetIoParam)GetValue(TcpServerIoParamProperty); }
            set { SetValue(TcpServerIoParamProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TcpServerIoParam.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TcpServerIoParamProperty =
            DependencyProperty.Register("TcpServerIoParam", typeof(NetIoParam), typeof(IoSelectPage), new FrameworkPropertyMetadata(new NetIoParam()) { BindsTwoWayByDefault = true });



        /// <summary>
        /// tcp client io param
        /// </summary>
        public NetIoParam TcpClientIoParam
        {
            get { return (NetIoParam)GetValue(TcpClientIoParamProperty); }
            set { SetValue(TcpClientIoParamProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TcpClientIoParam.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TcpClientIoParamProperty =
            DependencyProperty.Register("TcpClientIoParam", typeof(NetIoParam), typeof(IoSelectPage), new FrameworkPropertyMetadata(new NetIoParam()) { BindsTwoWayByDefault = true });





        public CommunicateIo CurIo { get; set; } = null;


        public IoSelectPage()
        {
            InitializeComponent();
        }

        private bool OpenIo()
        {
            switch(CurSel)
            {
                case 0:
                    {
                        var comIo = new ComIo();
                        comIo.Param = ComIoParam;
                        CurIo = comIo;
                        break;
                    }
                case 1:
                    {
                        var udpIo = new UdpIo();
                        udpIo.Param = UdpIoParam;
                        CurIo = udpIo;
                        break;
                    }
                case 3:
                    {
                        var tcpclientIo = new TcpIo();
                        tcpclientIo.Param = TcpClientIoParam;
                        CurIo = tcpclientIo;
                        break;
                    }
            }
            if (CurIo != null)
                return CurIo.Open();
            else
                return false;
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if(OpenIo())
            {
                IsOpen = true;
                RaiseEvent(new RoutedEventArgs(IoOpenedEvent, this));
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurIo != null)
            {
                CurIo.Close();
                CurIo = null;
            }

            IsOpen = false;
            RaiseEvent(new RoutedEventArgs(IoClosedEvent, this));
        }
    }
}
