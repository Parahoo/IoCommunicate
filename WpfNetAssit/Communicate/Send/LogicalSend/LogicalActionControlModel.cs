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
        public ICommand ActionMoveUpCommand { get; }
        public ICommand ActionMoveDownCommand { get; }
        public ICommand ActionCopyCommand { get; }
        public ICommand ActionPasteCommand { get; }

        public LogicalActionControlModel()
        {
            SelectedItemChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(SelectedItemChanged);

            AddActionCommand = new RelayCommand(AddAction);
            DeleteActionCommand = new RelayCommand(DeleteAction);
            DeleteAllActionsCommand = new RelayCommand(DeleteAllActions);

            ActionMoveUpCommand = new RelayCommand(ActionMoveUp);
            ActionMoveDownCommand = new RelayCommand(ActionMoveDown);
            ActionCopyCommand = new RelayCommand(ActionCopy);
            ActionPasteCommand = new RelayCommand(ActionPaste);
        }

        public bool IsItemCanMoveUp { get; set; } = false;
        private void ActionMoveUp()
        {
            if (selectedAction != null)
            {
                var target = selectedAction;
                var parent = selectedAction.Parent;
                int index = parent.ChildActions.IndexOf(target);
                if (index > 0)
                {
                    parent.ChildActions.Move(index, index - 1);
                }
                IsItemCanMoveUp = index > 1; 
                IsItemCanMoveDown = true;
            }
        }


        public bool IsItemCanMoveDown { get; set; } = false;
        private void ActionMoveDown()
        {
            if (selectedAction != null)
            {
                var target = selectedAction;
                var parent = selectedAction.Parent;
                int index = parent.ChildActions.IndexOf(target);
                if (index != parent.ChildActions.Count-1)
                {
                    parent.ChildActions.Move(index, index + 1);
                }
                IsItemCanMoveUp = true;
                IsItemCanMoveDown = index < parent.ChildActions.Count - 2;
            }
        }

        public bool IsEnablePaste { get; set; } = false;
        private ObservableLogicalAction CopyActionSrc;
        private void ActionCopy()
        {
            if (selectedAction != null)
            {
                CopyActionSrc = selectedAction.SerializeBuilder().Build();
                IsEnablePaste = true;
            }
        }

        private void ActionPaste()
        {
            if (selectedAction != null && CopyActionSrc != null)
            {
                var builder = CopyActionSrc.SerializeBuilder();
                var action = builder.Build();
                InsertNewAction(action);
            }
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
                {
                    int index = selectedAction.Parent.ChildActions.IndexOf(selectedAction);
                    selectedAction.Parent.Insert(index+1,newAction);
                }
            }
            else
                RootAction.Add(newAction);
        }

        private void SelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            selectedAction = e.NewValue as ObservableLogicalAction;
            IsItemSelected = e.NewValue != null;
            IsEnablePaste = (e.NewValue != null) && (CopyActionSrc != null);

            CheckItemCanMoveUpDown(selectedAction);
        }

        private void CheckItemCanMoveUpDown(ObservableLogicalAction action)
        {
            if(action == null)
            {
                IsItemCanMoveUp = false;
                IsItemCanMoveDown = false;
            }
            else
            {
                int index = action.Parent.ChildActions.IndexOf(action);
                IsItemCanMoveUp = index > 0;
                IsItemCanMoveDown = index < action.Parent.ChildActions.Count - 1;
            }
        }

        public void BuildFrom(ControlActionBuilder rootbuild)
        {
            RootAction = new ControlAction(rootbuild);
        }
    }
}
