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
using System.Windows.Input;
using WpfNetAssit.IoConnect;

namespace WpfNetAssit.LogicalAction.BaseAction
{
    [Serializable]
    public class ReadFileActionParam : ObservableObject
    {
        public string Info => (filepath);

        private string filepath = "";
        public string FilePath
        {
            get { return filepath; }
            set { Set("FilePath", ref filepath, value); RaisePropertyChanged("Info"); }
        }

        public ReadFileActionParam() { }
        public ReadFileActionParam(string v) { filepath = v; }
        public ReadFileActionParam Clone()
        {
            return new ReadFileActionParam(FilePath);
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
            if (fd.ShowDialog() == true)
                FilePath = fd.FileName;
        }
    }

    public class ReadFileActionBuilder : BaseActionBuilder
    {
        public ReadFileActionParam Param { get; set; }

        public override ObservableLogicalAction Build()
        {
            return new ReadFileAction(this);
        }

        public override object GetParam()
        {
            return Param.Clone();
        }

        public ReadFileActionBuilder() { Param = new ReadFileActionParam(); }
        public ReadFileActionBuilder(string v) { Param = new ReadFileActionParam() { FilePath = v }; }
        public ReadFileActionBuilder(ReadFileActionParam param) { Param = param.Clone(); }
        public override string ToString()
        {
            return "读取文件行";
        }
    }

    public class ReadFileAction : BaseAction
    {
        private ReadFileActionParam param = new ReadFileActionParam();
        public ReadFileActionParam Param
        {
            get { return param; }
            set { Set("Param", ref param, value); Name = ToString(); }
        }

        public override string ToString()
        {
            return "读取文件行: ";
        }

        public ReadFileAction(LogicalActionBuilder builder) : base(builder)
        {
            Param = builder.GetParam() as ReadFileActionParam;
        }

        public ReadFileAction() : base()
        {
        }

        public override bool Act(object datacontext)
        {
            var dict = datacontext as Dictionary<string, object>;

            Action<string, string> logfunc = dict["log"] as Action<string, string>;
            string tab = dict["tab"] as string;
            var cancel = (CancellationToken)dict["canceltoken"];

            logfunc("ReadFile line...", tab);
            try
            {
                if(dict.ContainsKey("file") == false)
                {
                    dict["file"] = new StreamReader(Param.FilePath);
                }
                var sr = dict["file"] as StreamReader;
                var buf = sr.ReadLine();
                if (buf != null)
                {
                    logfunc(string.Format("ReadFile line:{0}", buf), tab);
                    dict["filedata"] = Encoding.UTF8.GetBytes(buf);
                }
                else
                    logfunc(string.Format("ReadFile: finished"), tab);
                return buf != null;
            }
            catch (Exception e)
            {
                logfunc(string.Format("ReadFile exception: {0}", e.Message), tab);
                return false;
            }
        }

        override public LogicalActionBuilder SerializeBuilder()
        {
            return new ReadFileActionBuilder(Param);
        }
    }
}
