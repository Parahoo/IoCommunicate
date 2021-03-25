using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNetAssit.LogicalAction;
using WpfNetAssit.LogicalAction.BaseAction;
using WpfNetAssit.LogicalAction.ControlAction;

namespace WpfNetAssit.Communicate.Send.LogicalSend
{
    public class LogicalActionAddControlModel : ViewModelBase
    {
        private ObservableCollection<LogicalActionBuilder> avaiableActionBuilders = new ObservableCollection<LogicalActionBuilder>();
        public ObservableCollection<LogicalActionBuilder> AvaiableActionBuilders
        {
            get { return avaiableActionBuilders; }
            set { Set("AvaiableActions", ref avaiableActionBuilders, value); }
        }

        private LogicalActionBuilder selectedActionBuilder = null;
        public LogicalActionBuilder SelectedActionBuilder
        {
            get { return selectedActionBuilder; }
            set { Set("SelectedAction", ref selectedActionBuilder, value); IsSelected = selectedActionBuilder != null; }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set { Set("IsSelected", ref isSelected, value); }
        }

        public ObservableLogicalAction NewAction { get; private set; }

        public ICommand AddEndActionCommand { get; }

        public Action<ObservableLogicalAction> AddActionToTarget;


        public LogicalActionAddControlModel()
        {
            InitAvaiableActionBuilders();

            AddEndActionCommand = new RelayCommand(AddEndAction);
        }

        private void AddEndAction()
        {
            NewAction = SelectedActionBuilder?.Build();
            AddActionToTarget?.Invoke(NewAction);
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public void InitAvaiableActionBuilders()
        {
            AvaiableActionBuilders.Add(new DisorderControlActionBuilder());
            AvaiableActionBuilders.Add(new ForControlActionBuilder());
            AvaiableActionBuilders.Add(new SendActionBuilder());
            AvaiableActionBuilders.Add(new SleepActionBuilder());
        }
    }
}
