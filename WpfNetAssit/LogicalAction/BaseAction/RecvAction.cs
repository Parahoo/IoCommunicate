using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfNetAssit.IoConnect;

namespace WpfNetAssit.LogicalAction.BaseAction
{
    [Serializable]
    public class RecvActionParam : ObservableObject
    {
        public string Info => (string.Format("in {0}ms", timeout));

        private int timeout = 1000;
        public int Timeout
        {
            get { return timeout; }
            set { Set("Timeout", ref timeout, value); RaisePropertyChanged("Info"); }
        }

        public RecvActionParam() { }
        public RecvActionParam(int v) { Timeout = v; }
        public RecvActionParam Clone()
        {
            return new RecvActionParam(Timeout);
        }
    }

    public class RecvActionBuilder : BaseActionBuilder
    {
        public RecvActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new RecvAction(this);
        }

        public override object GetParam()
        {
            return Param.Clone();
        }

        public RecvActionBuilder() { Param = new RecvActionParam(); }
        public RecvActionBuilder(int v) { Param = new RecvActionParam() { Timeout = v }; }
        public RecvActionBuilder(RecvActionParam param) { Param = param.Clone(); }
        public override string ToString()
        {
            return "接收";
        }
    }

    public class RecvAction : BaseAction
    {
        private RecvActionParam param = new RecvActionParam();
        public RecvActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value); Name = ToString(); }
        }

        public override string ToString()
        {
            return "接收: ";
        }

        public RecvAction(LogicalActionBuilder builder) : base(builder)
        {
            Param = builder.GetParam() as RecvActionParam;
        }

        public RecvAction() : base()
        {
        }

        public override bool Act(object datacontext)
        {
            var dict = datacontext as Dictionary<string, object>;

            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;
            var cancel = (CancellationToken)dict["canceltoken"];

            logfunc("recv...", tab);


            var io = dict["io"] as IoPipe;
            byte[] buf = null;
            double usedtime = 0.0;
            bool ret = io.Read(Param.Timeout, cancel, ref buf, ref usedtime);
            if (ret)
            {
                logfunc(string.Format("recved:{0} in {1}ms", DataToStringHelper.BeautifyToString(buf), usedtime), tab);
                dict["data"] = buf;
            }
            else
                logfunc(string.Format("recv: timeout {0}ms", Param.Timeout), tab);
            return ret;
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            return new RecvActionBuilder(Param);
        }
    }
}
