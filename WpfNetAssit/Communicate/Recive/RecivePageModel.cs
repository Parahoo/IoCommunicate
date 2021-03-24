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

    }
}
