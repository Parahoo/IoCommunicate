using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNetAssit.Communicate.Recive;
using WpfNetAssit.Communicate.Send;

namespace WpfNetAssit.Communicate
{
    public class CommunicatePageModel : ViewModelBase , ICloneable
    {
        private RecivePageModel recivePageModel = new RecivePageModel();
        public RecivePageModel RecivePageModel {
            get {return recivePageModel; }
            set {Set("RecivePageModel", ref recivePageModel, value); } 
        }

        private SendPageModel sendPageModel = new SendPageModel();
        public SendPageModel SendPageModel
        {
            get { return sendPageModel; }
            set { Set("SendPageModel", ref sendPageModel, value); }
        }

        private Action<byte[]> ProcessRecv;


        public CommunicatePageModel()
        {
            ProcessRecv += SendPageModel.LogicalSendPageModel.RecvData;
            ProcessRecv += RecivePageModel.ReciveShowPageModel.RecvData;
        }       

        public void StartCommunicate(IoConnect.ICommunicateIo io)
        {
            SendPageModel.SetIo(io);
        }

        public void StopCommunicate()
        {
            SendPageModel.LogicalSendPageModel.Stop();
            SendPageModel.SetIo(null);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        internal void ProcessRecvData(byte[] data)
        {
            ProcessRecv?.Invoke(data);
        }
    }
}
