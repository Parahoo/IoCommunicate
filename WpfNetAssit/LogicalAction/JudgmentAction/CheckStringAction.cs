using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNetAssit.LogicalAction.BaseAction;

namespace WpfNetAssit.LogicalAction.JudgmentAction
{
    [Serializable]
    public class CheckStringActionParam : ObservableObject
    {
        public string Info => (IsHaveOrNot ? "包含 ":"不包含 ") +TargetString;

        private bool isHaveOrNot = true;
        public bool IsHaveOrNot
        {
            get { return isHaveOrNot; }
            set { Set("IsHaveOrNot", ref isHaveOrNot, value); }
        }


        private string targetString = "";
        public string TargetString
        {
            get { return targetString; }
            set { Set("TargetString", ref targetString, value); }
        }


        public CheckStringActionParam() { }
        public CheckStringActionParam(string v, bool ishave) { TargetString = v; IsHaveOrNot = ishave; }
        public CheckStringActionParam Clone()
        {
            return new CheckStringActionParam( TargetString , IsHaveOrNot );
        }
    }

    public class CheckStringActionBuilder : BaseActionBuilder
    {
        public CheckStringActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new CheckStringAction(this);
        }

        public override object GetParam()
        {
            return Param.Clone();
        }

        public CheckStringActionBuilder() { Param = new CheckStringActionParam(); }
        public CheckStringActionBuilder(string v) { Param = new CheckStringActionParam() { TargetString = v }; }
        public CheckStringActionBuilder(CheckStringActionParam param) { Param = param.Clone(); }
        public override string ToString()
        {
            return "检查字符串";
        }
    }

    public class CheckStringAction : BaseAction.BaseAction
    {
        private CheckStringActionParam param = new CheckStringActionParam();
        public CheckStringActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value); Name = ToString(); }
        }

        public override string ToString()
        {
            return "检查字符串: ";
        }

        public CheckStringAction(LogicalActionBuilder builder) : base(builder)
        {
            Param = builder.GetParam() as CheckStringActionParam;
        }

        public CheckStringAction() : base()
        {
        }

        public override bool Act(object datacontext)
        {
            var dict = datacontext as Dictionary<string, object>;

            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;

            var ret = false;
            try
            {
                var data = dict["recvdata"] as byte[];

                if (data != null)
                    ret = Encoding.UTF8.GetString(data).Contains(param.TargetString);
                if (param.IsHaveOrNot == false)
                    ret = !ret;
            }
            catch (Exception)
            {
                ret = false;
            }
            logfunc(string.Format("检查{0}包含 {1} :{2}", Param.IsHaveOrNot?"":"不",Param.TargetString, ret), tab);
            return ret;
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            return new CheckStringActionBuilder(Param);
        }
    }

}
