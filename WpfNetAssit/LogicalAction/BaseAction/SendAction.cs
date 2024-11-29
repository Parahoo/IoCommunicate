using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNetAssit.Communicate;
using WpfNetAssit.IoConnect;

namespace WpfNetAssit.LogicalAction.BaseAction
{
    interface ISendData
    {
        byte[] GetData(object datacontext);
    }
    public class SendData : ObservableObject, ISendData
    {
        protected ObservableCollection<SendData> parent = null;

        public void SetParent(ObservableCollection<SendData> parent) {  this.parent = parent; }

        public virtual byte[] GetData(object datacontext)
        {
            throw new NotImplementedException();
        }

        public virtual SendData Clone()
        {
            throw new NotImplementedException();
        }

        public ICommand DeleteFromParentCommand
        {
            get
            {
                if(deleteFromParentCommand == null)
                {
                    deleteFromParentCommand = new RelayCommand(() =>
                    {
                        parent.Remove(this);
                    });
                }
                return deleteFromParentCommand;
            }
        }
        RelayCommand deleteFromParentCommand = null;
    }

    [Serializable]
    public class StaticSendData : SendData
    {

        private string data = "";
        public string Data
        {
            get { return data; }
            set { 
                Set("Data", ref data, value); 
                RaisePropertyChanged("Data");
            }
        }

        public override byte[] GetData(object datacontext)
        {
            return Encoding.UTF8.GetBytes(data);
        }
        public override string ToString()
        {
            return data;
        }

        public override SendData Clone()
        {
            return new StaticSendData()
            {
                Data = data
            };
        }
    }

    [Serializable]
    public class MetaSendData : SendData
    {
        private string metaKey = "";
        public string MetaKey
        {
            get { return metaKey; }
            set { Set("MetaKey", ref metaKey, value); RaisePropertyChanged("MetaKey"); }
        }

        public override string ToString()
        {
            return "{" + metaKey + "}";
        }

        public override SendData Clone()
        {
            return new MetaSendData() { MetaKey = metaKey };
        }

        public override byte[] GetData(object datacontext)
        {
            if(datacontext == null)
                return new byte[]{ };
            var dict = datacontext as Dictionary<string, object>;
            if(dict == null)
                return new byte[] { };
            if (dict.ContainsKey(metaKey) == false)
                return new byte[] { };
            return dict[metaKey] as byte[];
        }
    }

    [Serializable]
    public class SendActionParam : ObservableObject
    {
        public string Info => ToString();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (HeadAppendHex != "" && HeadAppendRepeat > 0)
            {
                if (HeadAppendRepeat > 1)
                    sb.Append("[" + HeadAppendHex + "  *" + HeadAppendRepeat.ToString() + "]");
                else
                    sb.Append("[" + HeadAppendHex + "]");
            }
            sb.Append(Data);
            foreach (var item in PlusData)
            {
                sb.Append(item);
            }
            if (IsPlusR)
                sb.Append("\\r");
            if (IsPlusN)
                sb.Append("\\n");
            if (TailAppendHex != "" && TailAppendRepeat > 0)
            {
                if (TailAppendRepeat > 1)
                    sb.Append("[" + TailAppendHex + "  *" + TailAppendRepeat.ToString() + "]");
                else
                    sb.Append("[" + TailAppendHex + "]");
            }
            return sb.ToString();
        }

        private string data = "";
        public string Data
        {
            get { return data; }
            set { Set("Data", ref data, value); RaisePropertyChanged("Info"); }
        }

        private ObservableCollection<SendData> plusData = new ObservableCollection<SendData>();
        public ObservableCollection<SendData> PlusData
        {
            get { return plusData; }
            set { plusData = value; RaisePropertyChanged("Info"); }
        }

        private bool isPlusR = false;
        public bool IsPlusR
        {
            get { return isPlusR; }
            set { Set("IsPlusR", ref isPlusR, value); RaisePropertyChanged("Info"); }
        }

        private bool isPlusN = false;
        public bool IsPlusN
        {
            get { return isPlusN; }
            set { Set("IsPlusN", ref isPlusN, value); RaisePropertyChanged("Info"); }
        }

        private string headAppendHex = "";
        public string HeadAppendHex
        {
            get { return headAppendHex; }
            set { Set("HeadAppendHex", ref headAppendHex, HexStringConvertor.HexInput(value)); RaisePropertyChanged("Info"); }
        }

        private int headAppendRepeat = 1;
        public int HeadAppendRepeat
        {
            get { return headAppendRepeat; }
            set { Set("HeadAppendRepeat", ref headAppendRepeat, value); RaisePropertyChanged("Info"); }
        }

        private string tailAppendHex = "";
        public string TailAppendHex
        {
            get { return tailAppendHex; }
            set { Set("TailAppendHex", ref tailAppendHex, HexStringConvertor.HexInput(value)); RaisePropertyChanged("Info"); }
        }

        private int tailAppendRepeat = 1;
        public int TailAppendRepeat
        {
            get { return tailAppendRepeat; }
            set { Set("TailAppendRepeat", ref tailAppendRepeat, value); RaisePropertyChanged("Info"); }
        }



