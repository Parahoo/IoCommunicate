using GalaSoft.MvvmLight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfNetAssit.LogicalAction.ControlAction
{
    public class ControlActionBuilder : LogicalActionBuilder
    {

        [System.Xml.Serialization.XmlIgnore]
        public bool IsContainer { get; set; } = true;


        public override ObservableLogicalAction Build()
        {
            return new ControlAction(this);
        }

        public override object GetParam()
        {
            return null;
        }
    }

    public class ControlAction : ObservableLogicalAction
    {
        public ControlAction()
        {
            ChildActions = new ObservableCollection<ObservableLogicalAction>();
        }

        public ControlAction(LogicalActionBuilder builder) :base(builder)
        {
            Name = "...";
        }

        protected bool DoChildAction(ObservableCollection<ObservableLogicalAction> actions, object datacontext)
        {
            var dict = datacontext as Dictionary<string, object>;
            var cancel = (CancellationToken) dict["canceltoken"];

            foreach (var action in actions)
            {
                cancel.ThrowIfCancellationRequested();
                if (!action.Act(datacontext))
                    return false;
            }
            return true;
        }

        public void AddLogicalAction(ObservableLogicalAction action)
        {
            ChildActions?.Add(action);
        }

        public void SwapChildActions(ControlAction other)
        {
            ObservableCollection<ObservableLogicalAction> t = other.ChildActions;
            other.ChildActions = this.ChildActions;
            this.ChildActions = t;
        }

        override public bool Act(object datacontext)
        {
            return DoChildAction(ChildActions, datacontext);
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            var parent = new ControlActionBuilder();
            SerializeChildBuilder(parent);
            return parent;
        }

        protected void SerializeChildBuilder(LogicalActionBuilder parent)
        {
            foreach (var item in ChildActions)
            {
                var child = item.SerializeBuilder();
                parent.Add(child);
            }
        }

        //public IEnumerator GetEnumerator()
        //{
        //    //return new ControlActionEnumertor(this);
        //    return ChildActions?.GetEnumerator();
        //}
    }
}
