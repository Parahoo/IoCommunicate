using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNetAssit.Communicate;
using WpfNetAssit.IoConnect;

namespace WpfNetAssit.LogicalAction.BaseAction
{
    [Serializable]
    public class SendActionParam : ObservableObject
    {
        public string Info => ToString();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (HeadAppendHex != "")
                sb.Append("[" + HeadAppendHex + "]");
            sb.Append(Data);
            if (IsPlusR)
                sb.Append("\\r");
            if (IsPlusN)
                sb.Append("\\n");
            if(TailAppendHex != "")
                sb.Append("[" + TailAppendHex + "]");
            return sb.ToString();
        }

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

        private string headAppendHex = "";
        public string HeadAppendHex
        {
            get { return headAppendHex; }
            set { Set("HeadAppendHex", ref headAppendHex, HexStringConvertor.HexInput(value)); RaisePropertyChanged("Info"); }
        }

        private string tailAppendHex = "";
        public string TailAppendHex
        {
            get { return tailAppendHex; }
            set { Set("TailAppendHex", ref tailAppendHex, HexStringConvertor.HexInput(value)); RaisePropertyChanged("Info"); }
        }



        public SendActionParam() { }
        public SendActionParam(string v) { Data = v; }
        public SendActionParam(string v, bool br, bool bn, string head, string tail) { Data = v; IsPlusR = br; IsPlusN = bn; HeadAppendHex = head; TailAppendHex = tail; }
        public SendActionParam Clone()
        {
            return new SendActionParam(Data, IsPlusR, IsPlusN, HeadAppendHex, TailAppendHex);
        }

        public byte[] GetData()
        {
            var headdata = HexStringConvertor.StringToHex(HeadAppendHex);
            var taildata = HexStringConvertor.StringToHex(TailAppendHex);
            string str = Data;
            if (IsPlusR)
                str += "\r";
            if (IsPlusN)
                str += "\n";
            var bodydata = Encoding.UTF8.GetBytes(str);
            if (headdata.Length == 0 && taildata.Length == 0)
                return bodydata;
            else
            {
                var buf = new byte[headdata.Length + taildata.Length + bodydata.Length];
                headdata.CopyTo(buf, 0);
                bodydata.CopyTo(buf, headdata.Length);
                taildata.CopyTo(buf, headdata.Length + bodydata.Length);
                return buf;
            }
        }
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

            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;
            logfunc(string.Format("send: {0}", Param.Info), tab);

            var buf = Param.GetData();
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