        public SendActionParam() {
            BindPlusData();
        }
        public SendActionParam(SendActionParam param)
        {
            Data = param.Data; 
            IsPlusR = param.IsPlusR; 
            IsPlusN = param.IsPlusN;
            HeadAppendHex = param.HeadAppendHex; 
            HeadAppendRepeat = param.HeadAppendRepeat;
            TailAppendHex = param.TailAppendHex; 
            TailAppendRepeat = param.TailAppendRepeat;

            PlusData = new ObservableCollection<SendData>();
            BindPlusData();
            foreach (var item in param.PlusData)
            {
                var nitem = item.Clone();
                PlusData.Add(nitem);
            }
        }
        void BindPlusData()
        {
            PlusData.CollectionChanged += (s, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        var item2 = item as SendData;
                        item2.SetParent(PlusData);
                        item2.PropertyChanged += (s2, e2) => RaisePropertyChanged("Info");
                    }
                }
                RaisePropertyChanged("Info");
            };

        }
        public SendActionParam Clone()
        {
            return new SendActionParam(this);
        }

        public byte[] GetData(object datacontext)
        {
            var headdata = HexStringConvertor.StringToHex(HeadAppendHex);
            var taildata = HexStringConvertor.StringToHex(TailAppendHex);

            string str = Data;
            var bodydata = Encoding.UTF8.GetBytes(str);

            List<byte[]> plusdatabytes = new List<byte[]>();
            foreach (var item in PlusData)
                plusdatabytes.Add(item.GetData(datacontext));

            string crstr = "";
            if (IsPlusR)
                crstr += "\r";
            if (IsPlusN)
                crstr += "\n";
            var crstrdata = Encoding.UTF8.GetBytes(crstr);


            int headRepeat = HeadAppendRepeat >= 0 ? HeadAppendRepeat : 0;
            int tailRepeat = TailAppendRepeat >= 0 ? TailAppendRepeat : 0;
            int totalsize = headdata.Length * headRepeat + taildata.Length * tailRepeat + bodydata.Length + crstrdata.Length;
            foreach (var item in plusdatabytes)
                totalsize += item.Length;
            if(totalsize <= 0)
                return new byte[]{ };

            var buf = new byte[totalsize];
            int pos = 0;
            for (int i = 0; i < headRepeat; i++)
            {
                headdata.CopyTo(buf, pos);
                pos += headdata.Length;
            }

            bodydata.CopyTo(buf, pos);
            pos += bodydata.Length;

            foreach (var item in plusdatabytes)
            {
                item.CopyTo(buf, pos);
                pos += item.Length;
            }

            crstrdata.CopyTo(buf, pos);
            pos += crstrdata.Length;

            for (int i = 0; i < tailRepeat; i++)
            {
                taildata.CopyTo(buf, pos);
                pos += taildata.Length;
            }
            return buf;
        }

        public ICommand AddStaticStringDataCommand {
            get 
            {
                if (addStaticStringDataCommand == null)
                    addStaticStringDataCommand = new RelayCommand(new Action(() => {
                        var data = new StaticSendData() { Data = "" };
                        PlusData.Add(data); 
                    })); 
                return addStaticStringDataCommand; 
            } 
        }
        RelayCommand addStaticStringDataCommand = null;

        public ICommand AddFileMetaDataCommand
        {
            get
            {
                if (addFileMetaDataCommand == null)
                    addFileMetaDataCommand = new RelayCommand(new Action(() => {
                        var data = new MetaSendData() { MetaKey = "filedata" };
                        PlusData.Add(data);
                    }));
                return addFileMetaDataCommand;
            }
        }
        RelayCommand addFileMetaDataCommand = null;

        public ICommand AddIoRecvMetaDataCommand
        {
            get
            {
                addIoRecvMetaDataCommand = new RelayCommand(new Action(() =>
                {
                    var data = new MetaSendData() { MetaKey = "recvdata" };
                    PlusData.Add(data);
                }));
                return addIoRecvMetaDataCommand;
            }
        }
        RelayCommand addIoRecvMetaDataCommand = null;
    }

    public class SendActionBuilder : BaseActionBuilder
    {
        public SendActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new SendAction(this);
        }

        public override object GetParam()
        {
            return Param.Clone();
        }

        public SendActionBuilder() { Param = new SendActionParam(); }
        public SendActionBuilder(SendActionParam param) { Param = param.Clone(); }
        public override string ToString()
        {
            return "发送";
        }
    }

    public class SendAction : BaseAction
    {
        private SendActionParam param = new SendActionParam();
        public SendActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value);Name = ToString(); }
        }

        public override string ToString()
        {
            return "发送: ";
        }

        public SendAction(LogicalActionBuilder builder) :base(builder)
        {
            Param = builder.GetParam() as SendActionParam;
        }

        public SendAction():base()
        {
        }

        public override bool Act(object datacontext)
        {
            var dict = datacontext as Dictionary<string, object>;

            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;
            logfunc(string.Format("send: {0}", Param.Info), tab);

            var buf = Param.GetData(datacontext);
            int writesize = buf.Length;
            var io = dict["io"] as IoPipe;
            var ret = io.Write(buf, ref writesize);

            logfunc(string.Format("send: {0}", ret ? "ok":"faild"), tab);
            return ret;
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            return new SendActionBuilder(Param);
        }
    }
}
