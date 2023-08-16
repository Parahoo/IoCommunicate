using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfNetAssit.Communicate;

namespace WpfNetAssit.LogicalAction.BaseAction
{
    [Serializable]
    public class SendFileActionParam : ObservableObject
    {
        public string Info => ToString();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(filepath);
            sb.Append(string.Format(" [block:{0},{1}ms]", blocksize, gaptime));
            return sb.ToString();
        }

        private string filepath = "";
        public string FilePath
        {
            get { return filepath; }
            set { Set("FilePath", ref filepath, value); RaisePropertyChanged("Info"); }
        }

        private int blocksize = 4096;
        public int BlockSize
        {
            get { return blocksize; }
            set { Set("BlockSize", ref blocksize, value); RaisePropertyChanged("Info"); }
        }

        private int gaptime = 0;
        public int GapTime
        {
            get { return gaptime; }
            set { Set("GapTime", ref gaptime, value); RaisePropertyChanged("Info"); }
        }



        public SendFileActionParam() { }
        public SendFileActionParam(string v, int blocksize, int gaptime)
        {
            FilePath = v; 
            BlockSize = blocksize;
            GapTime = gaptime;
        }
        public SendFileActionParam Clone()
        {
            return new SendFileActionParam(FilePath, BlockSize, gaptime);
        }

        private RelayCommand selectFileCommand;

        public ICommand SelectFileCommand
        {
            get
            {
                if (selectFileCommand == null)
                {
                    selectFileCommand = new RelayCommand(SelectFile);
                }

                return selectFileCommand;
            }
        }

        private void SelectFile()
        {
            // this is used to select file
            var fd = new OpenFileDialog();
            if(fd.ShowDialog() == true)
                FilePath = fd.FileName;
        }
    }

    public class SendFileActionBuilder : BaseActionBuilder
    {
        public SendFileActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new SendFileAction(this);
        }

        public override object GetParam()
        {
            return Param.Clone();
        }

        public SendFileActionBuilder() { Param = new SendFileActionParam(); }
        public SendFileActionBuilder(SendFileActionParam param) { Param = param.Clone(); }
        public override string ToString()
        {
            return "发送文件";
        }
    }

    public class SendFileAction : BaseAction
    {
        private SendFileActionParam param = new SendFileActionParam();
        public SendFileActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value); Name = ToString(); }
        }

        public override string ToString()
        {
            return "发送文件: ";
        }

        public SendFileAction(LogicalActionBuilder builder) : base(builder)
        {
            Param = builder.GetParam() as SendFileActionParam;
        }

        public SendFileAction() : base()
        {
        }

        public override bool Act(object datacontext)
        {
            var dict = datacontext as Dictionary<string, object>;

            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;
            var cancel = (CancellationToken)dict["canceltoken"];
            logfunc(string.Format("send: {0}", Param.Info), tab);

            if(File.Exists(Param.FilePath) == false)
            {
                logfunc(string.Format("send: {0}", "file not exist"), tab);
                return false;
            }

            try
            {
                var io = dict["io"] as IoPipe;
                using (FileStream fs = new FileStream(Param.FilePath, FileMode.Open, FileAccess.Read))
                {
                    while (true)
                    {
                        var buf = new byte[Param.BlockSize];
                        int readsize = fs.Read(buf, 0, buf.Length);
                        if(readsize == 0)
                            break;

                        int writesize = readsize;
                        var ret = io.Write(buf, ref writesize);
                        if (ret == false)
                        {
                            logfunc(string.Format("send: {0}", "faild"), tab);
                            return false;
                        }

                        if (Param.GapTime > 0)
                            Task.Delay(Param.GapTime, cancel).Wait();

                        if (cancel.IsCancellationRequested)
                        {
                            logfunc(string.Format("send: {0}", "cancel"), tab);
                            break;
                        }
                    }

                }

                logfunc(string.Format("send: {0}", "ok"), tab);
                return true;
            }
            catch (Exception)
            {
                logfunc(string.Format("send: {0}", "file faild"), tab);
                return false;
            }
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            return new SendFileActionBuilder(Param);
        }
    }
}
