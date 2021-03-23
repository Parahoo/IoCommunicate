using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNetAssit.IoConnect;

namespace WpfNetAssit.LogicalAction.BaseAction
{
    [Serializable]
    public class SendActionParam : ObservableObject
    {
        public string Info => (Data+ (IsPlusR?"\\r":"")+(IsPlusN?"\\n":""));

        private string data = "";
        public string Data
        {
            get { return data; }
            set { Set("Data", ref data, value);RaisePropertyChanged("Info"); }
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


        public SendActionParam() { }
        public SendActionParam(string v) { Data = v; }
        public SendActionParam(string v, bool br, bool bn) { Data = v; IsPlusR = br; IsPlusN = bn; }
        public SendActionParam Clone()
        {
            return new SendActionParam(Data, IsPlusR, IsPlusN);
        }

        public byte[] GetData()
        {
            string str = Data;
            if (IsPlusR)
                str += "\r";
            if (IsPlusN)
                str += "\n";
            return Encoding.UTF8.GetBytes(str);
        }
    }

    public class SendActionBuilder : LogicalActionBuilder
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
        public SendActionBuilder(string str) { Param = new SendActionParam() { Data = str }; }
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

            var buf = Param.GetData();
            int writesize = buf.Length;
            var io = dict["io"] as ICommunicateIo;
            return io.Write(buf, ref writesize);
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            return new SendActionBuilder(Param);
        }
    }
}
