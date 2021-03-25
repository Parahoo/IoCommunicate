using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfNetAssit.Communicate;
using WpfNetAssit.IoConnect;
using WpfNetAssit.LogicalAction.ControlAction;

namespace WpfNetAssit.Pages
{
    public class CommunicateModel : ViewModelBase
    {
        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set { Set("IsSelected", ref isSelected, value); }
        }
        private string title = "";
        public string Title
        {
            get { return title; }
            set { Set("Title", ref title, value); }
        }

        private string info = "";
        public string Info
        {
            get { return info; }
            set { Set("Info", ref info, value); }
        }

        private bool isIoOk = false;
        public bool IsIoOk
        {
            get { return isIoOk; }
            set { Set("IsIoOk", ref isIoOk, value); }
        }


        private CommunicatePageModel communicatePageModel = new CommunicatePageModel();
        public CommunicatePageModel CommunicatePageModel
        {
            get { return communicatePageModel; }
            set { Set("CommunicatePageModel", ref communicatePageModel, value); }
        }

        public ICommand DeleteCommunicateCommand { get; }
        public ICommand ParentDeleteCommand { get; set; }

        private Action<byte[]> ProcessRecvData;

        public CommunicateModel()
        {
            DeleteCommunicateCommand = new RelayCommand(DeleteCommunicate);
            ProcessRecvData += communicatePageModel.ProcessRecvData;
        }

        Task recvTask = null;
        bool IsRun = false;
        IoConnect.ICommunicateIo Io = null;
        private void StartRecive(IoConnect.ICommunicateIo io)
        {
            Io = io;
            recvTask = Task.Run(() => {
                byte[] buf = new byte[0x1000];
                int size = 0;
                IsRun = true;
                while (IsRun)
                {
                    bool ret = io.Read(buf, ref size);
                    if (!ret)
                    {
                        IsIoOk = false;
                        Task.Delay(100).Wait();
                    }
                    else
                    {
                        if (IsIoOk == false)
                            IsIoOk = true;
                    }

                    if (ret && size > 0)
                    {
                        byte[] data = buf.Take(size).ToArray();
                        ProcessRecvData?.Invoke(data);
                    }
                }
                IsRun = false;
            });
        }

        private void StopRecive()
        {
            if (recvTask != null && !recvTask.IsCompleted)
            {
                IsRun = false;
                Io.Close();
                recvTask.Wait();
            }
        }

        internal void StartCommunicate(ICommunicateIo io)
        {
            communicatePageModel.StartCommunicate(io);
            StartRecive(io);
        }

        private void DeleteCommunicate()
        {
            StopRecive();
            communicatePageModel.StopCommunicate();
            ParentDeleteCommand?.Execute(this);
        }

        override public string ToString() 
        {
            return "WpfNetAssit.Pages.CommunicateModel " +  Title ;
;
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public bool IoDialogIsOpen { get; set; } = false;

        public IoSelectPageViewModel IoSelectPageViewModel { get; set; } = new IoSelectPageViewModel();

        public ObservableCollection<CommunicateModel> CommunicateIos { get; set; } = new ObservableCollection<CommunicateModel>();


        public ICommand CloseCommand { get; }
        public ICommand ShowAddCommand { get; }



        public MainWindowViewModel()
        {
            UserSetting.Default = UserSetting.Load();
            IoSelectPageViewModel.IosSetting = UserSetting.Default.IosSetting;
            IoSelectPageViewModel.StartIoCommand = new RelayCommand<ICommunicateIo>(AddNewCommunicate);
            CloseCommand = new RelayCommand(Close1);
            ShowAddCommand = new RelayCommand(ShowAddAsync);

        }

        public void Close1()
        {
            if (CommunicateIos.Count > 0)
            {
                UserSetting.Default.CommunicatePageModel = CommunicateIos.Last().CommunicatePageModel;
                UserSetting.Default.RootBuilder = UserSetting.Default.CommunicatePageModel.SendPageModel.LogicalSendPageModel.LogicalActionControlModel.RootAction.SerializeBuilder() as ControlActionBuilder;
            }
            UserSetting.Default.Save();
            App.Current.MainWindow.Close();
        }

        void AddNewCommunicate(ICommunicateIo io)
        {
            CommunicateModel communicate = new CommunicateModel();
            communicate.Title = io.NickName;
            communicate.CommunicatePageModel.SendPageModel.LogicalSendPageModel.LogicalActionControlModel.BuildFrom(UserSetting.Default.RootBuilder);
            communicate.ParentDeleteCommand = new RelayCommand<CommunicateModel>(DeleteCommunicate);
            CommunicateIos.Add(communicate);
            communicate.StartCommunicate(io);
            communicate.Info = io.LinkInfo;
            communicate.IsIoOk = io.IsLinkOk;

            SwitchTo(CommunicateIos.Count - 1);
            IoDialogIsOpen = false;
        }

        private void SwitchTo(int v)
        {
            int i = 0;
            foreach (var io in CommunicateIos)
            {
                if (i != v && io.IsSelected == true)
                    io.IsSelected = false;
                else if(i == v && io.IsSelected == false)
                    io.IsSelected = true;
                i++;
            }
        }

        private void DeleteCommunicate(CommunicateModel communicateModel)
        {
            if(communicateModel != null)
            {
                CommunicateIos.Remove(communicateModel);
            }
        }

        private async void ShowAddAsync()
        {
            await MaterialDesignThemes.Wpf.DialogHost.Show(IoSelectPageViewModel);
        }
    }
}
