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
using WpfNetAssit.Pages;

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

        private bool isOpenLogView = false;
        public bool IsOpenLogView
        {
            get { return isOpenLogView; }
            set { Set("IsOpenLogView", ref isOpenLogView, value); }
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

        private TextMonitorModel textMonitorModel = new TextMonitorModel();
        public TextMonitorModel TextMonitorModel
        {
            get { return textMonitorModel; }
            set { Set("TextMonitorModel", ref textMonitorModel, value); }
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
                IsOpenLogView = true;

                if (!Directory.Exists("./log/"))
                    Directory.CreateDirectory("./log/");
                string logfilename = "./log/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".log";
                FileStream fs = new FileStream(logfilename, FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fs);
                Dictionary<string, object> datacontext = new Dictionary<string, object>();
                try
                {

                    ioPipe = new IoPipe() { Io = Io };
                    datacontext.Add("io", ioPipe);

                    cancellationTokensource = new CancellationTokenSource();
                    CancellationToken cancellationToken = cancellationTokensource.Token;
                    datacontext.Add("canceltoken", cancellationToken);

                    datacontext.Add("log", new Action<string, string>((string log, string tab) =>
                    {
                        string text = tab + log;
                        OutputLog(streamWriter, text);
                    }));
                    datacontext.Add("tab", "");

                    TextMonitorModel.ClearMonitor();
                    logicSendTask = Task.Run(() =>
                    {
                        try
                        {
                            var ok = LogicalActionControlModel.RootAction.Act(datacontext);
                            OutputLog(streamWriter, ok ? "完成":"出错停止");
                        }
                        catch (Exception e)
                        {
                            OutputLog(streamWriter, e.Message);
                        }
                        IsRuningFrozen = false;
                    }, cancellationToken);
                    await logicSendTask;
                }
                finally
                {
                    if(datacontext.ContainsKey("file"))
                    {
                        (datacontext["file"] as StreamReader).Dispose();
                    }
                    streamWriter.Dispose();
                    fs.Dispose();
                    ioPipe = null;
                }
            }
            else
            {
                cancellationTokensource.Cancel();
            }
        }
        public async void Stop()
        {
            if(IsRuningFrozen)
            {
                cancellationTokensource.Cancel();
                await logicSendTask;
            }
        }

        private void OutputLog(StreamWriter streamWriter, string text)
        {
            TextMonitorModel.MonitorData(text + "\r\n");
            streamWriter.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + text);
            streamWriter.Flush();
        }

        IoPipe ioPipe = null;
        public void RecvData(byte[] data)
        {
            ioPipe?.RecvData(data);
        }
    }
}
