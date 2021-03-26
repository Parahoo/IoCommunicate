using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.LogicalAction.ControlAction
{
    public class PickControlActionParam : ObservableObject
    {
        public string Info => IsDisorder ? "随机" : "循环";
        private bool isDisorder = true;
        public bool IsDisorder
        {
            get { return isDisorder; }
            set { Set("IsDisorder", ref isDisorder, value); RaisePropertyChanged("Info"); }
        }

        public PickControlActionParam() { }
        public PickControlActionParam(bool v) { IsDisorder = v; }
        public PickControlActionParam Clone()
        {
            return new PickControlActionParam(IsDisorder);
        }
    }

    public class PickControlActionBuilder : ControlActionBuilder
    {
        public PickControlActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new PickControlAction(this);
        }

        public override object GetParam()
        {
            return Param;
        }

        public PickControlActionBuilder() { Param = new PickControlActionParam(); }
        public PickControlActionBuilder(bool bPick) { Param = new PickControlActionParam() { IsDisorder = bPick }; }
        public PickControlActionBuilder(PickControlActionParam param) { Param = param.Clone(); }

        public override string ToString()
        {
            return "挑选组";
        }
    }

    /// <summary>
    /// 随机乱序执行操作
    /// </summary>
    public class PickControlAction : ControlAction
    {
        private PickControlActionParam param = new PickControlActionParam();
        public PickControlActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value); Name = ToString(); }
        }
        public PickControlAction() : base()
        {
            Name = ToString();
        }

        public PickControlAction(LogicalActionBuilder builder) : base(builder)
        {
            Param = builder.GetParam() as PickControlActionParam;
        }

        public override string ToString()
        {
            return "挑选：";
        }

        private int pick = 0;
        Random random = new Random();
        private ObservableLogicalAction PickActions(ObservableCollection<ObservableLogicalAction> org)
        {
            if (org.Count == 0)
                return new ObservableLogicalAction();

            var collections = org;
            if(param.IsDisorder)
            {
                pick = random.Next(collections.Count);
                return collections[pick]; 
            }
            else
            {
                var item = collections[pick];
                pick++;
                if (pick >= collections.Count)
                    pick = 0;
                return item;
            }
        }

        public override bool Act(object datacontext)
        {
            bool ret = false;
            Dictionary<string, object> dict = datacontext as Dictionary<string, object>;
            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;
            dict["tab"] = tab + "  ";

            if (param.IsDisorder)
            {
                logfunc("随机挑选：", tab);
            }
            else
            {
                logfunc("循环挑选：", tab);
            }
            var action = PickActions(ChildActions);
            ret = action != null ? action.Act(datacontext) : false;

            dict["tab"] = tab;
            return ret;
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            var parent = new PickControlActionBuilder(Param);
            SerializeChildBuilder(parent);
            return parent;
        }
    }
}
