using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WpfNetAssit.LogicalAction.BaseAction;
using WpfNetAssit.LogicalAction.ControlAction;
using WpfNetAssit.LogicalAction.JudgmentAction;

namespace WpfNetAssit.LogicalAction
{

    [XmlInclude(typeof(ControlAction.ControlActionBuilder))]
    [XmlInclude(typeof(ControlAction.DisorderControlActionBuilder))]
    [XmlInclude(typeof(ControlAction.ForControlActionBuilder))]
    [XmlInclude(typeof(ControlAction.RetryControlActionBuilder))]
    [XmlInclude(typeof(ControlAction.PickControlActionBuilder))]
    [XmlInclude(typeof(ControlAction.OperatorControlActionBuilder))]
    [XmlInclude(typeof(BaseAction.SendActionBuilder))]
    [XmlInclude(typeof(BaseAction.RecvActionBuilder))]
    [XmlInclude(typeof(BaseAction.SleepActionBuilder))]
    [XmlInclude(typeof(JudgmentAction.CheckStringActionBuilder))]
    abstract public class LogicalActionBuilder
    {
        abstract public object GetParam();

        public List<LogicalActionBuilder> ChildBuilders { get; set; } = new List<LogicalActionBuilder>();

        public void Add(LogicalActionBuilder builder)
        {
            ChildBuilders.Add(builder);
        }

        abstract public ObservableLogicalAction Build();

        static public ObservableCollection<LogicalActionBuilder> GetAvaiableActionBuilders()
        {
            var AvaiableActionBuilders = new ObservableCollection<LogicalActionBuilder>();

            AvaiableActionBuilders.Add(new DisorderControlActionBuilder());
            AvaiableActionBuilders.Add(new ForControlActionBuilder());
            AvaiableActionBuilders.Add(new RetryControlActionBuilder());
            AvaiableActionBuilders.Add(new PickControlActionBuilder());
            AvaiableActionBuilders.Add(new OperatorControlActionBuilder());

            AvaiableActionBuilders.Add(new SendActionBuilder());
            AvaiableActionBuilders.Add(new RecvActionBuilder());
            AvaiableActionBuilders.Add(new SleepActionBuilder());

            AvaiableActionBuilders.Add(new CheckStringActionBuilder());

            return AvaiableActionBuilders;
        }
    }


    public class ObservableLogicalAction : ObservableObject, ILogicalAction
    {
        public string Name { get; set; } = "";

        public bool IsContainer { get { return ChildActions != null; } }

        public ObservableLogicalAction Parent { get; set; } = null;

        public ObservableCollection<ObservableLogicalAction> ChildActions { get; set; } = null;

        virtual public bool Act(object datacontext) { return true; }

        public ObservableLogicalAction() { }
        public ObservableLogicalAction(LogicalActionBuilder builder)
        {
            if (builder == null)
                return;
            if (builder.ChildBuilders == null)
                return;
            ChildActions = new ObservableCollection<ObservableLogicalAction>();
            foreach (var build in builder?.ChildBuilders)
            {
                var action = build.Build();
                Add(action);
            }
        }

        virtual public LogicalActionBuilder SerializeBuilder()
        {
            throw new NotImplementedException();
        }

        public void Add(ObservableLogicalAction child)
        {
            child.Parent = this;
            ChildActions.Add(child);
        }

        public void Insert(int index, ObservableLogicalAction child)
        {
            child.Parent = this;
            ChildActions.Insert(index, child);
        }
    }
}
