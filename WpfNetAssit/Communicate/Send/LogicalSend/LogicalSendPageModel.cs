using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
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

        private string displayLogString = "";
        public string DisplayLogString
        {
            get { return displayLogString; }
            set { Set("DisplayLogString", ref displayLogString, value); }
        }

        /// <summary>
        /// 显示接收到的数据
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="size"></param>
        private void MonitorLog(string log)
        {
            if (DisplayLogString.Length > 10000)
                DisplayLogString = DisplayLogString.Substring(2500) + log;
            else
                DisplayLogString += log;
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

                if (!Directory.Exists("./log/"))
                    Directory.CreateDirectory("./log/");
                string logfilename = "./log/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".log";
                FileStream fs = new FileStream(logfilename, FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fs);
                try
                {
                    Dictionary<string, object> datacontext = new Dictionary<string, object>();
                    datacontext.Add("io", Io);
                    datacontext.Add("canceltoken", cancellationToken);
                    datacontext.Add("log", new Action<string,string>((string log, string tab) =>
                     {
                         string text = tab + log;
                         MonitorLog(text+"\r\n");
                         streamWriter.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ")+ text);
                         streamWriter.Flush();
                     }));
                    datacontext.Add("tab", "");

                    DisplayLogString = "";
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
                finally
                {
                    streamWriter.Dispose();
                    fs.Dispose();
                }
            }
            else
            {
                cancellationTokensource.Cancel();
            }
        }
    }
}
