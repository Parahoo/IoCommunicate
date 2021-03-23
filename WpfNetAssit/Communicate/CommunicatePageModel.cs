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


        public CommunicatePageModel()
        {

        }

       

        public void StartCommunicate(IoConnect.ICommunicateIo io)
        {
            RecivePageModel.StartRecive(io);
            SendPageModel.SetIo(io);
        }

        public void StopCommunicate()
        {
            RecivePageModel.StopRecive();
            SendPageModel.SetIo(null);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
