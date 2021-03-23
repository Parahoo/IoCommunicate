using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.LogicalAction.ControlAction
{
    public class DisorderControlActionParam : ObservableObject
    {
        public string Info => IsDisorder ? "乱序" : "顺序";
        private bool isDisorder = new bool();
        public bool IsDisorder
        {
            get { return isDisorder; }
            set { Set("IsDisorder", ref isDisorder, value); RaisePropertyChanged("Info"); }
        }

        public DisorderControlActionParam() { }
        public DisorderControlActionParam(bool v) { IsDisorder = v; }
        public DisorderControlActionParam Clone()
        {
            return new DisorderControlActionParam(IsDisorder);
        }
    }

    public class DisorderControlActionBuilder : LogicalActionBuilder
    {
        public DisorderControlActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new DisorderControlAction(this);
        }

        public override object GetParam()
        {
            return Param;
        }

        public DisorderControlActionBuilder() { Param = new DisorderControlActionParam(); }
        public DisorderControlActionBuilder(bool bDisorder) { Param = new DisorderControlActionParam() { IsDisorder = bDisorder }; }
        public DisorderControlActionBuilder(DisorderControlActionParam param) { Param = param.Clone(); }

        public override string ToString()
        {
            return "顺序乱序组";
        }
    }

    /// <summary>
    /// 随机乱序执行操作
    /// </summary>
    public class DisorderControlAction : ControlAction
    {
        private DisorderControlActionParam param = new DisorderControlActionParam();
        public DisorderControlActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value); Name = ToString(); }
        }
        public DisorderControlAction():base()
        {
            Name = ToString();
        }

        public DisorderControlAction(LogicalActionBuilder builder) :base(builder)
        {
            Param = builder.GetParam() as DisorderControlActionParam;
        }

        public override string ToString()
        {
            return ""; 
        }

        private ObservableCollection<ObservableLogicalAction> DisorderActions(ObservableCollection<ObservableLogicalAction> org)
        {
            var collections = new List<ObservableLogicalAction>(org);

            var dst = new ObservableCollection<ObservableLogicalAction>();
            Random random = new Random();
            for (int i = 0; i < org.Count; i++)
            {
                int pick = random.Next(collections.Count);
                var item = collections[pick];
                dst.Add(item);
                collections.RemoveAt(pick);
            }
            return dst;
        }

        public override bool Act(object datacontext)
        {
            if (param.IsDisorder)
            {
                var actions = DisorderActions(ChildActions);
                return DoChildAction(actions, datacontext);
            }
            else
                return DoChildAction(ChildActions, datacontext);
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            var parent = new DisorderControlActionBuilder(Param);
            SerializeChildBuilder(parent);
            return parent;
        }
    }
}
