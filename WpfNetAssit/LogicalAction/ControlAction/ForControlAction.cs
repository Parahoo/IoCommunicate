using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.LogicalAction.ControlAction
{
    [Serializable]
    public class ForControlActionParam : ObservableObject
    {
        public string Info => Count.ToString();

        private int count = 10000;
        public int Count
        {
            get { return count; }
            set { Set("Count", ref count, value); }
        }

        public ForControlActionParam() { }
        public ForControlActionParam(int c) { Count = c; }

        internal ForControlActionParam Clone()
        {
            return new ForControlActionParam(Count);
        }
    }

        [Serializable]
    public class ForControlActionBuilder : LogicalActionBuilder
    {
        public  ForControlActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new ForControlAction(this);
        }

        public override object GetParam()
        {
            return Param.Clone();
        }

        public ForControlActionBuilder()
        {
            Param = new ForControlActionParam();
        }

        public ForControlActionBuilder(int count)
        {
            Param = new ForControlActionParam(count);
        }

        public ForControlActionBuilder(ForControlActionParam param)
        {
            Param = param.Clone();
        }
        public override string ToString()
        {
            return "循环组";
        }
    }

    /// <summary>
    /// for 循环操作
    /// </summary>
    public class ForControlAction : ControlAction
    {
        private ForControlActionParam param = new ForControlActionParam();
        public ForControlActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value); Name = ToString(); }
        }

        public override string ToString()
        {
            return "循环:";
        }

        public ForControlAction():base()
        {
            Name = ToString();
        }

        public ForControlAction(LogicalActionBuilder builder) :base(builder)
        {
            Param = builder.GetParam() as ForControlActionParam;
        }

        override public bool Act(object datacontext)
        {
            Dictionary<string, object> dict = datacontext as Dictionary<string, object>;
            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;

            for(int i = 0; i < param.Count; i++)
            {
                logfunc(string.Format("第{0}次循环", i+1), tab);
                dict["tab"] = tab + "  ";
                if (!DoChildAction(ChildActions, datacontext))
                    return false;
                dict["tab"] = tab;
            }
            return true;
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            var parent = new ForControlActionBuilder(Param);
            SerializeChildBuilder(parent);
            return parent;
        }
    }
}
