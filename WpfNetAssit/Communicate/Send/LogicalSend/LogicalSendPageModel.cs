using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNetAssit.IoConnect;
using WpfNetAssit.LogicalAction;
using WpfNetAssit.LogicalAction.ControlAction;

namespace WpfNetAssit.Communicate.Send.LogicalSend
{
    public class LogicalSendPageModel : ViewModelBase
    {

        private bool isOpenHost = false;
        public bool IsOpenHost
        {
            get { return isOpenHost; }
            set { Set("IsOpenHost", ref isOpenHost, value); }
        }

        private bool isRuningFrozen = false;
        public bool IsRuningFrozen
        {
            get { return isRuningFrozen; }
            set { Set("IsRuningFrozen", ref isRuningFrozen, value); }
        }


        private LogicalActionControlModel logicalActionControlModel = new LogicalActionControlModel();
        public LogicalActionControlModel LogicalActionControlModel
        {
            get { return logicalActionControlModel; }
            set { Set("LogicalActionControlModel", ref logicalActionControlModel, value); }
        }

        private LogicalActionControlSettingPageModel logicalActionControlSettingPageModel = new LogicalActionControlSettingPageModel();
        public LogicalActionControlSettingPageModel LogicalActionControlSettingPageModel
        {
            get { return logicalActionControlSettingPageModel; }
            set { Set("LogicalActionControlSettingPageModel", ref logicalActionControlSettingPageModel, value); }
        }


        private ICommunicateIo Io = null;
        public void SetIo(ICommunicateIo io) { Io = io; }

        public ICommand StartCommand { get; }

        public ICommand OpenSettingCommand { get; }

        public LogicalSendPageModel()
        {
            StartCommand = new RelayCommand(Start);
            OpenSettingCommand = new RelayCommand(OpenSetting);
        }

        private void OpenSetting()
        {
            LogicalActionControlSettingPageModel.RootAction = LogicalActionControlModel.RootAction;
            LogicalActionControlSettingPageModel.Open((ControlAction controlAction)=> {
                LogicalActionControlModel.RootAction = controlAction;
            });
        }

        Task logicSendTask = null;
        CancellationTokenSource cancellationTokensource = new CancellationTokenSource();
        private async void Start()
        {
            if (isRuningFrozen == false)
            {
                IsRuningFrozen = true; 
                cancellationTokensource = new CancellationTokenSource();
                CancellationToken cancellationToken = cancellationTokensource.Token;

                Dictionary<string, object> datacontext = new Dictionary<string, object>();
                datacontext.Add("io", Io);
                datacontext.Add("canceltoken", cancellationToken);

                logicSendTask = Task.Run(() =>
                {
                    try
                    {
                        LogicalActionControlModel.RootAction.Act(datacontext); 

                    }
                    catch (Exception)
                    {

                    }
                    IsRuningFrozen = false;
                }, cancellationToken);
                await logicSendTask;
            }
            else
            {
                cancellationTokensource.Cancel();
            }
        }
    }
}
