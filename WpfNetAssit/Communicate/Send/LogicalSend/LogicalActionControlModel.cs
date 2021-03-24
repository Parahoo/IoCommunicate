using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using WpfNetAssit.LogicalAction;
using WpfNetAssit.LogicalAction.BaseAction;
using WpfNetAssit.LogicalAction.ControlAction;

namespace WpfNetAssit.Communicate.Send.LogicalSend
{
    public class LogicalActionControlModel : ViewModelBase
    {
        public bool IsDialogOpen { get; set; } = false;

        [System.Xml.Serialization.XmlIgnore]
        private ControlAction rootAction;
        [System.Xml.Serialization.XmlIgnore]
        public ControlAction RootAction
        {
            get { return rootAction; }
            set { Set("RootAction", ref rootAction, value); }
        }

        private bool isItemSelected = false;
        public bool IsItemSelected
        {
            get { return isItemSelected; }
            set { Set("IsItemSelected", ref isItemSelected, value); }
        }


        public ICommand SelectedItemChangedCommand { get; }

        public ICommand AddActionCommand { get; }
        public ICommand DeleteActionCommand { get; }
        public ICommand DeleteAllActionsCommand { get; }

        public LogicalActionControlModel()
        {
            SelectedItemChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(SelectedItemChanged);

            AddActionCommand = new RelayCommand(AddAction);
            DeleteActionCommand = new RelayCommand(DeleteAction);
            DeleteAllActionsCommand = new RelayCommand(DeleteAllActions);
        }

        private void DeleteAction()
        {
            if (selectedAction != null)
            {
                var parent = selectedAction.Parent;
                parent.ChildActions.Remove(selectedAction);
            }
        }

        private void DeleteAllActions()
        {
            RootAction.ChildActions.Clear();
        }

        private  void AddAction()
        {
            var vm = new LogicalActionAddControlModel();
            vm.AddActionToTarget = InsertNewAction; //new RelayCommand<ObservableLogicalAction>();
             MaterialDesignThemes.Wpf.DialogHost.Show(vm);
        }
        private ObservableLogicalAction selectedAction;
        private void InsertNewAction(ObservableLogicalAction newAction)
        {
            if (selectedAction != null)
            {
                if (selectedAction.IsContainer)
                    selectedAction.Add(newAction);
                else
                    selectedAction.Parent.Add(newAction);
            }
            else
                RootAction.Add(newAction);
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void SelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            selectedAction = e.NewValue as ObservableLogicalAction;
            IsItemSelected = e.NewValue != null;

        }

        public void BuildFrom(ControlActionBuilder rootbuild)
        {
            RootAction = new ControlAction(rootbuild);
        }
    }
}
