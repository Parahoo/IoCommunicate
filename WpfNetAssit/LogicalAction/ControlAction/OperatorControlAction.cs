using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfNetAssit.LogicalAction.ControlAction
{
    public class OperatorControlActionParam : ObservableObject
    {
        public string Info => isAnd ? "与" : (isOr ? "或" : "非");

        private bool isAnd = false;
        public bool IsAnd
        {
            get { return isAnd; }
            set { Set("IsAnd", ref isAnd, value); }
        }

        private bool isOr = true;
        public bool IsOr
        {
            get { return isOr; }
            set { Set("IsOr", ref isOr, value); }
        }

        private bool isNot = false;
        public bool IsNot
        {
            get { return isNot; }
            set { Set("IsNot", ref isNot, value); }
        }



        public OperatorControlActionParam() { }
        public OperatorControlActionParam(bool andv, bool orv, bool notv) { 
            IsAnd = andv; IsOr = orv; IsNot = notv;
        }
        public OperatorControlActionParam Clone()
        {
            return new OperatorControlActionParam(IsAnd, IsOr, IsNot);
        }
    }

    public class OperatorControlActionBuilder : ControlActionBuilder
    {
        public OperatorControlActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new OperatorControlAction(this);
        }

        public override object GetParam()
        {
            return Param;
        }

        public OperatorControlActionBuilder() { Param = new OperatorControlActionParam(); }
        public OperatorControlActionBuilder(bool andv, bool orv, bool notv) { Param = new OperatorControlActionParam(andv, orv, notv); }
        public OperatorControlActionBuilder(OperatorControlActionParam param) { Param = param.Clone(); }

        public override string ToString()
        {
            return "与或非组";
        }
    }

    /// <summary>
    /// 随机乱序执行操作
    /// </summary>
    public class OperatorControlAction : ControlAction
    {
        private OperatorControlActionParam param = new OperatorControlActionParam();
        public OperatorControlActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value); Name = ToString(); }
        }
        public OperatorControlAction() : base()
        {
            Name = ToString();
        }

        public OperatorControlAction(LogicalActionBuilder builder) : base(builder)
        {
            Param = builder.GetParam() as OperatorControlActionParam;
        }

        public override string ToString()
        {
            return "";
        }

        public override bool Act(object datacontext)
        {
            bool ret = false;
            Dictionary<string, object> dict = datacontext as Dictionary<string, object>;
            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;
            dict["tab"] = tab + "  ";

            if (param.IsAnd)
            {
                logfunc("与：", tab);
                ret = DoChildAction(ChildActions, datacontext);
            }
            else if(param.IsOr)
            { 
                logfunc("或：", tab);
                ret = DoOrChildAction(ChildActions, datacontext);
            }
            else
            {
                logfunc("非：", tab);
                ret = !DoChildAction(ChildActions, datacontext);
            }

            dict["tab"] = tab;
            return ret;
        }

        private bool DoOrChildAction(ObservableCollection<ObservableLogicalAction> actions, object datacontext)
        {
            var dict = datacontext as Dictionary<string, object>;
            var cancel = (CancellationToken)dict["canceltoken"];

            foreach (var action in actions)
            {
                cancel.ThrowIfCancellationRequested();
                if (action.Act(datacontext))
                    return true;
            }
            return false;
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            var parent = new OperatorControlActionBuilder(Param);
            SerializeChildBuilder(parent);
            return parent;
        }
    }
}
