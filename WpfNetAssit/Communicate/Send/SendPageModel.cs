using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNetAssit.Communicate.Send.LogicalSend;
using WpfNetAssit.Communicate.Send.NormalSend;
using WpfNetAssit.IoConnect;

namespace WpfNetAssit.Communicate.Send
{
    [Serializable]
    public class SendPageModel : ViewModelBase
    {
        private NormalSendPageModel normalSendPageModel = new NormalSendPageModel();
        public NormalSendPageModel NormalSendPageModel
        {
            get { return normalSendPageModel; }
            set { Set("NormalSendPageModel", ref normalSendPageModel, value); }
        }

        private LogicalSendPageModel logicalSendPageModel = new LogicalSendPageModel();
        [System.Xml.Serialization.XmlIgnore]
        public LogicalSendPageModel LogicalSendPageModel
        {
            get { return logicalSendPageModel; }
            set { Set("LogicalSendPageModel", ref logicalSendPageModel, value); }
        }


        ICommunicateIo Io = null;
        internal void SetIo(ICommunicateIo io)
        {
            Io = io;
            normalSendPageModel.SetIo(io);
            logicalSendPageModel.SetIo(io);
        }
    }
}
