using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.LogicalAction.ControlAction
{
    [Serializable]
    public class RetryControlActionParam : ObservableObject
    {
        public string Info => Count.ToString();

        private int count = 10000;
        public int Count
        {
            get { return count; }
            set { Set("Count", ref count, value); }
        }

        public RetryControlActionParam() { }
        public RetryControlActionParam(int c) { Count = c; }

        internal RetryControlActionParam Clone()
        {
            return new RetryControlActionParam(Count);
        }
    }

    [Serializable]
    public class RetryControlActionBuilder : ControlActionBuilder
    {
        public RetryControlActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new RetryControlAction(this);
        }

        public override object GetParam()
        {
            return Param.Clone();
        }

        public RetryControlActionBuilder()
        {
            Param = new RetryControlActionParam();
        }

        public RetryControlActionBuilder(int count)
        {
            Param = new RetryControlActionParam(count);
        }

        public RetryControlActionBuilder(RetryControlActionParam param)
        {
            Param = param.Clone();
        }
        public override string ToString()
        {
            return "重试组";
        }
    }

    /// <summary>
    /// for 循环操作
    /// </summary>
    public class RetryControlAction : ControlAction
    {
        private RetryControlActionParam param = new RetryControlActionParam();
        public RetryControlActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value); Name = ToString(); }
        }

        public override string ToString()
        {
            return "重试:";
        }

        public RetryControlAction() : base()
        {
            Name = ToString();
        }

        public RetryControlAction(LogicalActionBuilder builder) : base(builder)
        {
            Param = builder.GetParam() as RetryControlActionParam;
        }

        override public bool Act(object datacontext)
        {
            Dictionary<string, object> dict = datacontext as Dictionary<string, object>;
            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;

            int count = param.Count;
            bool bInfinity = false ;
            if (count == 0)
                count = 1;
            else if (count < 0)
            {
                count = int.MaxValue;
                bInfinity = true;
            }

            for (int i = 0; i < count; i++)
            {
                logfunc(string.Format("第{0}次尝试", i + 1), tab);
                dict["tab"] = tab + "  ";
                if (!DoChildAction(ChildActions, datacontext))
                {
                    if (bInfinity)
                    {
                        if (i == int.MaxValue - 1)
                            i = -1;
                    }
                    continue;
                }
                dict["tab"] = tab;
                return true;
            }
            return false;
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            var parent = new RetryControlActionBuilder(Param);
            SerializeChildBuilder(parent);
            return parent;
        }
    }
}
