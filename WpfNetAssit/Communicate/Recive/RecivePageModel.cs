using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.Communicate.Recive
{
    [Serializable]
    public class RecivePageModel : ViewModelBase
    {
        private ReciveShowPageModel reciveShowPageModel = new ReciveShowPageModel();
        public ReciveShowPageModel ReciveShowPageModel {
            get {return reciveShowPageModel; }
            set {Set("RecivieShowPageModel", ref reciveShowPageModel, value); } 
        }

        Task recvTask = null;
        bool IsRun = false;
        IoConnect.ICommunicateIo Io = null;
        public void StartRecive(IoConnect.ICommunicateIo io)
        {
            recvTask = Task.Run(() => {
                Io = io;
                byte[] buf = new byte[0x1000];
                int size = 0;
                IsRun = true;
                while (IsRun)
                {
                    bool ret = io.Read(buf, ref size);
                    if (ret && size > 0)
                    {
                        byte[] data = buf.Take(size).ToArray();
                        ReciveShowPageModel.RecvData(data);
                    }
                }
            });
        }

        public void StopRecive()
        {
            if (recvTask != null && !recvTask.IsCompleted)
            {
                IsRun = false;
                Io.Close();
                recvTask.Wait();
            }
        }
    }
}
