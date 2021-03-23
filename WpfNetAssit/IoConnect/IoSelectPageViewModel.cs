using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfNetAssit.IoConnect
{
    public class IoSelectPageViewModel : ViewModelBase
    {

        private IosSetting iosSetting = new IosSetting();
        public IosSetting IosSetting { 
            get => iosSetting; 
            set => Set("IosSetting", ref iosSetting, value);
        }

        [System.Xml.Serialization.XmlIgnore]
        public SnackbarMessageQueue BoundMessageQueue { get; } = new SnackbarMessageQueue();

        public ICommand OpenCommand { get; }
        public ICommand StartIoCommand { get; set; }

        public IoSelectPageViewModel()
        {
            OpenCommand = new RelayCommand(Open);
        }

        private bool OpenIo(ref ICommunicateIo CurIo, ref string message)
        {
            switch (IosSetting.IoSel)
            {
                case 0:
                    {
                        var comIo = new ComIo();
                        comIo.Param = IosSetting.ComIoParam.Clone() as ComIoParam;
                        CurIo = comIo;
                        break;
                    }
                case 1:
                    {
                        var udpIo = new UdpIo();
                        udpIo.Param = IosSetting.UdpIoParam.Clone() as NetIoParam;
                        CurIo = udpIo;
                        break;
                    }
                case 3:
                    {
                        var tcpclientIo = new TcpIo();
                        tcpclientIo.Param = IosSetting.TcpClientIoParam.Clone() as NetIoParam;
                        CurIo = tcpclientIo;
                        break;
                    }
            }
            if (CurIo != null)
            {
                var ret = CurIo.Open();
                if (!ret)
                    message = CurIo.ErrorInfo;
                return ret;
            }
            else
            {
                message = "未支持的数据通道";
                return false;
            }
        }

        public void Open()
        {
            ICommunicateIo CurIo = null;
            string message = "";
            if (OpenIo(ref CurIo, ref message))
            {
                StartIoCommand?.Execute(CurIo);                
            }
            else
            {
                BoundMessageQueue.Clear();
                BoundMessageQueue.Enqueue(message);
            }
        }
    }
}
