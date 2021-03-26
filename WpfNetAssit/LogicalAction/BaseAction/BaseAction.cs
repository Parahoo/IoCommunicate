using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.LogicalAction.BaseAction
{
    public class BaseActionBuilder : LogicalActionBuilder
    {

        [System.Xml.Serialization.XmlIgnore]
        public bool IsContainer { get; set; } = false;


        public override ObservableLogicalAction Build()
        {
            return new BaseAction(this);
        }

        public override object GetParam()
        {
            return null;
        }
    }

    public class BaseAction : ObservableLogicalAction
    {
        public BaseAction() { }
        public BaseAction(LogicalActionBuilder builder):base(builder)
        {
            ChildActions = null;
        }
    }
}
