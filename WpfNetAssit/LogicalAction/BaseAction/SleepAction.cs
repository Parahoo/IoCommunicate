using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfNetAssit.LogicalAction.BaseAction
{
    [Serializable]
    public class SleepActionParam : ObservableObject
    {
        public string Info => SleepMs.ToString()+"ms";

        private int sleepMs = 1000;
        public int SleepMs
        {
            get { return sleepMs; }
            set { Set("SleepMs", ref sleepMs, value); RaisePropertyChanged("Info"); }
        }

        public SleepActionParam() { }
        public SleepActionParam(int v) { SleepMs = v; }
        public SleepActionParam Clone()
        {
            return new SleepActionParam(SleepMs);
        }
    }

    public class SleepActionBuilder : LogicalActionBuilder
    {
        public SleepActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new SleepAction(this);
        }

        public override object GetParam()
        {
            return Param.Clone();
        }

        public SleepActionBuilder() { Param = new SleepActionParam(); }
        public SleepActionBuilder(int ms) { Param = new SleepActionParam() { SleepMs = ms }; }
        public SleepActionBuilder(SleepActionParam param) { Param = param.Clone(); }
        public override string ToString()
        {
            return "休眠 x ms";
        }
    }

    public class SleepAction : BaseAction
    {
        private SleepActionParam param = new SleepActionParam();
        public SleepActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value); Name = ToString(); }
        }

        public override string ToString()
        {
            return "休眠: ";
        }

        public SleepAction(LogicalActionBuilder builder) : base(builder)
        {
            Param = builder.GetParam() as SleepActionParam;
        }

        public SleepAction() : base()
        {
        }

        public override bool Act(object datacontext)
        {
            var dict = datacontext as Dictionary<string, object>;
            var cancel = (CancellationToken)dict["canceltoken"];

            Task.Delay(Param.SleepMs, cancel).Wait();
            return true;
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            return new SleepActionBuilder(Param);
        }
    }
}
