﻿using GalaSoft.MvvmLight;
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

    public class DisorderControlActionBuilder : ControlActionBuilder
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

        Random random = new Random();
        private ObservableCollection<ObservableLogicalAction> DisorderActions(ObservableCollection<ObservableLogicalAction> org)
        {
            var collections = new List<ObservableLogicalAction>(org);

            var dst = new ObservableCollection<ObservableLogicalAction>();
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
            bool ret = false;
            Dictionary<string, object> dict = datacontext as Dictionary<string, object>;
            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;
            dict["tab"] = tab + "  ";

            if (param.IsDisorder)
            {
                logfunc("乱序：", tab);
                var actions = DisorderActions(ChildActions);
                ret = DoChildAction(actions, datacontext);
            }
            else
            {
                logfunc("顺序：", tab);
                ret = DoChildAction(ChildActions, datacontext);
            }

            dict["tab"] = tab;
            return ret;
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            var parent = new DisorderControlActionBuilder(Param);
            SerializeChildBuilder(parent);
            return parent;
        }
    }
}
